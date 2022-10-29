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
    public static bool Submit => Instance._input.actions["Submit"].IsPressed();
    #endregion

    private void Awake()
    {
        Instance = this;
        TryGetComponent(out _input);
    }
}
