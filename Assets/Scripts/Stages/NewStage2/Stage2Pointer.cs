using UnityEngine;
using UnityEngine.InputSystem;

public class Stage2Pointer : MonoBehaviour
{
    [SerializeField]
    private PlayerInput _input;

    [SerializeField]
    private Stage2UIController _uiCtrl;

    private void Start()
    {
        _input.actions["PointerMove"].performed += Move;
    }

    private void Move(InputAction.CallbackContext obj)
    {
        _uiCtrl.PointerMove(obj.ReadValue<Vector2>());
    }
}
