using UnityEngine;

public class BallController : MonoBehaviour, IThrowable
{
    #region Field
    [Header("ボールの転がる速さ")]
    [SerializeField]
    private float _moveSpeed;

    private bool _isMove = false;

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
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Goal")
        {
            Debug.Log("Goal");
            this.gameObject.SetActive(false);
        }

        // 法線を取得
        var normal = collision.contacts[0].normal;
        // 反射ベクトルを取得
        Vector3 result = Vector3.Reflect(_direction, normal);
        _rb.velocity = result;
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
        transform.position = pos;
        transform.rotation = dir;
        _isMove = true;
        _rb.velocity = transform.forward * _moveSpeed;
    }
    #endregion

    #region Private Function
    #endregion
}
