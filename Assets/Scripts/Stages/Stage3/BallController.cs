using System;
using UnityEngine;
using UnityEngine.Networking;

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
    private CameraController _camera;

    [SerializeField]
    private GameObject _arrowImage;

    private bool IsThrowed { get; set; } = false;

    private event Action OnGoaled;
    private Rigidbody _rb;
    private Vector3 _direction;

    /// <summary>
    /// �U������̒l(0������)
    /// </summary>
    private float _directionValue = 0;
    #endregion

    #region Unity Fucntion
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        _direction = _rb.velocity;
        if (IsThrowed)
        {
            ForwardRotation();
            MoveCameraRequest();
        }
    }

    /// <summary>
    /// �O�]����
    /// </summary>
    private void ForwardRotation()
    {
        transform.Rotate(new Vector3(3, 0, 0));
    }

    /// <summary>
    /// �J�����𓮂���
    /// </summary>
    private void MoveCameraRequest()
    {
        _camera.SendBallPosition(this.transform.position);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Goal")
        {
            Debug.Log("Goal");
            OnGoaled();
            this.gameObject.SetActive(false);
            return;
        }

        if (collision.gameObject.CompareTag("Block"))
        {
            // �@�����擾
            var normal = collision.contacts[0].normal;
            // ���˃x�N�g�����擾
            Vector3 result = Vector3.Reflect(_direction, normal);
            _rb.velocity = result;
            return;
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
            transform.Rotate(0, -0.1f, 0);
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
            transform.Rotate(0, 0.1f, 0);
        }
        
    }
    #endregion

    #region Private Function
    #endregion
}
