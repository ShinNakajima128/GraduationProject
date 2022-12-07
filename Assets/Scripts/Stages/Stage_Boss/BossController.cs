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

    [Tooltip("�U�����J�n����v���C���[�Ƃ̋���")]
    [SerializeField]
    float _attackDistance = 2.0f;

    [SerializeField]
    float _bounceAttackInterval = 1.0f;

    [Header("Phase")]
    [Tooltip("�{�X��̊e�t�F�C�Y�̃p�����[�^�[")]
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

    public bool IsInvincibled => throw new NotImplementedException();
    #endregion
    #region public
    #endregion
    #region property
    #endregion

    private void Awake()
    {
        TryGetComponent(out _anim);
        TryGetComponent(out _cc);
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
        _velocity = _dir.normalized * _defaultMoveSpeed;
        _cc.Move(_velocity * Time.deltaTime);
    }

    /// <summary>
    /// �{�X�̃A�j���[�V�������Đ�����
    /// </summary>
    /// <param name="type"> �w�肵���A�j���[�V���� </param>
    /// <param name="fadeTime"> �J�ڂɂ����鎞�� </param>
    void PlayBossAnimation(BossAnimationType type, float fadeTime = 0.1f)
    {
        _anim.CrossFadeInFixedTime(type.ToString(), fadeTime);
    }
    
    /// <summary>
    /// �퓬�t�F�C�Y�̃R���[�`��
    /// </summary>
    /// <param name="battlePhase"> �퓬�t�F�C�Y�̎�� </param>
    /// <returns></returns>
    public IEnumerator BattlePhaseCoroutine(BossBattlePhase battlePhase)
    {
        _isDamaged = false;

        PlayBossAnimation(BossAnimationType.Jump);

        yield return new WaitForSeconds(1.3f);

        yield return transform.DOMove(_startBattleTrans.position, 1.0f).WaitForCompletion();
        
        BossStageManager.CameraShake();

        yield return new WaitForSeconds(0.5f);

        PlayBossAnimation(BossAnimationType.Idle);

        _currentBattlePhase = battlePhase;

        StartCoroutine(ChangeState(BossState.Move));

        while (!_isDamaged)
        {
            yield return null;
        }
        Debug.Log("�{�X����e�B�o�g���t�F�C�Y���I�����A���o���J�n");
    }

    public IEnumerator DirectionPhaseCoroutine()
    {
        yield return null;
    }

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
        transform.DOLookAt(_playerTrans.position, 0.1f, AxisConstraint.Y);

        yield return new WaitForSeconds(1.3f);

        BossStageManager.CameraChange(CameraType.JumpAttack, 1.5f);

        var playerTop = new Vector3(_playerTrans.position.x, _playerTrans.position.y + 10.0f, _playerTrans.position.z);

        yield return transform.DOLocalMove(playerTop, _jumpUpTime).WaitForCompletion(); //�{�X����яオ��

        transform.DOLookAt(_playerTrans.position, 0.1f, AxisConstraint.Y);
        
        float timer = 0f;

        StartCoroutine(ChangeState(BossState.Chase));

        //�ǐՎ��Ԃ��o�߂���܂ŏ�����ҋ@
        while (timer < param.ChaseTime)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        BossStageManager.CameraChange(CameraType.Battle, 2f);

        yield return transform.DOMoveY(0f, _fallDownTime)
                             .OnComplete(() =>
                             {
                                 StartCoroutine(ChangeState(BossState.Landing));
                                 BossStageManager.CameraShake();
                                 Debug.Log("���n");
                             })
                             .WaitForCompletion();

        if (param.AttackCount > 1)
        {
            for (int i = 0; i < param.AttackCount - 1; i++)
            {
                yield return new WaitForSeconds(0.5f);

                PlayBossAnimation(BossAnimationType.Falling, 0.2f);

                yield return new WaitForSeconds(_bounceAttackInterval);

                var bouncePosition = new Vector3(_playerTrans.position.x, _playerTrans.position.y + 2.0f, _playerTrans.position.z);

                transform.DOLookAt(_playerTrans.position, 0.1f, AxisConstraint.Y);

                Vector3[] jumpPath = { transform.position, bouncePosition, new Vector3(bouncePosition.x, 0, bouncePosition.z)};

                yield return transform.DOPath(jumpPath, _bounceAttackInterval, PathType.CubicBezier)
                                      .OnComplete(() => 
                                      {
                                          StartCoroutine(ChangeState(BossState.Landing));
                                          BossStageManager.CameraShake();
                                          Debug.Log("���n");
                                      })
                                      .WaitForCompletion();
            }     
        }

        yield return new WaitForSeconds(param.CoolTime);

        StartCoroutine(ChangeState(BossState.Move));
        StartCoroutine(AttackInterval(_attackInterval));
        _currentMoveSpeed = _defaultMoveSpeed;
        _isWaiting = false;
    }

    IEnumerator DamageCoroutine()
    {
        PlayBossAnimation(BossAnimationType.Damage);
        _isDamaged = true;

        yield return new WaitForSeconds(_downTime);
    }

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
                StartCoroutine(JumpAttack());
                break;
            case BossState.Landing:
                PlayBossAnimation(BossAnimationType.Landing);
                break;
            case BossState.Damage:
                StartCoroutine(DamageCoroutine());
                break;
            case BossState.KnockOut:
                break;
            default:
                break;
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
        StartCoroutine(ChangeState(BossState.Damage));
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
    Damage
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
    [Tooltip("�U����ɒ�~���鎞��")]
    public float CoolTime;
    [Tooltip("�U����"), Range(1, 10)]
    public int AttackCount;
}