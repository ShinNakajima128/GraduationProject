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
    [Tooltip("移動速度")]
    [SerializeField]
    float _moveSpeed = 5.0f;

    [Tooltip("旋回速度")]
    [SerializeField]
    float _turnSpeed = 5.0f;

    [Tooltip("ダウン時間")]
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
    /// <summary> 何もしない </summary>
    Idle,
    /// <summary> モーション後の待機 </summary>
    Wait,
    /// <summary> 演出中 </summary>
    Direction,
    /// <summary> 移動 </summary>
    Move,
    /// <summary> ジャンプ攻撃 </summary>
    Attack_Jump,
    /// <summary> ダウン(戦闘は続く) </summary>
    Down,
    /// <summary> 倒された </summary>
    KnockOut
}
