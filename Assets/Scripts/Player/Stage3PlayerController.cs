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

    [Header("������ꏊ")]
    [SerializeField]
    private Transform _throwPoint;

    [Header("�{�[��")]
    [SerializeField]
    private BallController _ball;

    private PlayerInput _pInput;

    public bool Debug = false;

    // �����I�������
    public bool IsThrowed { get; private set; } = false;
    // �������邩
    public bool CanControl { get; private set; } = false;
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

        if (_pInput)
        {
            ChengeActionMap("Stage3");
            CallBackRegist();
        }

        if (Debug)
        {
            CanControl = true;
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
    /// ���\�b�h�̓o�^
    /// </summary>
    private void CallBackRegist()
    {
        _pInput.actions["Move"].performed += OnMove;
        _pInput.actions["ToLeft"].started += OnToLeft;
        _pInput.actions["ToRight"].started += OnToRight;
        _pInput.actions["Throw"].started += OnThrow;
    }

    /// <summary>
    /// ���\�b�h�̓o�^����
    /// </summary>
    private void CallBackUnRegist()
    {
        _pInput.actions["Move"].performed -= OnMove;
        _pInput.actions["ToLeft"].started -= OnToLeft;
        _pInput.actions["ToRight"].started -= OnToRight;
        _pInput.actions["Throw"].started -= OnThrow;
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
    /// ���g�̍��W�̏C��
    /// </summary>
    private (Vector3,bool) FixPosition(Vector3 myPos)
    {
        var isFix = false;
        if (myPos.x > _width)
        {
            myPos.x = _width;
            isFix = true;
        }
        else if (myPos.x < -_width)
        {
            myPos.x = -_width;
            isFix = true;
        }

        return (myPos,isFix);
    }
    #endregion

    #region Public Function
    public void Send()
    {
        CanControl = true;
    }
    #endregion

    #region InputSystem CallBacks
    public void OnMove(InputAction.CallbackContext context)
    {
        if (!CanControl) return;

        // �����I���Ă����牽�����Ȃ�
        if (IsThrowed)
        {
            CallBackUnRegist();
            return;
        }

        var velue = context.ReadValue<Vector2>();
        var myPos = this.transform.position;

        if (velue != Vector2.zero)
        {
            // ���ړ�
            myPos.x = myPos.x + velue.x * _moveSpeed;
            // ���W�̐��K���ƌ��ʂ��擾
            var fixedPos = FixPosition(myPos);
            // ���K���������W���擾
            myPos = fixedPos.Item1;
            // ���K�������ꍇ�͍��W���ړ����Ȃ�
            if (!fixedPos.Item2)
            {
                // �{�[����x���W�ړ�
                _ball.SyncMovedTransorm(velue.x * _moveSpeed);
            }

            this.transform.position = myPos;
        }
    }

    private void OnToLeft(InputAction.CallbackContext context)
    {
        if (!CanControl) return;

        if (context.started)
        {
            var euler = Quaternion.Euler(0f, -1f, 0f);
            this.transform.rotation = this.transform.rotation * euler;

            FixRotation(this.transform.rotation);
        }
    }

    private void OnToRight(InputAction.CallbackContext context)
    {
        if (!CanControl) return;

        if (context.started)
        {
            var euler = Quaternion.Euler(0f, 1f, 0f);
            this.transform.rotation = this.transform.rotation * euler;

            FixRotation(this.transform.rotation);
        }
    }

    private void OnThrow(InputAction.CallbackContext context)
    {
        if (!CanControl) return;

        if (context.started)
        {
            var ballPosition = _throwPoint.position;
            var ballDirection = this.transform.rotation;

            var ball = _ball as IThrowable;

            ball.Throw(ballPosition, ballDirection);
            IsThrowed = true;
        }
    }
    #endregion
}
