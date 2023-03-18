using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBase : MonoBehaviour
{
    #region member variable
    protected AliceFaceController _fc;
    PlayerInput _input;
    IMovable _move;
    #endregion

    protected virtual void Awake()
    {
        TryGetComponent(out _input);
        TryGetComponent(out _fc);
        TryGetComponent(out _move);
    }

    private void OnEnable()
    {
        _input.actions["Move"].performed += OnMove;
        _input.actions["Move"].canceled += OnMoveStop;
    }
    private void OnDisable()
    {
        _input.actions["Move"].performed -= OnMove;
        _input.actions["Move"].canceled -= OnMoveStop;
    }
    private void OnMove(InputAction.CallbackContext obj)
    {
        var value = obj.ReadValue<Vector2>();
        _move.SetDirection(value);
    }
    private void OnMoveStop(InputAction.CallbackContext obj)
    {
        _move.SetDirection(Vector3.zero);
    }
}
