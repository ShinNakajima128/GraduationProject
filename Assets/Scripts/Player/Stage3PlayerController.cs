using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class Stage3PlayerController : MonoBehaviour
{
    #region Field
    [Header("���ړ��̑���")]
    [SerializeField]
    private float _moveSpeed;

    [Header("���ړ����ł��镝")]
    [SerializeField]
    private float _width;

    [Header("�U�������ő�l�@������(0 �` 0.4)")]
    [SerializeField]
    private float _eulerMaxValue;

    [SerializeField]
    private Transform _throwPoint;

    [SerializeField]
    private BallController _ball;

    private PlayerInput _pInput;

    private ControlTarget _target = ControlTarget.None;

    // �����I�������
    public bool IsThrowed { get; private set; } = false;
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
        _pInput.SwitchCurrentActionMap(name);
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
        _pInput.actions["Chenge"].started += OnChenge;
        _pInput.actions["Throw"].started += OnThrow;
    }

    /// <summary>
    /// ���\�b�h�̓o�^����
    /// </summary>
    private void CallBackUnRegist()
    {
        _pInput.actions["Move"].performed -= OnMove;
        _pInput.actions["Chenge"].started -= OnChenge;
        _pInput.actions["Throw"].started -= OnThrow;
    }

    /// <summary>
    /// ������ς���
    /// </summary>
    private void ChengeDirection(Vector2 value)
    {
        var euler = Quaternion.identity;

        if (value.x == -1)
        {
            euler = Quaternion.Euler(0f, -1f, 0f);
        }
        else if (value.x == 1)
        {
            euler = Quaternion.Euler(0f, 1f, 0f);
        }

        this.transform.rotation = this.transform.rotation * euler;

        FixRotation(this.transform.rotation);
    }

    /// <summary>
    /// ���K��
    /// </summary>
    private void FixRotation(Quaternion rotation)
    {
        if (rotation.y > _eulerMaxValue)
        {
            rotation.y = _eulerMaxValue;
        }
        else if (rotation.y < -_eulerMaxValue)
        {
            rotation.y = -_eulerMaxValue;
        }

        this.transform.rotation = rotation;
    }

    /// <summary>
    /// Player�̉��ړ�
    /// </summary>
    private void Move(Vector2 value)
    {
        var myPos = this.transform.position;

        if (value != Vector2.zero)
        {
            // ���ړ�
            myPos.x = myPos.x + value.x * _moveSpeed;
            // ���W�̏C��
            myPos = FixPosition(myPos);

            this.transform.position = myPos;
        }
    }

    /// <summary>
    /// ���g�̍��W�̏C��
    /// </summary>
    private Vector3 FixPosition(Vector3 myPos)
    {
        if (myPos.x > _width)
        {
            myPos.x = _width;
        }
        else if (myPos.x < -_width)
        {
            myPos.x = -_width;
        }

        return myPos;
    }
    #endregion

    #region InputSystem CallBacks
    public void OnMove(InputAction.CallbackContext context)
    {
        // �����I���Ă����牽�����Ȃ�
        if (IsThrowed) 
        {
            CallBackUnRegist();
            return;
        }

        var h = context.ReadValue<Vector2>();

        switch (_target)
        {
            case ControlTarget.None:
                break;
            case ControlTarget.Position:
                Move(h);
                break;
            case ControlTarget.Direction:
                ChengeDirection(h);
                break;
            default:
                break;
        }

    }

    public void OnChenge(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            ChengeControlMode();
        }
    }

    private void OnThrow(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            var ballPosition = _throwPoint.position;
            var ballDirection = this.transform.rotation;

            _ball.Throw(ballPosition, ballDirection);
            IsThrowed = true;
        }
    }
    #endregion
}
