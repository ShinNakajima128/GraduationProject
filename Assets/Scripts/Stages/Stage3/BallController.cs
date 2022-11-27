using UnityEngine;

public class BallController : MonoBehaviour, IThrowable
{
    #region Field
    [Header("�{�[���̓]���鑬��")]
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

        // �@�����擾
        var normal = collision.contacts[0].normal;
        // ���˃x�N�g�����擾
        Vector3 result = Vector3.Reflect(_direction, normal);
        _rb.velocity = result;
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
        transform.position = pos;
        transform.rotation = dir;
        _isMove = true;
        _rb.velocity = transform.forward * _moveSpeed;
    }
    #endregion

    #region Private Function
    #endregion
}
