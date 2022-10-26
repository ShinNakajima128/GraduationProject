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
    public void Throw(Vector3 pos,Quaternion dir)
    {
        this.transform.position = pos;
        this.transform.rotation = dir;
        this.gameObject.SetActive(true);
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
        var forward = transform.forward;
        forward.x *= -1;
        transform.forward = forward;
    }
}
