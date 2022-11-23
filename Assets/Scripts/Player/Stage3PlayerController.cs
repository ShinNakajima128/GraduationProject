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

    [Header("振り向ける最大値　※推奨(0 ～ 0.4)")]
    [SerializeField]
    private float _eulerMaxValue;

    [Header("投げる場所")]
    [SerializeField]
    private Transform _throwPoint;

    [Header("ボール")]
    [SerializeField]
    private BallController _ball;

    private PlayerInput _pInput;

    public bool Debug = false;

    // 投げ終わったか
    public bool IsThrowed { get; private set; } = false;
    // 投げられるか
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

        if (Debug)
        {
            CanControl = true;
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
        _pInput.actions["ToLeft"].started += OnToLeft;
        _pInput.actions["ToRight"].started += OnToRight;
        _pInput.actions["Throw"].started += OnThrow;
    }

    /// <summary>
    /// メソッドの登録解除
    /// </summary>
    private void CallBackUnRegist()
    {
        _pInput.actions["Move"].performed -= OnMove;
        _pInput.actions["ToLeft"].started -= OnToLeft;
        _pInput.actions["ToRight"].started -= OnToRight;
        _pInput.actions["Throw"].started -= OnThrow;
    }

    /// <summary>
    /// 正規化
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
    /// 自身の座標の修正
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

        // 投げ終えていたら何もしない
        if (IsThrowed)
        {
            CallBackUnRegist();
            return;
        }

        var velue = context.ReadValue<Vector2>();
        var myPos = this.transform.position;

        if (velue != Vector2.zero)
        {
            // 横移動
            myPos.x = myPos.x + velue.x * _moveSpeed;
            // 座標の正規化と結果を取得
            var fixedPos = FixPosition(myPos);
            // 正規化した座標を取得
            myPos = fixedPos.Item1;
            // 正規化した場合は座標を移動しない
            if (!fixedPos.Item2)
            {
                // ボールのx座標移動
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
