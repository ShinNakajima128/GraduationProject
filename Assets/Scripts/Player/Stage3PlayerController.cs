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

    [Header("������ꏊ")]
    [SerializeField]
    private Transform _throwPoint;

    [Header("�{�[��")]
    [SerializeField]
    private BallController _ball;

    [SerializeField]
    private Animator _animator;

    private PlayerInput _pInput;

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
        if (!CanControl) return;

        if (IsInputTurnLeft)
        {
            _ball.TurnLeft();
        }
        else if (IsInputTurnRight)
        {
            _ball.TurnRight();
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
        _pInput.actions["TurnLeft"].canceled += OnTurnLeft;

        _pInput.actions["TurnRight"].started += OnToRight;
        _pInput.actions["TurnRight"].canceled += OnToRight;

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
            myPos.x = myPos.x + value.x * _moveSpeed * Time.deltaTime;
            // ���W�̐��K���ƌ��ʂ��擾
            var fixedPos = FixPosition(myPos);
            // ���K���������W���擾
            myPos = fixedPos.Item1;

            this.transform.position = myPos;
        }
    }
    #endregion

    #region Public Function
    public void LookForForward()
    {
        var rnd = UnityEngine.Random.Range(0, 3);
        if (rnd == 2)
        {
            _animator.Play("LookForForward");
        }
    }

    /// <summary>
    /// AnimationEvent�ɌĂ΂��
    /// </summary>
    public void Throw()
    {
        // �{�[���̍��W
        var ballPosition = _throwPoint.position;

        // �e�q�֌W�̉���
        _ball.gameObject.transform.parent = null;

        var ball = _ball as IThrowable;

        ball.Throw(ballPosition);
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
        if (!CanControl) return;

        // �����I���Ă����牽�����Ȃ�
        if (IsThrowed)
        {
            CallBackUnRegist();
            return;
        }

        var value = context.ReadValue<Vector2>();

        InputedMoveValue = value;
    }

    private void OnTurnLeft(InputAction.CallbackContext context)
    {
        if (!CanControl) return;

        if (context.started)
        {
            IsInputTurnLeft = true;
        }
        else if (context.canceled)
        {
            IsInputTurnLeft = false;
        }
    }

    private void OnToRight(InputAction.CallbackContext context)
    {
        if (!CanControl) return;

        if (context.started)
        {
            IsInputTurnRight = true;
        }
        else if (context.canceled)
        {
            IsInputTurnRight = false;
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

    
    #endregion
}
