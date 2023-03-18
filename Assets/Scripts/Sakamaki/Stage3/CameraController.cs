using DG.Tweening;
using System;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Transform _startPoint;

    [SerializeField]
    private Transform _stopPoint;

    [Header("�J�����̈ړ��ɂ����鎞��")]
    [SerializeField]
    private float _duration;

    private bool Initialize = false;

    // �{�[���Ƃ̋���
    private float _distanceToBall;

    public bool HasBallPosition { get; private set; } = false;

    private void Start()
    {
        transform.position = _startPoint.position;
    }

    /// <summary>
    /// �J�����̈ړ������N�G�X�g
    /// </summary>
    public void MoveRequest(Action action = null)
    {
        if (Initialize is false)
        {
            transform.DORotate(_stopPoint.transform.eulerAngles, _duration);
            transform.DOMove(_stopPoint.position, _duration).OnComplete(() => action());
        }
        Initialize = true;
    }

    /// <summary>
    /// �ł�����̃J�����ړ�
    /// </summary>
    public void SendBallPosition(Vector3 ballPos)
    {
        var camPos = transform.position;
        if (HasBallPosition is false)
        {
            // �{�[���Ƃ̋���
            _distanceToBall = Math.Abs(ballPos.z - camPos.z) + 1f;
            HasBallPosition = true;
        }
        camPos.z = ballPos.z - _distanceToBall;
        transform.position = camPos;
    }
}
