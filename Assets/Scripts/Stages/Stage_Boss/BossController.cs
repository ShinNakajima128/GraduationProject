using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(CharacterController))]
public class BossController : MonoBehaviour, IDamagable
{
    #region serialize
    [Header("Variables")]
    [Tooltip("移動速度")]
    [SerializeField]
    float _defaultMoveSpeed = 5.0f;

    [Tooltip("旋回速度")]
    [SerializeField]
    float _turnSpeed = 5.0f;

    [Tooltip("ジャンプして上まで行くのにかかる時間")]
    [SerializeField]
    float _jumpUpTime = 1.5f;

    [Tooltip("着地するまでにかかる時間")]
    [SerializeField]
    float _fallDownTime = 0.5f;

    [Tooltip("攻撃可能になるまでの時間")]
    [SerializeField]
    float _attackInterval = 3.0f;

    [Tooltip("ダウン時間")]
    [SerializeField]
    float _downTime = 3.0f;

    [Tooltip("倒された時の時間")]
    [SerializeField]
    float _knockOutTime = 6.5f;

    [Tooltip("攻撃を開始するプレイヤーとの距離")]
    [SerializeField]
    float _attackDistance = 2.0f;

    [SerializeField]
    float _bounceAttackInterval = 1.0f;

    [SerializeField]
    float _minShadowSize = 0f;

    [Header("Phase")]
    [Tooltip("ボス戦の各フェイズのパラメータ")]
    [SerializeField]
    PhaseParameter[] _phaseParams = default;

    [Header("Positions")]
    [SerializeField]
    Transform _directionTrans = default;

    [SerializeField]
    Transform _startBattleTrans = default;

    [Header("Debug")]
    [SerializeField]
    bool _debugMode = false;

    [SerializeField]
    BossBattlePhase _debugPhase = default;
    #endregion

    #region private
    Animator _anim;
    CharacterController _cc;
    BossShadow _bossShadow;
    Transform _playerTrans = default;
    BossState _currentBossState = default;
    BossBattlePhase _currentBattlePhase = default;
    Vector3 _dir;
    Vector3 _velocity;
    float _currentMoveSpeed;
    bool _isInBattle = false;
    bool _isWaiting = false;
    bool _isDamaged = false;
    bool _isCanAttack = true;
    bool _isApproached = false;
    bool _isInvincibled = false;
    Coroutine _jumpCoroutine;

    public bool IsInvincibled => _isInvincibled;
    #endregion
    #region public
    #endregion
    #region property
    #endregion

    private void Awake()
    {
        TryGetComponent(out _anim);
        TryGetComponent(out _cc);
        TryGetComponent(out _bossShadow);
    }
    void Start()
    {
        _playerTrans = GameObject.FindGameObjectWithTag("Player").transform;
        _currentMoveSpeed = _defaultMoveSpeed;
        BossStageManager.Instance.CharacterMovable += BossMovable;
        MessagePlayer.Instance.Closeup += () => PlayBossAnimation(BossAnimationType.Angry);
    }

    void FixedUpdate()
    {
        if (_currentBossState == BossState.Move || _currentBossState == BossState.Chase)
        {
            OnMove();
        }
    }

    void BossMovable(bool isMove)
    {
        _isInBattle = isMove;
    }

