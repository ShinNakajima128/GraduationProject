using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// スチル獲得時の機能を持つコンポーネント
/// </summary>
public class GetStillController : MonoBehaviour
{
    #region serialize
    [Header("UI")]
    [SerializeField]
    CanvasGroup _stillPanelGroup = default;

    [SerializeField]
    Image _stillImage = default;

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

    public static void ActiveGettingStillPanel(Stages stage)
    {
        Instance.SetStageStill(stage);

        Instance._stillPanelGroup.alpha = 1;
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
