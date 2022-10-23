using UnityEngine;

public class BallController : MonoBehaviour
{
    [Header("ボールの転がる速さ")]
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

    private void Move()
    {
        _rb.velocity = transform.forward * _moveSpeed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("SideBlock"))
        {
        }
    }
}
