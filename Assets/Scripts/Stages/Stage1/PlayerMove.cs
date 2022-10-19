using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// ステージ1のプレイヤーの移動機能を管理する
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class PlayerMove : MonoBehaviour, IMovable
{
    #region serialize
    [SerializeField]
    float _moveSpeed = 5.0f;
    #endregion

    #region private
    CharacterController _cc;
    Vector3 _dir;
    Vector3 _moveDir;
    #endregion

    private void Awake()
    {
        TryGetComponent(out _cc);
    }

    private void Update()
    {
        _moveDir = _dir * _moveSpeed;
        _cc.Move(_moveDir * Time.deltaTime);
    }

    public void SetDirection(Vector3 dir)
    {
        _dir = dir;
    }
}
