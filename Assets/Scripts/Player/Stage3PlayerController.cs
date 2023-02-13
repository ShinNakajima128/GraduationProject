using System;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

[RequireComponent(typeof(PlayerInput))]
public class Stage3PlayerController : MonoBehaviour
{
    #region Field
    [Header("横移動の速さ")]
    [SerializeField]
    private float _moveSpeed;

    [Header("横移動ができる幅")]
    [SerializeField]
    private float _width;

    [Header("投げる場所")]
    [SerializeField]
    private Transform _throwPoint;

    [Header("ボール")]
    [SerializeField]
    private BallController _ball;

    [SerializeField]
    private Animator _animator;

    private PlayerInput _pInput;
    private Vector3 _startPos;

    public Action OnCircleButtonStarted { get; private set; }

    /// <summary>
    /// 構え終わっているか
    /// </summary>
    public bool IsStandbyed = false;

    /// <summary>
    /// 構え途中か
    /// </summary>
    public bool IsStanding = false;

    /// <summary>
    /// 投げ終わったか
    /// </summary>
    public bool IsThrowed { get; private set; } = false;

    /// <summary>
    /// 投げられるか
    /// </summary>
    public bool CanControl { get; private set; } = false;

    /// <summary>
    /// 入力されている値
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
    /// 初期化処理
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
    /// アクションマップの変更
    /// </summary>
    private void ChengeActionMap(string name)
    {
        _pInput.SwitchCurrentActionMap(name);
    }

    /// <summary>
    /// 構える
    /// </summary>
    private void OnStandby(InputAction.CallbackContext obj)
    {
        if (!CanControl) return;

        // 構えている最中or構え終わっている時は呼ばれない
        if (IsStandbyed == false && IsStanding == false)
        {
            print("スイング開始");
            _animator.SetFloat("Speed", 1f);
            _animator.CrossFadeInFixedTime("Swing_Standby", 0.2f);
            IsStanding = true;
            InputedMoveValue = Vector2.zero;
        }
    }

    /// <summary>
    /// アニメーションのキャンセル
    /// </summary>
    private void CancelTheAnimation()
    {
        Debug.Log("Cancel");
        _animator.SetFloat("Speed", -1f);
        _animator.CrossFadeInFixedTime("Swing_Standby", 0.5f);
        IsStanding = false;
    }

    /// <summary>
    /// メソッドの登録
    /// </summary>
    private void RegistCallBacks()
    {
        _pInput.actions["Move"].performed += OnMove;
        _pInput.actions["Move"].canceled += OnMove;

        _pInput.actions["TurnLeft"].started += OnTurnLeft;
        _pInput.actions["TurnLeft"].canceled += OnTurnLeft;

        _pInput.actions["TurnRight"].started += OnToRight;
        _pInput.actions["TurnRight"].canceled += OnToRight;

        // 押したとき
        _pInput.actions["Throw"].started += OnStandby;
        // 離したとき
        _pInput.actions["Throw"].canceled += OnThrow;
    }

    /// <summary>
    /// メソッドの登録解除
    /// </summary>
    private void UnRegistCallBacks()
    {
        _pInput.actions["Move"].performed -= OnMove;
        _pInput.actions["Move"].canceled -= OnMove;

        _pInput.actions["TurnLeft"].started -= OnTurnLeft;
        _pInput.actions["TurnLeft"].canceled -= OnTurnLeft;

        _pInput.actions["TurnRight"].started -= OnToRight;
        _pInput.actions["TurnRight"].canceled -= OnToRight;

        // 押したとき
        _pInput.actions["Throw"].started -= OnStandby;
        // 離したとき
        _pInput.actions["Throw"].canceled -= OnThrow;
    }

    /// <summary>
    /// 自身の座標の修正
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
            // 横移動
            myPos.x = myPos.x + value.x * _moveSpeed * Time.deltaTime;
            // 座標の正規化と結果を取得
            var fixedPos = FixPosition(myPos);
            // 正規化した座標を取得
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
    /// AnimationEventに呼ばれる
    /// ※正確にはStage3AnimationEventから呼ばれる
    /// </summary>
    public void Throw()
    {
        // ボールの座標
        var ballPosition = _throwPoint.position;

        // 親子関係の解消
        _ball.gameObject.transform.parent = null;

        var ball = _ball as IThrowable;

        ball.Throw(ballPosition);
        IsThrowed = true;
        IsStandbyed = false;

        AudioManager.PlaySE(SEType.Stage3_Shot);
        VibrationController.OnVibration(Strength.High, 0.2f);
    }

    /// <summary>
    /// AnimationEventに呼ばれる
    /// ※正確にはStage3AnimationEventから呼ばれる
    /// </summary>
    public void EndStandby()
    {
        IsStanding = false;
        IsStandbyed = true;
    }

    /// <summary>
    /// Player操作の開始
    /// </summary>
    public void BeginControl()
    {
        CanControl = true;
    }

    /// <summary>
    /// 〇ボタンが押された時にする処理を追加
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

        // 投げ終えていたら何もしない
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