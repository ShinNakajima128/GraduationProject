using UnityEngine;

public class Stage3TrumpSolderController : MonoBehaviour
{
    [Header("�ړ����x")]
    [SerializeField]
    private float _moveSpeed;

    /// <summary>
    /// ��~���Ă��邩
    /// </summary>
    public bool IsStoped { get; set; } = false;

    private CardType _type;

    public Stage3ScoreConter Counter { get; private set; }

    private bool _isHit = false;

    /// <summary>
    /// �i�s����
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
    /// �ŏ��̐i�s�������w��
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
                // ���ɓ���
                pos.x -= _moveSpeed * Time.deltaTime;
                break;
                // �E�ɓ���
            case MoveDirection.Right:
                pos.x += _moveSpeed * Time.deltaTime;
                break;
            default:
                break;
        }

        transform.position = pos;
    }

    /// <summary>
    /// �J�E���^�[�̎Q��
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
