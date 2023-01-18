using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���r�[�ł̃q���g��\������@�\�����R���|�[�l���g
/// </summary>
public class LobbyTipsUI : MonoBehaviour
{
    #region serialize
    [Header("Variables")]
    [Tooltip("�q���g�f�[�^")]
    [SerializeField]
    TipsData[] _tipsDatas = default;

    [Tooltip("�����̃q���g��CanvasGroup")]
    [SerializeField]
    CanvasGroup _leftTipsGroup = default;

    [Tooltip("�E���̃q���g��CanvasGroup")]
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
/// �q���g�f�[�^
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
