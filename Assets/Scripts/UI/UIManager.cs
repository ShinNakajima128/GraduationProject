using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    #region serialize
    [SerializeField]
    CanvasGroup _mainUIGroup = default;

    [SerializeField]
    UIPanel[] _Panels = default;
    #endregion

    #region private
    #endregion

    #region public
    #endregion

    #region property
    public static UIManager Instance { get; private set; }
    public bool IsCanOpenUI { get; private set; } = false;
    #endregion

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (GameManager.Instance.CurrentLobbyState == LobbyState.Default)
        {
            LobbyManager.Instance.PlayerMove += ChangeOperateUI;
        }
        else
        {

        }        
    }

    /// <summary>
    /// UI操作のフラグを切り替える
    /// </summary>
    /// <param name="isOperate"> 操作可能かどうか </param>
    void ChangeOperateUI(bool isOperate)
    {
        IsCanOpenUI = isOperate;
        _mainUIGroup.alpha = isOperate ? 1 : 0;
    }
}

[Serializable]
public struct UIPanel
{
    public string PanelName;
    public CanvasGroup PanelGroup;
    public UIPanelType PanelType;
}

/// <summary>
/// UIの種類
/// </summary>
public enum UIPanelType
{
    Pause,
    Option,
    GameEnd
}
