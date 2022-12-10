using UnityEngine;

public class CardManController : MonoBehaviour
{
    [Header("à⁄ìÆë¨ìx")]
    [SerializeField]
    private float _moveSpeed;

    [SerializeField]
    private bool _isStoped;

    private CardType _type;

    public Stage3ScoreConter Counter { get; private set; }

    private bool _isHit = false;

    private enum MoveDirection
    {
        None,
        Left,
        Right
    }

    private MoveDirection _currentMoveDirection = MoveDirection.None;

    public void SetCardType(CardType type)
    {
        _type = type;
    }

    private void Start()
    {
        SetMoveDirection(MoveDirection.Left);
    }

    private void Update()
    {
        Move();
        if (_isHit)
        {
            var pos = transform.position;
            pos.y = pos.y + 0.1f;
            transform.position = pos;
        }
    }

    private void SetMoveDirection(MoveDirection nextDirection)
    {
        _currentMoveDirection = nextDirection;
    }

    private void Move()
    {
        if (_isStoped) return;

        var pos = transform.position;

        switch (_currentMoveDirection)
        {
            case MoveDirection.None:
                break;
            case MoveDirection.Left:
                // ç∂Ç…ìÆÇ≠
                pos.x = pos.x + -_moveSpeed * Time.deltaTime;
                break;
            // âEÇ…ìÆÇ≠
            case MoveDirection.Right:
                pos.x = pos.x + _moveSpeed * Time.deltaTime;
                break;
            default:
                break;
        }

        transform.position = pos;
    }

    public void SetCounter(Stage3ScoreConter counter)
    {
        Counter = counter;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            Counter.AddCount(_type);
            _isHit = true;
        }

        if (other.CompareTag("Block"))
        {
            switch (_currentMoveDirection)
            {
                case MoveDirection.None:
                    break;
                case MoveDirection.Left:
                    SetMoveDirection(MoveDirection.Right);
                    break;
                case MoveDirection.Right:
                    SetMoveDirection(MoveDirection.Left);
                    break;
                default:
                    break;
            }
        }
    }
}