    private void OnMove()
    {

        if (_currentBossState != BossState.Move)
        {
            _anim.CrossFadeInFixedTime("Move", 0.1f);
        }
        _dir = _playerTrans.position - transform.position;
        _dir.y = 0;

        //プレイヤーに攻撃する距離まで近づいているかの判定結果を取得
        _isApproached = Vector3.Distance(transform.position, _playerTrans.position) < _attackDistance;

        if (_currentBossState == BossState.Move)
        {
            if (_isApproached && _isCanAttack)
            {
                StartCoroutine(ChangeState(BossState.Attack_Jump));
                return;
            }
        }
        else
        {
            if (_isApproached)
            {
                return;
            }
        }

        Quaternion targetRotation = Quaternion.LookRotation(_dir.normalized);
        targetRotation.x = 0;
        targetRotation.z = 0;
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _turnSpeed * Time.deltaTime);
        _velocity = _dir.normalized * _currentMoveSpeed;
        _cc.Move(_velocity * Time.deltaTime);
    }

    /// <summary>
    /// ボスのアニメーションを再生する
    /// </summary>
    /// <param name="type"> 指定したアニメーション </param>
    /// <param name="fadeTime"> 遷移にかける時間 </param>
    public void PlayBossAnimation(BossAnimationType type, float fadeTime = 0.1f)
    {
        _anim.CrossFadeInFixedTime(type.ToString(), fadeTime);
    }

    /// <summary>
    /// 戦闘フェイズのコルーチン
    /// </summary>
    /// <param name="battlePhase"> 戦闘フェイズの種類 </param>
    public IEnumerator BattlePhaseCoroutine(BossBattlePhase battlePhase, Action action = null)
    {
        _isDamaged = false;
        _bossShadow.ChangeShadowSize(0.1f, 0f);
        PlayBossAnimation(BossAnimationType.Jump);

        yield return new WaitForSeconds(1.3f);

        _bossShadow.ChangeShadowSize(2f, 1.0f);
        yield return transform.DOMove(_startBattleTrans.position, 1.0f).WaitForCompletion();

        BossStageManager.CameraShake();

        action?.Invoke();
        yield return new WaitForSeconds(2.5f);

        PlayBossAnimation(BossAnimationType.Idle);

        _currentBattlePhase = battlePhase;

        StartCoroutine(ChangeState(BossState.Move));

        while (!_isDamaged)
        {
            yield return null;
        }
    }

    /// <summary>
    /// ジャンプ攻撃のコルーチン
    /// </summary>
    IEnumerator JumpAttack()
    {
        _isWaiting = true;
        _isCanAttack = false;

        if (_debugMode)
        {
            _currentBattlePhase = _debugPhase;
        }

        var param = _phaseParams.FirstOrDefault(p => p.Phase == _currentBattlePhase); //現在のフェイズの各パラメーターを取得

        PlayBossAnimation(BossAnimationType.JumpUp);
        _currentMoveSpeed = param.MoveSpeed;
        Debug.Log(_currentMoveSpeed);
        transform.DOLookAt(_playerTrans.position, 0.1f, AxisConstraint.Y);

        yield return new WaitForSeconds(1.3f);

        BossStageManager.CameraChange(CameraType.JumpAttack, 1.5f);

        var playerTop = new Vector3(_playerTrans.position.x, _playerTrans.position.y + 10.0f, _playerTrans.position.z);

        _bossShadow.ChangeShadowSize(_minShadowSize, _jumpUpTime + param.ChaseTime); //影を徐々に小さくする

        //プレイヤーの頭上へジャンプする
        yield return transform.DOLocalMove(playerTop, _jumpUpTime)
                              .SetEase(Ease.OutCubic)
                              .WaitForCompletion(); //ボスが飛び上がる

        transform.DOLookAt(_playerTrans.position, 0.1f, AxisConstraint.Y);

        float timer = 0f;

        StartCoroutine(ChangeState(BossState.Chase)); //プレイヤーを追うステータスに切り替える

        //追跡時間が経過するまで処理を待機
        while (timer < param.ChaseTime)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        yield return null;

        //ボスの影を徐々に大きくする
        _bossShadow.ChangeShadowSize(2f, param.FallTime, Ease.InCubic);

        //プレイヤーの現在地に着地する
        yield return transform.DOMove(_playerTrans.position, param.FallTime)
                              .SetEase(Ease.InCubic)
                              .OnComplete(() =>
                              {
                                  StartCoroutine(ChangeState(BossState.Landing));
                                  BossStageManager.CameraShake();
                                  EffectManager.PlayEffect(EffectType.ShockWave, transform.position);
                                  Debug.Log("着地");
                              })
                              .WaitForCompletion();

        //通常戦闘のカメラに切り替える
        BossStageManager.CameraChange(CameraType.Battle, 2f);

        //追撃回数が1回以上の場合
        if (param.BounceCount > 0)
        {
            PlayBossAnimation(BossAnimationType.Bounce);

            //設定された回数だけバウンドを行う
            for (int i = 0; i < param.BounceCount; i++)
            {
                yield return transform.DOLocalJump(transform.position + (transform.forward * 2f), 1.5f, 1, 0.8f)
                                      .SetEase(Ease.Linear)
                                      .WaitForCompletion();

                EffectManager.PlayEffect(EffectType.ShockWave, transform.position);
            }
        }

        StartCoroutine(ChangeState(BossState.Landing));

        //攻撃後のクールタイム
        yield return new WaitForSeconds(param.CoolTime);

        StartCoroutine(ChangeState(BossState.Move));
        StartCoroutine(AttackInterval(_attackInterval));
        _currentMoveSpeed = _defaultMoveSpeed;
        _isWaiting = false;
    }

    /// <summary>
    /// ダメージを受けた時のコルーチン
    /// </summary>
    /// <param name="action"> ダウン状態が終わった時に実行するAction </param>
    /// <returns></returns>
    IEnumerator DamageCoroutine(Action action)
    {
        //攻撃中にダメージを受けた場合は攻撃処理を中断
        if (_jumpCoroutine != null)
        {
            StopCoroutine(_jumpCoroutine);
            _jumpCoroutine = null;

            //攻撃時に使用している各値をリセット
            _currentMoveSpeed = _defaultMoveSpeed;
            _isWaiting = false;
            _isCanAttack = true;

            yield return null;
        }

        //3フェイズ目の時は倒されたモーションを再生
        if (_currentBattlePhase == BossBattlePhase.Third)
        {
            PlayBossAnimation(BossAnimationType.Defeat);
            yield return new WaitForSeconds(_knockOutTime);
        }
        else
        {
            PlayBossAnimation(BossAnimationType.Damage);
            yield return new WaitForSeconds(_downTime);
        }

        _isDamaged = true;
        action?.Invoke();
    }

    /// <summary>
    /// ボスのステータスを変更する
    /// </summary>
    /// <param name="state"> 変更するステータス </param>
    /// <param name="waitTime"> 変更後に行うアクションの待機時間 </param>
    /// <param name="action"> 変更後に行うアクション </param>
    /// <returns></returns>
    IEnumerator ChangeState(BossState state, float waitTime = 0.02f, Action action = null)
    {
        _currentBossState = state;

        switch (_currentBossState)
        {
            case BossState.Idle:
                PlayBossAnimation(BossAnimationType.Idle);
                break;
            case BossState.Wait:
                break;
            case BossState.Direction:
                break;
            case BossState.Move:
                PlayBossAnimation(BossAnimationType.Move);
                break;
            case BossState.Attack_Jump:
                _jumpCoroutine = StartCoroutine(JumpAttack());
                break;
            case BossState.Landing:
                PlayBossAnimation(BossAnimationType.Landing);
                break;
            case BossState.Damage:
                StartCoroutine(DamageCoroutine(action));
                break;
            case BossState.KnockOut:
                PlayBossAnimation(BossAnimationType.Defeat);
                break;
            default:
                break;
        }
        if (_currentBossState == BossState.Damage)
        {
            yield break;
        }

        yield return new WaitForSeconds(waitTime);

        action?.Invoke();
    }
    IEnumerator AttackInterval(float interval)
    {
        yield return new WaitForSeconds(interval);

        _isCanAttack = true;
    }

    /// <summary>
    /// 被弾
    /// </summary>
    /// <param name="value"> 受けるダメージ </param>
    public void Damage(int value)
    {
        StartCoroutine(ChangeState(BossState.Damage, action: () => _isInvincibled = false));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.TryGetComponent<IDamagable>(out var target))
            {
                target.Damage(1);
                Debug.Log("ボスがプレイヤーへダメージを与えた");
            }
        }
    }
}

