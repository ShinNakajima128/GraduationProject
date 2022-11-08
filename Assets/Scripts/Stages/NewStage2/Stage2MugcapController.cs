using DG.Tweening;
using System;
using UnityEngine;

public class Stage2MugcapController : MonoBehaviour
{
    [SerializeField]
    private Transform _targetUpPos;

    [SerializeField]
    private Transform _targetDownPos;

    [SerializeField]
    private float _duration;

    /// <summary>
    /// カップをあげる
    /// </summary>
    public void MoveUpRequest(Action action = null)
    {
        transform.DOLocalMove(_targetUpPos.localPosition, _duration).
            SetEase(Ease.Linear).
            OnComplete(() => action());
    }

    /// <summary>
    /// カップをさげる
    /// </summary>
    public void MoveDownRequest(Action action = null)
    {
        transform.DOLocalMove(_targetDownPos.localPosition, _duration).
            SetEase(Ease.Linear).
            OnComplete(() => action());
    }
}
