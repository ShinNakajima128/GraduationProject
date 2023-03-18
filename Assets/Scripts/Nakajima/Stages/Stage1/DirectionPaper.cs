using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DirectionPaper : MonoBehaviour
{
    #region serialize
    [SerializeField]
    float _animTime = 2.0f;

    [SerializeField]
    Ease _easeType = default;
    #endregion

    #region private
    RectTransform _rect;
    #endregion
    #region property
    #endregion

    private void OnDisable()
    {
        transform.localPosition = Vector3.zero;
    }

    private void Start()
    {
        TryGetComponent(out _rect);
        gameObject.SetActive(false);
    }

    public void OnAnimation(Transform initial, RectTransform target, Action callback)
    {
        _rect.transform.position = RectTransformUtility.WorldToScreenPoint(Camera.main, initial.position);

        _rect.transform.DOLocalMove(target.localPosition, _animTime)
                       .SetEase(_easeType)
                       .OnComplete(() =>
                       {
                           callback?.Invoke();
                           gameObject.SetActive(false);
                       });
    }
}
