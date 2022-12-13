using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIInput : MonoBehaviour
{
    #region private
    PlayerInput _input;
    #endregion

    #region property
    public static UIInput Instance { get; private set; }
    //public static bool Submit => Instance._input.actions["Submit"].IsPressed();
    public static bool Submit => Instance._input.actions["Submit"].WasPressedThisFrame();
    public static bool A => Instance._input.actions["A"].IsPressed();
    public static bool B => Instance._input.actions["B"].IsPressed();
    public static bool X => Instance._input.actions["X"].IsPressed();
    public static bool Y => Instance._input.actions["Y"].IsPressed();
    #endregion

    private void Awake()
    {
        Instance = this;
        TryGetComponent(out _input);
    }
}
public enum GamepadButtonType
{
    A,
    B,
    X,
    Y
}
