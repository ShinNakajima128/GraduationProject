using UnityEngine;

public class BallController : MonoBehaviour, IThrowable
{
    #region Field
    [Header("É{Å[ÉãÇÃì]Ç™ÇÈë¨Ç≥")]
    [SerializeField]
    private float _moveSpeed;

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
    /// ìäÇ∞ÇÈ
    /// </summary>
    void IThrowable.Throw(Vector3 pos, Quaternion dir)
    {
        this.transform.position = pos;
        this.transform.rotation = dir;
        this.gameObject.SetActive(true);
    }
    #endregion

    #region Private Function
    /// <summary>
    /// êiÇﬁ
    /// </summary>
    private void Move()
    {
        _rb.velocity = transform.forward * _moveSpeed;
    }
    #endregion
}
