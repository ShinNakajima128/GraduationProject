using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// ロビーでのヒントを表示する機能を持つコンポーネント
/// </summary>
public class LobbyTipsUI : MonoBehaviour
{
    #region serialize
    [Header("Variables")]
    [SerializeField]
    float _fadeTime = 0.25f;

    [SerializeField]
    float _animHeight = 20f;

    [SerializeField]
    float _animTime = 1.0f;

    [Header("Data")]
    [Tooltip("ヒントデータ")]
    [SerializeField]
    TipsData[] _tipsDatas = default;

    [Header("UI_Objects")]
    [Tooltip("左側のヒントのCanvasGroup")]
    [SerializeField]
    CanvasGroup _leftTipsGroup = default;

    [Tooltip("右側のヒントのCanvasGroup")]
    [SerializeField]
    CanvasGroup _rightTipsGroup = default;

    [Tooltip("左側のヒントのText")]
    [SerializeField]
    Text _leftTipsText = default;

    [Tooltip("右側のヒントのText")]
    [SerializeField]
    Text _rightTipsText = default;
    #endregion

    #region private
    bool _isStageCleared;
    #endregion

    #region public
    #endregion

    #region property
    public static LobbyTipsUI Instance { get; private set; }
    public bool IsStageCleared { get => _isStageCleared; set => _isStageCleared = value; }
    #endregion

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _isStageCleared = GameManager.IsClearStaged;
        OnAnimation();
    }

    public static void UpdateTips()
    {
        TipsData leftTips = Instance._tipsDatas.Where(d => d.ClockState == GameManager.Instance.CurrentClockState)
                                               .FirstOrDefault(d => d.TipsType == TipsType.LeftTips);

        TipsData rightTips = Instance._tipsDatas.Where(d => d.ClockState == GameManager.Instance.CurrentClockState)
                                                .FirstOrDefault(d => d.TipsType == TipsType.RightTips);

        Instance._leftTipsText.text = leftTips.TipsText;
        Instance._rightTipsText.text = rightTips.TipsText;
        print($"{GameManager.Instance.CurrentClockState}");
    }

    /// <summary>
    /// ヒントのUIの表示/非表示を切り替える
    /// </summary>
    /// <param name="value"> フェードの値 </param>
    /// <param name="fadeTime"> フェード時間 </param>
    public void ActivateTipsPanel(bool isActivate)
    {
        if (isActivate)
        {
            OnFadeDescription(TipsType.LeftTips, 1f, _fadeTime);
            if (_isStageCleared)
            {
                OnFadeDescription(TipsType.RightTips, 1f, _fadeTime);
            }
        }
        else
        {
            OnFadeDescription(TipsType.LeftTips, 0f, _fadeTime);
            if (_isStageCleared)
            {
                OnFadeDescription(TipsType.RightTips, 0f, _fadeTime);
            }
        } 
    }

    public void InactiveAlbumTips()
    {
        OnFadeDescription(TipsType.RightTips, 0f, _fadeTime);
    }

    void OnFadeDescription(TipsType type, float value, float fadeTime)
    {
        switch (type)
        {
            case TipsType.LeftTips:
                DOTween.To(() => _leftTipsGroup.alpha,
                x => _leftTipsGroup.alpha = x,
                value,
                fadeTime);
                break;
            case TipsType.RightTips:
                DOTween.To(() => _rightTipsGroup.alpha,
                x => _rightTipsGroup.alpha = x,
                value,
                fadeTime);
                break;
            default:
                break;
        }
    }

    void OnAnimation()
    {
        _leftTipsGroup.gameObject.transform.DOLocalMoveY(_leftTipsGroup.gameObject.transform.localPosition.y + _animHeight, _animTime)
                                           .SetLoops(-1, LoopType.Yoyo);

        _rightTipsGroup.gameObject.transform.DOLocalMoveY(_rightTipsGroup.gameObject.transform.localPosition.y + _animHeight, _animTime)
                                           .SetLoops(-1, LoopType.Yoyo);
    }
}

/// <summary>
/// ヒントデータ
/// </summary>
[System.Serializable]
public struct TipsData
{
    public string TipsName;
    public TipsType TipsType;
    public ClockState ClockState;
    [TextArea(1, 3)]
    public string TipsText;
}
public enum TipsType
{
    LeftTips,
    RightTips
}
