using UnityEngine;

public class BallController : MonoBehaviour
{
    [Header("É{Å[ÉãÇÃì]Ç™ÇÈë¨Ç≥")]
    [SerializeField]
    private float _moveSpeed;

    private Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        Move();
    }

    /// <summary>
    /// ìäÇ∞ÇÈ
    /// </summary>
    public void Throw(Vector3 dir)
    {

    }

    /// <summary>
    /// êiÇﬁ
    /// </summary>
    private void Move()
    {
        _rb.velocity = transform.forward * _moveSpeed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        var normal = transform.forward;
        normal.x *= -1;
        transform.forward = normal;
    }
}
