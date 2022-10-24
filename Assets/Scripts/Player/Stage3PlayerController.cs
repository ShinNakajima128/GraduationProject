using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class Stage3PlayerController : MonoBehaviour
{
    #region Define
    /// <summary>
    /// Playerの操作対象
    /// </summary>
    private enum ControlTarget
    {
        None,
        Position,
        Direction
    }
    #endregion

    #region Field
    [Header("横移動の速さ")]
    [SerializeField]
    private float _moveSpeed;

    [Header("横移動ができる幅")]
    [SerializeField]
    private float _width;

    [SerializeField]
    private BallController _ball;

    private PlayerInput _pInput;

    private ControlTarget _target = ControlTarget.None;
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
        _target = ControlTarget.Position;

        if (_pInput)
        {
            ChengeActionMap("Stage3");
            CallBackRegist();
        }
    }

    /// <summary>
    /// アクションマップの変更
    /// </summary>
    private void ChengeActionMap(string name)
    {
        _pInput.defaultActionMap = name;
    }

    /// <summary>
    /// 操作の変更
    /// </summary>
    private void ChengeControlMode()
    {
        switch (_target)
        {
            case ControlTarget.None:
                break;
            case ControlTarget.Position:
                _target = ControlTarget.Direction;
                break;
            case ControlTarget.Direction:
                _target = ControlTarget.Position;
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// メソッドの登録
    /// </summary>
    private void CallBackRegist()
    {
        _pInput.actions["Move"].performed += OnMove;
        _pInput.actions["ModeChenge"].performed += OnModeChenge;
    }
    #endregion

    #region InputSystem CallBacks
    private void OnMove(InputAction.CallbackContext context)
    {
        var h = context.ReadValue<Vector2>();
        var t = this.transform.position;

        if (h != Vector2.zero)
        {
            t.x = t.x + h.x * _moveSpeed;
            this.transform.position = t;
        }
    }

    private void OnModeChenge(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Debug.Log("Mode Chenge !!");
            ChengeControlMode();
        }
    }
    #endregion
}
