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

    [Header("振り向ける最大値　※推奨(0 ～ 0.4)")]
    [SerializeField]
    private float _eulerMaxValue;

    [SerializeField]
    private Transform _throwPoint;

    [SerializeField]
    private BallController _ball;

    private PlayerInput _pInput;

    private ControlTarget _target = ControlTarget.None;

    // 投げ終わったか
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
        _pInput.SwitchCurrentActionMap(name);
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
        _pInput.actions["Chenge"].started += OnChenge;
        _pInput.actions["Throw"].started += OnThrow;
    }

    /// <summary>
    /// メソッドの登録解除
    /// </summary>
    private void CallBackUnRegist()
    {
        _pInput.actions["Move"].performed -= OnMove;
        _pInput.actions["Chenge"].started -= OnChenge;
        _pInput.actions["Throw"].started -= OnThrow;
    }

    /// <summary>
    /// 向きを変える
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
    /// Playerの横移動
    /// </summary>
    private void Move(Vector2 value)
    {
        var myPos = this.transform.position;

        if (value != Vector2.zero)
        {
            // 横移動
            myPos.x = myPos.x + value.x * _moveSpeed;
            // 座標の修正
            myPos = FixPosition(myPos);

            this.transform.position = myPos;
        }
    }

    /// <summary>
    /// 自身の座標の修正
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
        // 投げ終えていたら何もしない
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
