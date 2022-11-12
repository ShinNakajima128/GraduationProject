using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    #region serialize

    [Header("Objects")]
    [SerializeField]
    GameObject _lobbyPanel = default;

    [SerializeField]
    MessagePlayer _mp = default;

    [Header("UI")]
    [SerializeField]
    GameObject _stageDescriptionPanel = default;

    [SerializeField]
    Text _stageNameText = default;

    [SerializeField]
    Stage[] _stageDatas = default;
    #endregion

    #region private
    bool _isApproached = false;
    #endregion
    #region property
    public static LobbyManager Instance { get; private set; }
    public static bool IsFirstArrival { get; private set; } = true;
    /// <summary> ドアに近づいた時のAction </summary>
    public Action ApproachDoor { get; set; }
    /// <summary> ドアから離れた時のAction </summary>
    public Action StepAwayDoor { get; set; }
    public Action<bool> PlayerMove { get; set; }
    public bool IsApproached => _isApproached;
    #endregion

    private void Awake()
    {
        Instance = this;
        _stageNameText.text = "";
    }

    IEnumerator Start()
    {
        yield return null;

        if (IsFirstArrival)
        {
            PlayerMove?.Invoke(false);
            StartCoroutine(_mp.PlayMessageCorountine(MessageType.Stage1_End, () => 
            {
                PlayerMove?.Invoke(true);
            }));
            IsFirstArrival = false;
        }
    }
    public static void OnStageDescription(SceneType type)
    {
        Instance._stageDescriptionPanel.SetActive(true);
        Instance._isApproached = true;
        Instance._stageNameText.text = Instance._stageDatas.FirstOrDefault(d => d.Type == type).SceneName;
        Instance.ApproachDoor?.Invoke();
    }

    public static void OffStageDescription()
    {
        Instance._stageDescriptionPanel.SetActive(false);
        Instance._isApproached = false;
        Instance._stageNameText.text = "";
        Instance.StepAwayDoor?.Invoke();
    }
}
[Serializable]
public class Stage
{
    public string SceneName;
    public SceneType Type;
}

