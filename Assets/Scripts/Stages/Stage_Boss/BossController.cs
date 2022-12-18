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
    [Tooltip("�ړ����x")]
    [SerializeField]
    float _defaultMoveSpeed = 5.0f;

    [Tooltip("���񑬓x")]
    [SerializeField]
    float _turnSpeed = 5.0f;

    [Tooltip("�W�����v���ď�܂ōs���̂ɂ����鎞��")]
    [SerializeField]
    float _jumpUpTime = 1.5f;

    [Tooltip("���n����܂łɂ����鎞��")]
    [SerializeField]
    float _fallDownTime = 0.5f;

    [Tooltip("�U���\�ɂȂ�܂ł̎���")]
    [SerializeField]
    float _attackInterval = 3.0f;

    [Tooltip("�_�E������")]
    [SerializeField]
    float _downTime = 3.0f;

    [Tooltip("�|���ꂽ���̎���")]
    [SerializeField]
    float _knockOutTime = 6.5f;

    [Tooltip("�U�����J�n����v���C���[�Ƃ̋���")]
    [SerializeField]
    float _attackDistance = 2.0f;

    [SerializeField]
    float _bounceAttackInterval = 1.0f;

    [SerializeField]
    float _minShadowSize = 0f;

    [Header("Phase")]
    [Tooltip("�{�X��̊e�t�F�C�Y�̃p�����[�^")]
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

        //�v���C���[�ɍU�����鋗���܂ŋ߂Â��Ă��邩�̔��茋�ʂ��擾
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
    /// �{�X�̃A�j���[�V�������Đ�����
    /// </summary>
    /// <param name="type"> �w�肵���A�j���[�V���� </param>
    /// <param name="fadeTime"> �J�ڂɂ����鎞�� </param>
    public void PlayBossAnimation(BossAnimationType type, float fadeTime = 0.1f)
    {
        _anim.CrossFadeInFixedTime(type.ToString(), fadeTime);
    }

    /// <summary>
    /// �퓬�t�F�C�Y�̃R���[�`��
    /// </summary>
    /// <param name="battlePhase"> �퓬�t�F�C�Y�̎�� </param>
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
    /// �W�����v�U���̃R���[�`��
    /// </summary>
    IEnumerator JumpAttack()
    {
        _isWaiting = true;
        _isCanAttack = false;

        if (_debugMode)
        {
            _currentBattlePhase = _debugPhase;
        }

        var param = _phaseParams.FirstOrDefault(p => p.Phase == _currentBattlePhase); //���݂̃t�F�C�Y�̊e�p�����[�^�[���擾

        PlayBossAnimation(BossAnimationType.JumpUp);
        _currentMoveSpeed = param.MoveSpeed;
        Debug.Log(_currentMoveSpeed);
        transform.DOLookAt(_playerTrans.position, 0.1f, AxisConstraint.Y);

        yield return new WaitForSeconds(1.3f);

        BossStageManager.CameraChange(CameraType.JumpAttack, 1.5f);

        var playerTop = new Vector3(_playerTrans.position.x, _playerTrans.position.y + 10.0f, _playerTrans.position.z);

        _bossShadow.ChangeShadowSize(_minShadowSize, _jumpUpTime + param.ChaseTime); //�e�����X�ɏ���������

        //�v���C���[�̓���փW�����v����
        yield return transform.DOLocalMove(playerTop, _jumpUpTime)
                              .SetEase(Ease.OutCubic)
                              .WaitForCompletion(); //�{�X����яオ��

        transform.DOLookAt(_playerTrans.position, 0.1f, AxisConstraint.Y);

        float timer = 0f;

        StartCoroutine(ChangeState(BossState.Chase)); //�v���C���[��ǂ��X�e�[�^�X�ɐ؂�ւ���

        //�ǐՎ��Ԃ��o�߂���܂ŏ�����ҋ@
        while (timer < param.ChaseTime)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        yield return null;

        //�{�X�̉e�����X�ɑ傫������
        _bossShadow.ChangeShadowSize(2f, param.FallTime, Ease.InCubic);

        //�v���C���[�̌��ݒn�ɒ��n����
        yield return transform.DOMove(_playerTrans.position, param.FallTime)
                              .SetEase(Ease.InCubic)
                              .OnComplete(() =>
                              {
                                  StartCoroutine(ChangeState(BossState.Landing));
                                  BossStageManager.CameraShake();
                                  EffectManager.PlayEffect(EffectType.ShockWave, transform.position);
                                  Debug.Log("���n");
                              })
                              .WaitForCompletion();

        //�ʏ�퓬�̃J�����ɐ؂�ւ���
        BossStageManager.CameraChange(CameraType.Battle, 2f);

        //�ǌ��񐔂�1��ȏ�̏ꍇ
        if (param.BounceCount > 0)
        {
            PlayBossAnimation(BossAnimationType.Bounce);

            //�ݒ肳�ꂽ�񐔂����o�E���h���s��
            for (int i = 0; i < param.BounceCount; i++)
            {
                yield return transform.DOLocalJump(transform.position + (transform.forward * 2f), 1.5f, 1, 0.8f)
                                      .SetEase(Ease.Linear)
                                      .WaitForCompletion();

                EffectManager.PlayEffect(EffectType.ShockWave, transform.position);
            }
        }

        StartCoroutine(ChangeState(BossState.Landing));

        //�U����̃N�[���^�C��
        yield return new WaitForSeconds(param.CoolTime);

        StartCoroutine(ChangeState(BossState.Move));
        StartCoroutine(AttackInterval(_attackInterval));
        _currentMoveSpeed = _defaultMoveSpeed;
        _isWaiting = false;
    }

    /// <summary>
    /// �_���[�W���󂯂����̃R���[�`��
    /// </summary>
    /// <param name="action"> �_�E����Ԃ��I��������Ɏ��s����Action </param>
    /// <returns></returns>
    IEnumerator DamageCoroutine(Action action)
    {
        //�U�����Ƀ_���[�W���󂯂��ꍇ�͍U�������𒆒f
        if (_jumpCoroutine != null)
        {
            StopCoroutine(_jumpCoroutine);
            _jumpCoroutine = null;

            //�U�����Ɏg�p���Ă���e�l�����Z�b�g
            _currentMoveSpeed = _defaultMoveSpeed;
            _isWaiting = false;
            _isCanAttack = true;

            yield return null;
        }

        //3�t�F�C�Y�ڂ̎��͓|���ꂽ���[�V�������Đ�
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
    /// �{�X�̃X�e�[�^�X��ύX����
    /// </summary>
    /// <param name="state"> �ύX����X�e�[�^�X </param>
    /// <param name="waitTime"> �ύX��ɍs���A�N�V�����̑ҋ@���� </param>
    /// <param name="action"> �ύX��ɍs���A�N�V���� </param>
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
    /// ��e
    /// </summary>
    /// <param name="value"> �󂯂�_���[�W </param>
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
                Debug.Log("�{�X���v���C���[�փ_���[�W��^����");
            }
        }
    }
}

public enum BossState
{
    /// <summary> �������Ȃ� </summary>
    Idle,
    /// <summary> ���[�V������̑ҋ@ </summary>
    Wait,
    /// <summary> ���o�� </summary>
    Direction,
    /// <summary> �ړ� </summary>
    Move,
    /// <summary> �ǂ������� </summary>
    Chase,
    /// <summary> �W�����v�U�� </summary>
    Attack_Jump,
    /// <summary> ���n </summary>
    Landing,
    /// <summary> ��e </summary>
    Damage,
    /// <summary> �|���ꂽ </summary>
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
    [Tooltip("�ݒ肷��t�F�C�Y")]
    public BossBattlePhase Phase;
    [Tooltip("�W�����v���̈ړ����x")]
    public float MoveSpeed;
    [Tooltip("�ǐՂ��鎞��")]
    public float ChaseTime;
    [Tooltip("�����ɂ����鎞��")]
    public float FallTime;
    [Tooltip("�U����ɒ�~���鎞��")]
    public float CoolTime;
    [Tooltip("�W�����v�U����ɒ��˂��"), Range(0, 3)]
    public int BounceCount;
}