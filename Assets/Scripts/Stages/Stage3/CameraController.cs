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
            transform.DOMove(_stopPoint.position, _duration).OnComplete(() => action());
        }
        Initialize = true;
    }

    /// <summary>
    /// 打った後のカメラ移動
    /// </summary>
    public void MoveCamera(Vector3 ballPos)
    {
        var camPos = transform.position;
        // ボールとの距離
        var distance = Math.Abs(ballPos.z - camPos.z);
        camPos.z = camPos.z + -distance;
        // transform.position = camPos;
    }
}
