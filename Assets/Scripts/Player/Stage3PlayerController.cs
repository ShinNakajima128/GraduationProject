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

    [Header("�U�������ő�l")]
    [Range(0f, 0.4f)]
    [SerializeField]
    private float _eulerMaxValue;

    [Header("������ꏊ")]
    [SerializeField]
    private Transform _throwPoint;

    [Header("�{�[��")]
    [SerializeField]
    private BallController _ball;

    [SerializeField]
    private Animator _animator;

    private PlayerInput _pInput;

    public bool IsDebug = false;

    public Action OnCircleButtonStarted { get; private set; }

    /// <summary>
    /// �����I�������
    /// </summary>
    public bool IsThrowed { get; private set; } = false;
    /// <summary>
    /// �������邩
    /// </summary>
    public bool CanControl { get; private set; } = false;

    /// <summary>
    /// ���͂���Ă���l
    /// </summary>
    private Vector2 InputedMoveValue { get; set; }

    private bool IsInputTurnLeft { get; set; } = false;
    private bool IsInputTurnRight { get; set; } = false;
    #endregion

    #region Unity Function
    private void Awake()
    {
        Initialize();
    }

    private void Update()
    {
        if (InputedMoveValue != Vector2.zero)
        {
            Move(InputedMoveValue);
        }

        Turn();
    }

    private void Turn()
    {
        if (IsInputTurnLeft)
        {
            var euler = Quaternion.Euler(0f, -1f, 0f);
            _ball.transform.rotation = _ball.transform.rotation * euler;

            FixRotation(_ball.transform.rotation);
        }
        else if (IsInputTurnRight)
        {

        }
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

        if (IsDebug)
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
        _pInput.actions["Move"].canceled += OnMove;
        _pInput.actions["TurnLeft"].started += OnTurnLeft;
        _pInput.actions["TurnRight"].started += OnToRight;
        _pInput.actions["Throw"].started += OnThrow;
    }

    /// <summary>
    /// ���\�b�h�̓o�^����
    /// </summary>
    private void CallBackUnRegist()
    {
        _pInput.actions["Move"].performed -= OnMove;
        _pInput.actions["TurnLeft"].started -= OnTurnLeft;
        _pInput.actions["TurnRight"].started -= OnToRight;
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
    private (Vector3, bool) FixPosition(Vector3 myPos)
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

        return (myPos, isFix);
    }

    private void Move(Vector2 value)
    {
        var myPos = this.transform.position;

        if (value != Vector2.zero)
        {
            // ���ړ�
            myPos.x = myPos.x + value.x * _moveSpeed;
            // ���W�̐��K���ƌ��ʂ��擾
            var fixedPos = FixPosition(myPos);
            // ���K���������W���擾
            myPos = fixedPos.Item1;
            // ���K�������ꍇ�͍��W���ړ����Ȃ�a
            if (!fixedPos.Item2)
            {
                // �{�[����x���W�ړ�
                _ball.SyncMovedTransorm(value.x * _moveSpeed);
            }

            this.transform.position = myPos;
        }
    }
    #endregion

    #region Public Function
    /// <summary>
    /// AnimationEvent�ɌĂ΂��
    /// </summary>
    public void Throw()
    {
        // �{�[���̍��W
        var ballPosition = _throwPoint.position;
        // �{�[���̌���
        var ballDirection = this.transform.rotation;

        var ball = _ball as IThrowable;

        ball.Throw(ballPosition, ballDirection);
        IsThrowed = true;
    }

    /// <summary>
    /// Player����̊J�n
    /// </summary>
    public void BeginControl()
    {
        CanControl = true;
    }

    /// <summary>
    /// �Z�{�^���������ꂽ���ɂ��鏈����ǉ�
    /// </summary>
    public void RegistToOnCircleButton(Action action)
    {
        OnCircleButtonStarted += action;
    }
    #endregion

    #region InputSystem CallBacks
    public void OnMove(InputAction.CallbackContext context)
    {
        Debug.Log("OnMove");

        if (!CanControl) return;

        // �����I���Ă����牽�����Ȃ�
        if (IsThrowed)
        {
            CallBackUnRegist();
            return;
        }

        var value = context.ReadValue<Vector2>();

        InputedMoveValue = value;

        Move(value);
    }

    private void OnTurnLeft(InputAction.CallbackContext context)
    {
        if (!CanControl) return;

        if (context.started)
        {
            var euler = Quaternion.Euler(0f, -1f, 0f);
            _ball.transform.rotation = _ball.transform.rotation * euler;

            FixRotation(this.transform.rotation);
        }
    }

    private void OnToRight(InputAction.CallbackContext context)
    {
        if (!CanControl) return;

        if (context.started)
        {
            var euler = Quaternion.Euler(0f, 1f, 0f);
            _ball.transform.rotation = _ball.transform.rotation * euler;

            FixRotation(this.transform.rotation);
        }
    }

    /// <summary>
    /// �Z�{�^���������ꂽ��
    /// </summary>
    private void OnThrow(InputAction.CallbackContext context)
    {
        OnCircleButtonStarted();

        if (!CanControl) return;

        if (context.started)
        {
            CanControl = false;
            _animator.Play("Swing");
        }
    }

    public void LookForForward()
    {
        var rnd = UnityEngine.Random.Range(0, 3);
        if (rnd == 2)
        {
            _animator.Play("LookForForward");
        }
    }
    #endregion
}
