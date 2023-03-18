using System;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

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
    private Vector3 _startPos;

    public Action OnCircleButtonStarted { get; private set; }

    /// <summary>
    /// �\���I����Ă��邩
    /// </summary>
    public bool IsStandbyed = false;

    /// <summary>
    /// �\���r����
    /// </summary>
    public bool IsStanding = false;

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

    private void Start()
    {
        CroquetGameManager.Instance.GameSetUp += Setup;
        _startPos = transform.localPosition;
    }

    private void FixedUpdate()
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
            RegistCallBacks();
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
    /// �\����
    /// </summary>
    private void OnStandby(InputAction.CallbackContext obj)
    {
        if (!CanControl) return;

        // �\���Ă���Œ�or�\���I����Ă��鎞�͌Ă΂�Ȃ�
        if (IsStandbyed == false && IsStanding == false)
        {
            print("�X�C���O�J�n");
            _animator.SetFloat("Speed", 1f);
            _animator.CrossFadeInFixedTime("Swing_Standby", 0.2f);
            IsStanding = true;
            InputedMoveValue = Vector2.zero;
        }
    }

    /// <summary>
    /// �A�j���[�V�����̃L�����Z��
    /// </summary>
    private void CancelTheAnimation()
    {
        Debug.Log("Cancel");
        _animator.SetFloat("Speed", -1f);
        _animator.CrossFadeInFixedTime("Swing_Standby", 0.5f);
        IsStanding = false;
    }

    /// <summary>
    /// ���\�b�h�̓o�^
    /// </summary>
    private void RegistCallBacks()
    {
        _pInput.actions["Move"].performed += OnMove;
        _pInput.actions["Move"].canceled += OnMove;

        _pInput.actions["TurnLeft"].started += OnTurnLeft;
        _pInput.actions["TurnLeft"].canceled += OnTurnLeft;

        _pInput.actions["TurnRight"].started += OnToRight;
        _pInput.actions["TurnRight"].canceled += OnToRight;

        // �������Ƃ�
        _pInput.actions["Throw"].started += OnStandby;
        // �������Ƃ�
        _pInput.actions["Throw"].canceled += OnThrow;
    }

    /// <summary>
    /// ���\�b�h�̓o�^����
    /// </summary>
    private void UnRegistCallBacks()
    {
        _pInput.actions["Move"].performed -= OnMove;
        _pInput.actions["Move"].canceled -= OnMove;

        _pInput.actions["TurnLeft"].started -= OnTurnLeft;
        _pInput.actions["TurnLeft"].canceled -= OnTurnLeft;

        _pInput.actions["TurnRight"].started -= OnToRight;
        _pInput.actions["TurnRight"].canceled -= OnToRight;

        // �������Ƃ�
        _pInput.actions["Throw"].started -= OnStandby;
        // �������Ƃ�
        _pInput.actions["Throw"].canceled -= OnThrow;
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
    /// �����m�ɂ�Stage3AnimationEvent����Ă΂��
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
        IsStandbyed = false;

        AudioManager.PlaySE(SEType.Stage3_Shot);
        VibrationController.OnVibration(Strength.High, 0.2f);
    }

    /// <summary>
    /// AnimationEvent�ɌĂ΂��
    /// �����m�ɂ�Stage3AnimationEvent����Ă΂��
    /// </summary>
    public void EndStandby()
    {
        IsStanding = false;
        IsStandbyed = true;
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
        if (!CanControl)
        {
            InputedMoveValue = Vector2.zero;
            return;
        }

        // �����I���Ă����牽�����Ȃ�
        if (IsThrowed)
        {
            UnRegistCallBacks();
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

    private void OnThrow(InputAction.CallbackContext context)
    {
        if (!CanControl)
        {
            return;
        }

        if (context.canceled)
        {
            if (!IsStandbyed)
            {
                CancelTheAnimation();
                return;
            }

            CanControl = false;
            _animator.CrossFadeInFixedTime("Swing_", 0.2f);
            AudioManager.PlaySE(SEType.Stage3_Swing);
        }

        InputedMoveValue = Vector2.zero;
        UnRegistCallBacks();
    }

    public void GoalAction(Action action)
    {
        _ball.AddCallBack(action);
    }

    public void CheckPointAction(Action action)
    {
        _ball.CheckPointCallBack(action);
    }

    void Setup()
    {
        RegistCallBacks();
        _animator.Play("Idle");
        IsThrowed = false;
        transform.localPosition = _startPos;
    }

    #endregion
}