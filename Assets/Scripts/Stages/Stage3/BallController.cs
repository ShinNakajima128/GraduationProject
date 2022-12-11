using System;
using UnityEngine;
using UnityEngine.Networking;

public class BallController : MonoBehaviour, IThrowable
{
    #region Field
    [Header("ボールの転がる速さ")]
    [SerializeField]
    private float _moveSpeed;

    [Header("ボールの振り向き速度")]
    [SerializeField]
    private float _turnSpeed;

    [SerializeField]
    private CameraController _camera;

    [SerializeField]
    private GameObject _arrowImage;

    private bool IsThrowed { get; set; } = false;

    private event Action OnGoaled;
    private Rigidbody _rb;
    private Vector3 _direction;
    #endregion

    #region Unity Fucntion
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        _direction = _rb.velocity;
        if (IsThrowed)
        {
            ForwardRotation();
            MoveCameraRequest();
        }
    }

    /// <summary>
    /// 前転する
    /// </summary>
    private void ForwardRotation()
    {
        transform.Rotate(new Vector3(3, 0, 0));
    }

    /// <summary>
    /// カメラを動かす
    /// </summary>
    private void MoveCameraRequest()
    {
        _camera.SendBallPosition(this.transform.position);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Goal")
        {
            Debug.Log("Goal");
            OnGoaled();
            this.gameObject.SetActive(false);
            return;
        }

        if (collision.gameObject.CompareTag("Block"))
        {
            // 法線を取得
            var normal = collision.contacts[0].normal;
            // 反射ベクトルを取得
            Vector3 result = Vector3.Reflect(_direction, normal);
            _rb.velocity = result;
            return;
        }
    }
    #endregion

    #region Public Fucntion
    /// <summary>
    /// 投げる
    /// </summary>
    void IThrowable.Throw(Vector3 pos)
    {
        transform.position = pos;
        _rb.velocity = transform.forward * _moveSpeed;
        _arrowImage.gameObject.SetActive(false);
        IsThrowed = true;
    }

    /// <summary>
    /// ゴール時のアクション
    /// </summary>
    public void AddCallBack(Action action)
    {
        OnGoaled += action;
    }

    /// <summary>
    /// 左を向く
    /// </summary>
    public void TurnLeft()
    {
        transform.Rotate(0, -0.1f, 0);
        Debug.Log(transform.eulerAngles);
    }

    /// <summary>
    /// 右を向く
    /// </summary>
    public void TurnRight()
    {
        transform.Rotate(0, 0.1f, 0);
    }
    #endregion

    #region Private Function
    #endregion
}
