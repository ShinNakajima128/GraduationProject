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
    public static bool Submit => Instance._input.actions["Submit"].WasPressedThisFrame();
    public static bool A => Instance._input.actions["A"].WasPressedThisFrame();
    public static bool B => Instance._input.actions["B"].WasPressedThisFrame();
    public static bool X => Instance._input.actions["X"].WasPressedThisFrame();
    public static bool Y => Instance._input.actions["Y"].WasPressedThisFrame();
    public static bool RB => Instance._input.actions["RB"].WasPressedThisFrame();
    public static bool LB => Instance._input.actions["LB"].WasPressedThisFrame();
    public static bool Next => Instance._input.actions["N"].WasPressedThisFrame(); 
    public static bool LeftCrossKey => Instance._input.actions["LeftCrossKey"].WasPressedThisFrame();
    public static bool RightCrossKey => Instance._input.actions["RightCrossKey"].WasPressedThisFrame();
    public static bool Option => Instance._input.actions["Option"].WasPressedThisFrame();
    public static bool Exit => Instance._input.actions["Exit"].WasPressedThisFrame();
    #endregion

    private void Awake()
    {
        Instance = this;
        TryGetComponent(out _input);
    }
}
