using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// ゲームスタート時にボタン入力を促す機能を持つコンポーネント
/// </summary>
public class GameStartIcon : MonoBehaviour
{
    #region serialize
    [Tooltip("アニメーションの速度")]
    [SerializeField]
    float _animTime = 1.0f;

    [Tooltip("透過値")]
    [SerializeField]
    float _fadeTargetValue = 0.7f;
    #endregion

    #region private
    CanvasGroup _iconGroup = default;
    bool _init = false;
    #endregion

    #region public
    #endregion

    #region property
    #endregion

    private void Awake()
    {
        TryGetComponent(out _iconGroup);
        _init = true;
    }

    private void Start()
    {
        OnAnimation();
    }

    /// <summary>
    /// アニメーションを再生する
    /// </summary>
    void OnAnimation()
    {
        _iconGroup.alpha = 1;

        DOTween.To(() => 
               _iconGroup.alpha,
               x => _iconGroup.alpha = x,
               _fadeTargetValue,
               _animTime)
               .SetEase(Ease.Linear)
               .SetLoops(-1, LoopType.Yoyo)
               .SetLink(gameObject, LinkBehaviour.RestartOnEnable);
    }
}
