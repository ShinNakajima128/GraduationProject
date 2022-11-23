using DG.Tweening;
using System;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Transform _stopPoint;

    [SerializeField]
    private float _duration;

    public void MoveRequest(Action action = null)
    {
        transform.DOMove(_stopPoint.position, _duration).OnComplete(() => action());
    }
}
