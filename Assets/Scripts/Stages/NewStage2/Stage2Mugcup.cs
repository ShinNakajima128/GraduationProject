using DG.Tweening;
using System;
using UnityEngine;

public class Stage2Mugcup : MonoBehaviour
{
    public void MoveRequest(Vector3 position, float duration, Action action = null)
    {
        transform.DOLocalMove(position, duration).OnComplete(() => action());
    }
}