using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class Stage3PlayerController : MonoBehaviour
{
    #region Define
    /// <summary>
    /// Player�̑���Ώ�
    /// </summary>
    private enum ControlTarget
    {
        None,
        Position,
        Direction
    }
    #endregion

    #region Field
    [Header("���ړ��̑���")]
    [SerializeField]
    private float _moveSpeed;

    [Header("���ړ����ł��镝")]
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
    /// ����������
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
    /// �A�N�V�����}�b�v�̕ύX
    /// </summary>
    private void ChengeActionMap(string name)
    {
        _pInput.defaultActionMap = name;
    }

    /// <summary>
    /// ����̕ύX
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
    /// ���\�b�h�̓o�^
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
