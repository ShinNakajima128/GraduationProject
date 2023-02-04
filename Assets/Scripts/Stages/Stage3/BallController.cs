using System;
using System.Collections;
using UnityEngine;

public class BallController : MonoBehaviour, IThrowable
{
    #region Field
    [Header("ボールの転がる速さ")]
    [SerializeField]
    private float _moveSpeed;

    [Header("ボールの振り向き速度")]
    [SerializeField]
    private float _turnSpeed;

    [Header("振り向ける最大値")]
    [SerializeField]
    private float _valueOfMaxTurn;

    [SerializeField]
    private GameObject _arrowImage;

    [SerializeField]
    Transform _parent = default;

    private bool IsThrowed { get; set; } = false;

    private event Action OnGoaled;
    private event Action OnCheckPointed;
    private Rigidbody _rb;
    private Vector3 _direction;
    private Vector3 _originPos;
    private Quaternion _default;

    /// <summary>
    /// 振り向きの値(0が正面)
    /// </summary>
    private float _directionValue = 0;
    #endregion

    #region Unity Fucntion
    private void Awake()
    {
        _default = this.transform.rotation;
        _rb = GetComponent<Rigidbody>();
        _originPos = transform.localPosition;
    }

    private void Start()
    {
        CroquetGameManager.Instance.GameSetUp += Setup;
    }

    private void FixedUpdate()
    {
        _direction = _rb.velocity;

        if (IsThrowed)
        {
            ForwardRotation();
        }
    }

    /// <summary>
    /// 前転する
    /// </summary>
    private void ForwardRotation()
    {
        transform.Rotate(new Vector3(15, 0, 0));
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Block"))
        {
            // 法線を取得
            var normal = collision.contacts[0].normal;
            // 反射ベクトルを取得
            Vector3 result = Vector3.Reflect(_direction, normal);

            transform.forward = result;

            _rb.velocity = result;
            return;
        }
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.CompareTag("Checkpoint"))
        {
            if (IsThrowed)
            {
                OnCheckPointed?.Invoke();
            }
            return;
        }

        if (other.gameObject.name == "Goal")
        {
            if (IsThrowed)
            {
                Debug.Log("Goal");
                OnGoaled?.Invoke();
                StartCoroutine(DelayVanishCoroutine());
                return;
            }
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

    public void CheckPointCallBack(Action action)
    {
        OnCheckPointed += action;
    }

    /// <summary>
    /// 左を向く
    /// </summary>
    public void TurnLeft()
    {
        var value = _directionValue - 0.1f;
        // 最大値より大きければ
        if (value > -_valueOfMaxTurn)
        {
            _directionValue = value;
            transform.Rotate(0, -0.5f, 0);
        }
    }

    /// <summary>
    /// 右を向く
    /// </summary>
    public void TurnRight()
    {
        var value = _directionValue + 0.1f;
        // 最大値より小さければ
        if (value < _valueOfMaxTurn)
        {
            _directionValue = value;
            transform.Rotate(0, 0.5f, 0);
        }
    }

    void Setup()
    {
        _directionValue = 0;

        IsThrowed = false;
        transform.SetParent(_parent);
        transform.SetAsLastSibling();
        transform.localPosition = _originPos;
        transform.localRotation = default;

        gameObject.SetActive(true);
        _arrowImage.gameObject.SetActive(true);
        _rb.velocity = Vector3.zero;

        transform.rotation = _default;
    }
    #endregion

    #region Private Function
    #endregion
    IEnumerator DelayVanishCoroutine()
    {
        yield return new WaitForSeconds(3.0f);

        gameObject.SetActive(false);
    }
}
