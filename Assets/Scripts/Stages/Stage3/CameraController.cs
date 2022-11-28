using DG.Tweening;
using System;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Transform _startPoint;

    [SerializeField]
    private Transform _stopPoint;

    [SerializeField]
    private float _duration;

    [SerializeField]
    private float _distanceToBallPosition;

    private void Start()
    {
        transform.position = _startPoint.position;
    }

    public void MoveRequest(Action action = null)
    {
        transform.DOMove(_stopPoint.position, _duration).OnComplete(() => action());
    }

    public void MoveCamera(float z)
    {
        var pos = transform.position;
        pos.z = z + _distanceToBallPosition;
        transform.position = pos;
    }
}