public enum BossState
{
    /// <summary> 何もしない </summary>
    Idle,
    /// <summary> モーション後の待機 </summary>
    Wait,
    /// <summary> 演出中 </summary>
    Direction,
    /// <summary> 移動 </summary>
    Move,
    /// <summary> 追いかける </summary>
    Chase,
    /// <summary> ジャンプ攻撃 </summary>
    Attack_Jump,
    /// <summary> 着地 </summary>
    Landing,
    /// <summary> 被弾 </summary>
    Damage,
    /// <summary> 倒された </summary>
    KnockOut
}

public enum BossAnimationType
{
    Idle,
    Move,
    Jump,
    JumpUp,
    Falling,
    Landing,
    Angry,
    Damage,
    Bounce,
    Defeat
}

public enum BossBattlePhase
{
    First,
    Second,
    Third
}

[Serializable]
public struct PhaseParameter
{
    [Tooltip("設定するフェイズ")]
    public BossBattlePhase Phase;
    [Tooltip("ジャンプ中の移動速度")]
    public float MoveSpeed;
    [Tooltip("追跡する時間")]
    public float ChaseTime;
    [Tooltip("落下にかける時間")]
    public float FallTime;
    [Tooltip("攻撃後に停止する時間")]
    public float CoolTime;
    [Tooltip("ジャンプ攻撃後に跳ねる回数"), Range(0, 3)]
    public int BounceCount;
}