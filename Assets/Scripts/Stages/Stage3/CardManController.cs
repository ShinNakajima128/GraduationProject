using UnityEngine;

public class CardManController : MonoBehaviour
{
    [SerializeField]
    private float _moveSpeed;

    [SerializeField]
    private float _radius;

    private Vector3 _defPos;

    private void Awake()
    {
        _defPos = this.transform.position;
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        var x = _radius * Mathf.Sin(Time.time * _moveSpeed);
        x = x + _defPos.x;
        this.transform.position = new Vector3(x, _defPos.y, _defPos.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            this.gameObject.SetActive(false);
        }
    }
}
