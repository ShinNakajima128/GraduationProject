using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UnclearedIcon : MonoBehaviour
{
    #region serialize
    [SerializeField]
    Stages _targetCheckStage = default;

    [SerializeField]
    float _targetScale = 1.2f;

    [SerializeField]
    float _animTime = 0.5f;

    [SerializeField]
    Ease _animEase = Ease.InOutQuad;
    #endregion

    #region private
    #endregion

    #region public
    #endregion

    #region property
    public bool IsCleared { get; set; } = false;
    #endregion

    private void Start()
    {
        if (GameManager.CheckStageStatus(_targetCheckStage))
        {
            IsCleared = true;
            gameObject.SetActive(false);
        }
        else
        {
            transform.DOScale(_targetScale, _animTime)
                     .SetEase(_animEase)
                     .SetLoops(-1, LoopType.Yoyo);
        }
    }
}
