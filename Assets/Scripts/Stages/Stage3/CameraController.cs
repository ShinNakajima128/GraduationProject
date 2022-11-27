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

    private void Start()
    {
        transform.position = _startPoint.position;
    }

    public void MoveRequest(Action action = null)
    {
        transform.DOMove(_stopPoint.position, _duration).OnComplete(() => action());
    }
}
