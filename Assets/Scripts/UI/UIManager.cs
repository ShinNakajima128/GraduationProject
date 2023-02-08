using System;
using System.Linq;
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

    public bool IsAnyPanelOpened => _Panels.Any(p => p.IsOpened);
    #endregion

    private void Awake()
    {
        Instance = this;

        for (int i = 0; i < _Panels.Length; i++)
        {
            _Panels[i].PanelGroup.alpha = 0;
            _Panels[i].IsOpened = false;
        }
    }

    private void Start()
    {
        if (GameManager.Instance.CurrentScene != SceneType.Title)
        {
            if (GameManager.Instance.CurrentLobbyState == LobbyState.Default)
            {
                LobbyManager.Instance.IsUIOperate += ChangeOperateUI;
                LobbyManager.Instance.IsUIOperate += LobbyTipsUI.Instance.ActivateTipsPanel;
            }
            else
            {
                UnderLobbyManager.Instance.IsUIOperate += ChangeOperateUI;
                UnderLobbyManager.Instance.IsUIOperate += LobbyTipsUI.Instance.ActivateTipsPanel;
            }
        }
        else
        {
            ChangeOperateUI(true);
        }
    }

    public static void ActivatePanel(UIPanelType type)
    {
        if (Instance.IsCanOpenUI)
        {
            if (Instance.IsAnyPanelOpened)
            {
                return;
            }

            Instance._Panels[(int)type].IsOpened = true;
        }
    }

    public static void InactivatePanel(UIPanelType type)
    {
        if (Instance.IsCanOpenUI)
        {
            Instance._Panels[(int)type].IsOpened = false;
        }
    }

    public static void SwitchIsCanOpenFlag(bool isCanOpen)
    {
        Instance.IsCanOpenUI = isCanOpen;
    }

    /// <summary>
    /// UI‘€ì‚Ìƒtƒ‰ƒO‚ğØ‚è‘Ö‚¦‚é
    /// </summary>
    /// <param name="isOperate"> ‘€ì‰Â”\‚©‚Ç‚¤‚© </param>
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
    public bool IsOpened;
}

/// <summary>
/// UI‚Ìí—Ş
/// </summary>
public enum UIPanelType
{
    Pause,
    Album,
    Tutorial,
    Option,
    GameEnd
}
