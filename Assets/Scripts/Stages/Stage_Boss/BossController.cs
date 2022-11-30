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

    [Tooltip("�_�E������")]
    [SerializeField]
    float _downTime = 3.0f;
    #endregion

    #region private
    CharacterController _cc;
    Transform _playerTrans = default;
    BossState _currentBossState = default;
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

    void Update()
    {
        if (_isInBattle)
        {
            OnMove();
            //if (!_isWaiting)
            //{
            //    switch (_currentBossState)
            //    {
            //        case BossState.Idle:
            //            break;
            //        case BossState.Wait:
            //            break;
            //        case BossState.Direction:
            //            break;
            //        case BossState.Move:
            //            break;
            //        case BossState.Attack_Jump:
            //            break;
            //        case BossState.Down:
            //            break;
            //        case BossState.KnockOut:
            //            break;
            //        default:
            //            break;
            //    }
            //}
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
    }

    void SetUp()
    {
        
    }
    
    public IEnumerator BattlePhaseCoroutine()
    {
        while (!_isDamaged)
        {
            yield return null;
        }

        yield return new WaitForSeconds(_downTime);
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
