using System;
using System.Collections;
using UnityEngine;

public class BallController : MonoBehaviour, IThrowable
{
    #region Field
    [Header("�{�[���̓]���鑬��")]
    [SerializeField]
    private float _moveSpeed;

    [Header("�{�[���̐U��������x")]
    [SerializeField]
    private float _turnSpeed;

    [Header("�U�������ő�l")]
    [SerializeField]
    private float _valueOfMaxTurn;

    [SerializeField]
    private GameObject _arrowImage;

    [SerializeField]
    Transform _parent = default;

    private bool IsThrowed { get; set; } = false;

    private event Action OnGoaled;
    private event Action OnCheckPointed;
    private Rigidbody _rb;
    private Vector3 _direction;
    private Vector3 _originPos;
    private Quaternion _default;

    /// <summary>
    /// �U������̒l(0������)
    /// </summary>
    private float _directionValue = 0;
    #endregion

    #region Unity Fucntion
    private void Awake()
    {
        _default = this.transform.rotation;
        _rb = GetComponent<Rigidbody>();
        _originPos = transform.localPosition;
    }

    private void Start()
    {
        CroquetGameManager.Instance.GameSetUp += Setup;
    }

    private void FixedUpdate()
    {
        _direction = _rb.velocity;

        if (IsThrowed)
        {
            ForwardRotation();
        }
    }

    /// <summary>
    /// �O�]����
    /// </summary>
    private void ForwardRotation()
    {
        transform.Rotate(new Vector3(15, 0, 0));
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Block"))
        {
            // �@�����擾
            var normal = collision.contacts[0].normal;
            // ���˃x�N�g�����擾
            Vector3 result = Vector3.Reflect(_direction, normal);

            transform.forward = result;

            _rb.velocity = result;
            return;
        }
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.CompareTag("Checkpoint"))
        {
            if (IsThrowed)
            {
                OnCheckPointed?.Invoke();
            }
            return;
        }

        if (other.gameObject.name == "Goal")
        {
            if (IsThrowed)
            {
                Debug.Log("Goal");
                OnGoaled?.Invoke();
                StartCoroutine(DelayVanishCoroutine());
                return;
            }
        }


    }
    #endregion

    #region Public Fucntion
    /// <summary>
    /// ������
    /// </summary>
    void IThrowable.Throw(Vector3 pos)
    {
        transform.position = pos;
        _rb.velocity = transform.forward * _moveSpeed;
        _arrowImage.gameObject.SetActive(false);
        IsThrowed = true;
    }

    /// <summary>
    /// �S�[�����̃A�N�V����
    /// </summary>
    public void AddCallBack(Action action)
    {
        OnGoaled += action;
    }

    public void CheckPointCallBack(Action action)
    {
        OnCheckPointed += action;
    }

    /// <summary>
    /// ��������
    /// </summary>
    public void TurnLeft()
    {
        var value = _directionValue - 0.1f;
        // �ő�l���傫�����
        if (value > -_valueOfMaxTurn)
        {
            _directionValue = value;
            transform.Rotate(0, -0.5f, 0);
        }
    }

    /// <summary>
    /// �E������
    /// </summary>
    public void TurnRight()
    {
        var value = _directionValue + 0.1f;
        // �ő�l��菬�������
        if (value < _valueOfMaxTurn)
        {
            _directionValue = value;
            transform.Rotate(0, 0.5f, 0);
        }
    }

    void Setup()
    {
        _directionValue = 0;

        IsThrowed = false;
        transform.SetParent(_parent);
        transform.SetAsLastSibling();
        transform.localPosition = _originPos;
        transform.localRotation = default;

        gameObject.SetActive(true);
        _arrowImage.gameObject.SetActive(true);
        _rb.velocity = Vector3.zero;

        transform.rotation = _default;
    }
    #endregion

    #region Private Function
    #endregion
    IEnumerator DelayVanishCoroutine()
    {
        yield return new WaitForSeconds(3.0f);

        gameObject.SetActive(false);
    }
}
