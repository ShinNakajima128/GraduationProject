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
    CharacterController _cc;
    Transform _playerTrans = default;
    BossState _currentBossState = default;
    BossBattlePhase _currentBattlePhase = default;
    Vector3 _dir;
    Vector3 _velocity;
    bool _isInBattle = false;
    bool _isWaiting = false;
    bool _isDamaged = false;
    #endregion
    #region public
    #endregion
    #region property
    #endregion

    private void Awake()
    {
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
                switch (_currentBossState)
                {
                    case BossState.Idle:
                        break;
                    case BossState.Wait:
                        break;
                    case BossState.Direction:
                        break;
                    case BossState.Move:
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
        _dir = _playerTrans.position - transform.position;
        _dir.y = 0;

        Quaternion targetRotation = Quaternion.LookRotation(_dir.normalized);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _turnSpeed * Time.deltaTime);
        _velocity = _dir.normalized * _moveSpeed;
        _cc.Move(_velocity * Time.deltaTime);

        if (Vector3.Distance(transform.position, _playerTrans.position) < _attackDistance)
        {
            StartCoroutine(ChangeState(BossState.Attack_Jump, 5.0f, () => 
            {
                StartCoroutine(ChangeState(BossState.Move));
            }));
        }
    }
    
    public IEnumerator BattlePhaseCoroutine(BossBattlePhase battlePhase)
    {
        transform.DOMove(_startBattleTrans.position, 2.0f);

        yield return new WaitForSeconds(3.0f);

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

        bool attackFinished = false;

        transform.DOMoveY(10.0f, _jumpUpTime);

        yield return new WaitForSeconds(_jumpUpTime);

        switch (_currentBattlePhase)
        {
            case BossBattlePhase.First:
                transform.DOMove(_playerTrans.position, _fallDownTime)
                         .OnComplete(() =>
                         {
                             attackFinished = true;
                             Debug.Log("���n");
                         })
                         .SetDelay(5);    
                break;
            case BossBattlePhase.Second:
                transform.DOMove(_playerTrans.position, _fallDownTime)
                         .OnComplete(() =>
                         {
                             attackFinished = true;
                             Debug.Log("���n");
                         })
                         .SetDelay(3.5f);
                break;
            case BossBattlePhase.Third:
                transform.DOMove(_playerTrans.position, _fallDownTime)
                         .OnComplete(() =>
                         {
                             transform.DOMoveY(3.0f, 1.0f)
                                      .OnComplete(() => 
                                      {
                                          transform.DOMove(_playerTrans.position, _fallDownTime)
                                                   .SetDelay(5);
                                      })
                                      .SetLoops(2);
                         })
                         .SetDelay(3.5f);
                break;
            default:
                break;
        }

        while (!attackFinished)
        {
            yield return null;
        }

        yield return new WaitForSeconds(3.0f);

        StartCoroutine(ChangeState(BossState.Move));
        _isWaiting = false;
    }

    IEnumerator ChangeState(BossState state, float waitTime = 0.02f, Action action = null)
    {
        _currentBossState = state;

        yield return new WaitForSeconds(waitTime);

        action?.Invoke();
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
