using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// フェイズの開始時のUIの動きを管理するコンポーネント
/// </summary>
public class PhaseInfo : MonoBehaviour
{
    #region serialize
    [Header("Variables")]
    [Tooltip("パネルのフェード時間")]
    [SerializeField]
    float _fadeGroupTime = 0.25f;

    [Tooltip("アニメーション時間")]
    [SerializeField]
    float _animTime = 3.0f;

    [Tooltip("移動先のX座標")]
    [SerializeField]
    float _moveTargetValue_x = default;

    [Tooltip("アニメーションの種類")]
    [SerializeField]
    Ease _easeType = default;

    [Header("UIObjects")]
    [Tooltip("各フェイズのImage")]
    [SerializeField]
    Image[] _phaseImages = default;

    [Tooltip("アンダーバーのImage")]
    [SerializeField]
    Image _underbarImage = default;

    [SerializeField]
    Image _background = default;

    [SerializeField]
    CanvasGroup _phaseInfoGroup = default;
    #endregion

    #region private
    #endregion

    #region public
    #endregion

    #region property
    #endregion
    private void Awake()
    {
        _background.DOFade(0f, 0f);
        _phaseInfoGroup.alpha = 0f;
        _underbarImage.gameObject.transform.DOScaleX(0f, 0f);

        foreach (var image in _phaseImages)
        {
            image.enabled = false;
        }
    }


    public IEnumerator OnAnimation(int phase)
    {
        yield return _background.DOFade(1f, 0.25f)
                     .WaitForCompletion();

        DOTween.To(() => 
               _phaseInfoGroup.alpha,
               x => _phaseInfoGroup.alpha = x,
               1f,
               _fadeGroupTime);

        _phaseImages[phase].enabled = true;
        _phaseImages[phase].gameObject.transform
                           .DOLocalMoveX(_moveTargetValue_x, _animTime)
                           .SetEase(_easeType);

        yield return new WaitForSeconds(0.15f);

        _underbarImage.gameObject.transform.DOScaleX(1f, 0.5f);

        yield return new WaitForSeconds(1f);

        yield return DOTween.To(() =>
                     _phaseInfoGroup.alpha,
                     x => _phaseInfoGroup.alpha = x,
                     0f,
                     _fadeGroupTime)
                     .WaitForCompletion();

        yield return _background.DOFade(0f, 0.25f)
                     .WaitForCompletion();

        _phaseImages[phase].enabled = false;
    }
}
