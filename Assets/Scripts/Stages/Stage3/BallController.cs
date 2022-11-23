using UnityEngine;

public class BallController : MonoBehaviour, IThrowable
{
    #region Field
    [Header("ボールの転がる速さ")]
    [SerializeField]
    private float _moveSpeed;

    private bool _isMove = false;

    private Rigidbody _rb;
    #endregion

    #region Unity Fucntion
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        Move();
    }

    private void OnCollisionEnter(Collision collision)
    {
        var forward = transform.forward;
        forward.x *= -1;
        transform.forward = forward;
    }
    #endregion

    #region Public Fucntion
    /// <summary>
    /// 座標移動の同期
    /// </summary>
    public void SyncMovedTransorm(float addValue)
    {
        var pos = transform.position;
        pos.x += addValue;
        transform.position = pos;
    }

    /// <summary>
    /// 投げる
    /// </summary>
    void IThrowable.Throw(Vector3 pos, Quaternion dir)
    {
        this.transform.position = pos;
        this.transform.rotation = dir;
        _isMove = true;
    }
    #endregion

    #region Private Function
    /// <summary>
    /// 進む
    /// </summary>
    private void Move()
    {
        if (_isMove)
        _rb.velocity = transform.forward * _moveSpeed;
    }
    #endregion
}
