using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// �Q�[���X�^�[�g���Ƀ{�^�����͂𑣂��@�\�����R���|�[�l���g
/// </summary>
public class GameStartIcon : MonoBehaviour
{
    #region serialize
    [Tooltip("�A�j���[�V�����̑��x")]
    [SerializeField]
    float _animTime = 1.0f;

    [Tooltip("���ߒl")]
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
    /// �A�j���[�V�������Đ�����
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
