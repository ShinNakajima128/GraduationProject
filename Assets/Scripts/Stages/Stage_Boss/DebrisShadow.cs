using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// がれきの影のコンポーネント
/// </summary>
public class DebrisShadow : MonoBehaviour
{
    #region serialize
    [SerializeField]
    float _originScale = 0.2f;

    [SerializeField]
    Ease _shadowEase = Ease.InQuart;
    #endregion

    #region private
    #endregion

    #region public
    #endregion

    #region property
    #endregion

    private void OnDisable()
    {
        transform.localScale = new Vector3(_originScale, _originScale, _originScale);
    }

    /// <summary>
    /// 影のアニメーションを再生する
    /// </summary>
    /// <param name="animTime"></param>
    public void OnAnimation(float animTime, Action action = null)
    {
        transform.DOScale(1.2f, animTime)
                 .SetEase(_shadowEase)
                 .OnComplete(() => 
                 {
                     //gameObject.SetActive(false);
                     action?.Invoke();
                 })
                 .SetLink(gameObject, LinkBehaviour.KillOnDisable);
    }
}
