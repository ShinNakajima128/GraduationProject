using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// スチル獲得時の機能を持つコンポーネント
/// </summary>
public class GetStillController : MonoBehaviour
{
    #region serialize
    [SerializeField]
    float _frameAnimTime = 1.5f;

    [SerializeField]
    float _clearAnimTime = 0.5f;

    [SerializeField]
    Ease _frameEase = Ease.OutBounce;

    [Header("UI")]
    [SerializeField]
    CanvasGroup _stillPanelGroup = default;

    [SerializeField]
    Image _stillImage = default;

    [SerializeField]
    Image _clearImage = default;

    [SerializeField]
    Transform _StillFrameTrans = default;

    [Header("Data")]
    [SerializeField]
    StillData _stillData = default;
    #endregion

    #region private
    #endregion

    #region public
    #endregion

    #region property
    public static GetStillController Instance { get; private set; }
    #endregion

    private void Awake()
    {
        Instance = this;
        _stillPanelGroup.alpha = 0;
    }

    public static IEnumerator ActiveGettingStillPanel(Stages stage)
    {
        Instance.SetStageStill(stage);

        Instance._stillPanelGroup.alpha = 1;

        Instance._StillFrameTrans.DOMoveY(1100, 0f);

        yield return Instance._StillFrameTrans.DOLocalMoveY(26f, Instance._frameAnimTime)
                                              .SetEase(Instance._frameEase)
                                              .WaitForCompletion();

        Instance._clearImage.enabled = true;

        yield return Instance._clearImage.gameObject.transform.DOScale(5f, Instance._clearAnimTime)
                                                              .WaitForCompletion();
    }

    public static void InactiveGetStillPanel()
    {
        Instance._stillPanelGroup.alpha = 0;
    }

    /// <summary>
    /// 指定されたステージのスチルをセットする
    /// </summary>
    /// <param name="stages">ステージの種類</param>
    void SetStageStill(Stages stages)
    {
        _stillImage.sprite = _stillData.StageStills.FirstOrDefault(s => s.Stage == stages).StillSprite;
    }
}
