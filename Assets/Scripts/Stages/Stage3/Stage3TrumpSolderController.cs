using UnityEngine;

public class Stage3TrumpSolderController : MonoBehaviour
{
    [Header("移動速度")]
    [SerializeField]
    private float _moveSpeed;

    /// <summary>
    /// 停止しているか
    /// </summary>
    public bool IsStoped { get; set; } = false;

    private CardType _type;

    public Stage3ScoreConter Counter { get; private set; }

    private bool _isHit = false;

    /// <summary>
    /// 進行方向
    /// </summary>
    public enum MoveDirection
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
        SetStartMoveDirection(MoveDirection.Left);
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

    /// <summary>
    /// 最初の進行方向を指定
    /// </summary>
    public void SetStartMoveDirection(MoveDirection nextDirection)
    {
        _currentMoveDirection = nextDirection;
    }

    private void Move()
    {
        if (IsStoped)
        { 
            return; 
        }

        var pos = transform.position;

        switch (_currentMoveDirection)
        {
            case MoveDirection.None:
                break;
            case MoveDirection.Left:
                // 左に動く
                pos.x -= _moveSpeed * Time.deltaTime;
                break;
                // 右に動く
            case MoveDirection.Right:
                pos.x += _moveSpeed * Time.deltaTime;
                break;
            default:
                break;
        }

        transform.position = pos;
    }

    /// <summary>
    /// カウンターの参照
    /// </summary>
    public void SetCounter(Stage3ScoreConter counter)
    {
        Counter = counter;
    }

    public void Reset()
    {
        _isHit = false;
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
                    SetStartMoveDirection(MoveDirection.Right);
                    break;
                case MoveDirection.Right:
                    SetStartMoveDirection(MoveDirection.Left);
                    break;
                default:
                    break;
            }
        }
    }
}
