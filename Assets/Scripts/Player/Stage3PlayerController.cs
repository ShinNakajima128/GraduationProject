using System;
using UnityEngine;
using UnityEngine.InputSystem;

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

    public Action OnCircleButtonStarted { get; private set; }

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
    /// 初期化処理
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
    /// アクションマップの変更
    /// </summary>
    private void ChengeActionMap(string name)
    {
        _pInput.SwitchCurrentActionMap(name);
    }

    /// <summary>
    /// メソッドの登録
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
    /// メソッドの登録解除
    /// </summary>
    private void CallBackUnRegist()
    {
        _pInput.actions["Move"].performed -= OnMove;
        _pInput.actions["TurnLeft"].started -= OnTurnLeft;
        _pInput.actions["TurnRight"].started -= OnToRight;
        _pInput.actions["Throw"].started -= OnThrow;
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
        if (!CanControl) return;

        // 投げ終えていたら何もしない
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
    /// 〇ボタンが押された時
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
