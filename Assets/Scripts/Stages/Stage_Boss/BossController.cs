using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(CharacterController))]
public class BossController : MonoBehaviour
{
    #region serialize
    [Header("Variables")]
    [Tooltip("�ړ����x")]
    [SerializeField]
    float _moveSpeed = 5.0f;

    [Tooltip("���񑬓x")]
    [SerializeField]
    float _turnSpeed = 5.0f;

    [Tooltip("�W�����v���ď�܂ōs���̂ɂ����鎞��")]
    [SerializeField]
    float _jumpUpTime = 1.5f;

    [Tooltip("���n����܂łɂ����鎞��")]
    [SerializeField]
    float _fallDownTime = 0.3f;

    [Tooltip("�U����A��~���Ă��鎞��")]
    [SerializeField]
    float _stopTime = 3.0f;

    [Tooltip("�U���\�ɂȂ�܂ł̎���")]
    [SerializeField]
    float _attackInterval = 3.0f;

    [Tooltip("�_�E������")]
    [SerializeField]
    float _downTime = 3.0f;

    [Tooltip("�U�����J�n����v���C���[�Ƃ̋���")]
    [SerializeField]
    float _attackDistance = 2.0f;

    [Header("Positions")]
    [SerializeField]
    Transform _directionTrans = default;

    [SerializeField]
    Transform _startBattleTrans = default;
    #endregion

    #region private
    Animator _anim;
    CharacterController _cc;
    Transform _playerTrans = default;
    BossState _currentBossState = default;
    BossBattlePhase _currentBattlePhase = default;
    Vector3 _dir;
    Vector3 _velocity;
    bool _isInBattle = false;
    bool _isWaiting = false;
    bool _isDamaged = false;
    bool _isCanAttack = true;
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
        StageGame<BossStageManager>.Instance.CharacterMovable += BossMovable;
    }

    private void Update()
    {
        if (_isInBattle)
        {
            if (!_isWaiting)
            {
                
            }
        }
    }

    void FixedUpdate()
    {
        if (_currentBossState == BossState.Move)
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

        Quaternion targetRotation = Quaternion.LookRotation(_dir.normalized);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _turnSpeed * Time.deltaTime);
        _velocity = _dir.normalized * _moveSpeed;
        _cc.Move(_velocity * Time.deltaTime);

        if (Vector3.Distance(transform.position, _playerTrans.position) < _attackDistance && _isCanAttack)
        {
            StartCoroutine(ChangeState(BossState.Attack_Jump));
        }
    }
    
    public IEnumerator BattlePhaseCoroutine(BossBattlePhase battlePhase)
    {
        _anim.CrossFadeInFixedTime("Jump", 0.1f);

        yield return new WaitForSeconds(1.3f);

        yield return transform.DOMove(_startBattleTrans.position, 1.0f).WaitForCompletion();

        yield return new WaitForSeconds(1.0f);

        _anim.CrossFadeInFixedTime("Idle", 0.1f);

        _currentBattlePhase = battlePhase;

        StartCoroutine(ChangeState(BossState.Move));

        while (!_isDamaged)
        {
            yield return null;
        }

        yield return new WaitForSeconds(_downTime);
    }

    public IEnumerator DirectionPhaseCoroutine()
    {
        yield return null;
    }

    IEnumerator JumpAttackCoroutine()
    {
        _isWaiting = true;
        _isCanAttack = false;

        bool attackFinished = false;
        
        _anim.CrossFadeInFixedTime("Jump", 0.1f);
        transform.DOLookAt(_playerTrans.position, 0.1f);

        yield return new WaitForSeconds(1.3f);

        transform.DOLookAt(_playerTrans.position, 0.1f,AxisConstraint.Y);

        switch (_currentBattlePhase)
        {
            case BossBattlePhase.First:
                yield return transform.DOMove(_playerTrans.position, _fallDownTime)
                                      .OnComplete(() =>
                                      {
                                          attackFinished = true;
                                          Debug.Log("���n");
                                      })
                                      .WaitForCompletion();    
                break;
            case BossBattlePhase.Second:
                yield return transform.DOMove(_playerTrans.position, _fallDownTime)
                                      .OnComplete(() =>
                                      {
                                          attackFinished = true;
                                          Debug.Log("���n");
                                      })
                                      .WaitForCompletion();
                //transform.DOMove(_playerTrans.position, _fallDownTime)
                //         .OnComplete(() =>
                //         {
                //             attackFinished = true;
                //             Debug.Log("���n");
                //         })
                //         .SetDelay(3.5f);
                break;
            case BossBattlePhase.Third:
                //transform.DOMove(_playerTrans.position, _fallDownTime)
                //         .OnComplete(() =>
                //         {
                //             transform.DOMoveY(3.0f, 1.0f)
                //                      .OnComplete(() => 
                //                      {
                //                          transform.DOMove(_playerTrans.position, _fallDownTime)
                //                                   .SetDelay(5);
                //                      })
                //                      .SetLoops(2);
                //         })
                //         .SetDelay(3.5f);
                break;
            default:
                break;
        }

        while (!attackFinished)
        {
            yield return null;
        }

        yield return new WaitForSeconds(_stopTime);

        StartCoroutine(ChangeState(BossState.Move));
        StartCoroutine(AttackInterval(_attackInterval));
        _isWaiting = false;
    }

    IEnumerator ChangeState(BossState state, float waitTime = 0.02f, Action action = null)
    {
        _currentBossState = state;

        switch (_currentBossState)
        {
            case BossState.Idle:
                break;
            case BossState.Wait:
                break;
            case BossState.Direction:
                break;
            case BossState.Move:
                _anim.CrossFadeInFixedTime("Move", 0.1f);
                break;
            case BossState.Attack_Jump:
                StartCoroutine(JumpAttackCoroutine());
                break;
            case BossState.Down:
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
    /// <summary> �W�����v�U�� </summary>
    Attack_Jump,
    /// <summary> �_�E��(�퓬�͑���) </summary>
    Down,
    /// <summary> �|���ꂽ </summary>
    KnockOut
}

public enum BossBattlePhase
{
    First,
    Second,
    Third
}
