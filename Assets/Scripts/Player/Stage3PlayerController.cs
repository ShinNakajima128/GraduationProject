using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class Stage3PlayerController : MonoBehaviour
{
    #region Field
    [SerializeField]
    private float _moveSpeed;

    private PlayerInput _pInput;
    #endregion

    #region Unity Function
    private void Awake()
    {
        Initialize();
    }
    #endregion

    #region Private Fucntion
    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Initialize()
    {
        _pInput = GetComponent<PlayerInput>();

        if (_pInput)
        {
            // アクションマップの変更
            _pInput.defaultActionMap = "Stage3";
            // コールバックの登録
            _pInput.actions["Move"].performed += OnMove;
        }
    }
    #endregion

    #region InputSystem CallBacks
    private void OnMove(InputAction.CallbackContext context)
    {
        var h = context.ReadValue<Vector2>();
        var t = this.transform.position;

        if (h != Vector2.zero)
        {
            t.x = t.x + h.x * 0.1f;
            this.transform.position = t;
        }
    }
    #endregion
}
