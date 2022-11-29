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
            transform.DOMove(_stopPoint.position, _duration).OnComplete(() => action());
        }
        Initialize = true;
    }

    /// <summary>
    /// �ł�����̃J�����ړ�
    /// </summary>
    public void MoveCamera(Vector3 ballPos)
    {
        var camPos = transform.position;
        // �{�[���Ƃ̋���
        var distance = Math.Abs(ballPos.z - camPos.z);
        camPos.z = camPos.z + -distance;
        // transform.position = camPos;
    }
}
