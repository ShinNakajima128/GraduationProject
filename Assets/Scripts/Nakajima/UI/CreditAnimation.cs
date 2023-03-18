using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreditAnimation : MonoBehaviour
{
    #region serialize
    [Header("Variables")]
    [SerializeField]
    float _startDelayTime = 0.1f;

    [Tooltip("コマ送りの間隔")]
    [SerializeField]
    float _animInterval = 0.25f;

    [Header("UIObjects")]
    [Tooltip("アニメーションのImage")]
    [SerializeField]
    Image _animationImage = default;

    [Tooltip("アニメーション素材")]
    [SerializeField]
    Sprite[] _animationSprites = default;

    [Tooltip("アニメーションのCanvasGroup")]
    [SerializeField]
    CanvasGroup _animationGroup = default;
    #endregion

    #region private
    #endregion

    #region public
    #endregion

    #region property
    #endregion

    IEnumerator Start()
    {
        yield return new WaitForSecondsRealtime(_startDelayTime);
        StartCoroutine(AnimationCoroutine());
    }

    IEnumerator AnimationCoroutine()
    {
        while (true)
        {
            _animationImage.sprite = _animationSprites[0];

            yield return new WaitForSecondsRealtime(_animInterval);

            _animationImage.sprite = _animationSprites[1];

            yield return new WaitForSecondsRealtime(_animInterval);

            _animationImage.sprite = _animationSprites[2];

            yield return new WaitForSecondsRealtime(_animInterval);

            _animationImage.sprite = _animationSprites[1];

            yield return new WaitForSecondsRealtime(_animInterval);
        }
    }

    public void OnFadeDescription(float value, float fadeTime)
    {
        DOTween.To(() => _animationGroup.alpha,
                x => _animationGroup.alpha = x,
                value,
                fadeTime);
    }
}
