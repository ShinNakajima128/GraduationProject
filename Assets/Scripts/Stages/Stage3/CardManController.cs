using UnityEngine;

public class CardManController : MonoBehaviour
{
    [Header("à⁄ìÆë¨ìx")]
    [SerializeField]
    private float _moveSpeed;

    [Header("â~â^ìÆÇÃç€ÇÃîºåa")]
    [SerializeField]
    private float _radius;

    [SerializeField]
    private bool _isStoped;

    [SerializeField]
    private CardType _type;

    [SerializeField]
    private Stage3ScoreConter _conter;

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
        if (_isStoped) return;
        var x = _radius * Mathf.Sin(Time.time * _moveSpeed);
        x = x + _defPos.x;
        this.transform.position = new Vector3(x, _defPos.y, _defPos.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            _conter.AddCount(_type);
            gameObject.SetActive(false);
        }
    }
}
