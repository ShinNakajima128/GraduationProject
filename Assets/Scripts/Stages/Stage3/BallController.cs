using UnityEngine;

public class BallController : MonoBehaviour, IThrowable
{
    #region Field
    [Header("�{�[���̓]���鑬��")]
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
    /// ���W�ړ��̓���
    /// </summary>
    public void SyncMovedTransorm(float addValue)
    {
        var pos = transform.position;
        pos.x += addValue;
        transform.position = pos;
    }

    /// <summary>
    /// ������
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
    /// �i��
    /// </summary>
    private void Move()
    {
        if (_isMove)
        _rb.velocity = transform.forward * _moveSpeed;
    }
    #endregion
}
