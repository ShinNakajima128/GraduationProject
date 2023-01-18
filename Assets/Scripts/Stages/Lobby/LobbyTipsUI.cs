using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ロビーでのヒントを表示する機能を持つコンポーネント
/// </summary>
public class LobbyTipsUI : MonoBehaviour
{
    #region serialize
    [Header("Variables")]
    [Tooltip("ヒントデータ")]
    [SerializeField]
    TipsData[] _tipsDatas = default;

    [Tooltip("左側のヒントのCanvasGroup")]
    [SerializeField]
    CanvasGroup _leftTipsGroup = default;

    [Tooltip("右側のヒントのCanvasGroup")]
    [SerializeField]
    CanvasGroup _rightTipsGroup = default;
    #endregion

    #region private
    #endregion

    #region public
    #endregion

    #region property
    #endregion


}

/// <summary>
/// ヒントデータ
/// </summary>
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
