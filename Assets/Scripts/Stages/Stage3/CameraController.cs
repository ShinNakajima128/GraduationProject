using DG.Tweening;
using System;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Transform _startPoint;

    [SerializeField]
    private Transform _stopPoint;

    [Header("カメラの移動にかける時間")]
    [SerializeField]
    private float _duration;

    private bool Initialize = false;

    // ボールとの距離
    private float _distanceToBall;

    public bool HasBallPosition { get; private set; } = false;

    private void Start()
    {
        transform.position = _startPoint.position;
    }

    /// <summary>
    /// カメラの移動をリクエスト
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
    /// 打った後のカメラ移動
    /// </summary>
    public void SendBallPosition(Vector3 ballPos)
    {
        var camPos = transform.position;
        if (HasBallPosition is false)
        {
            // ボールとの距離
            _distanceToBall = Math.Abs(ballPos.z - camPos.z) + 1f;
            HasBallPosition = true;
        }
        camPos.z = ballPos.z - _distanceToBall;
        transform.position = camPos;
    }
}
