using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

public class LobbyManager : MonoBehaviour
{
    #region serialize
    [Header("Objects")]
    [SerializeField]
    Transform _playerTrans = default;

    [SerializeField]
    Transform[] _startPlayerTrans = default;

    [SerializeField]
    GameObject _lobbyPanel = default;

    [SerializeField]
    MessagePlayer _messagePlayer = default;

    [SerializeField]
    LobbyClockController _clockCtrl = default;

    [SerializeField]
    CinemachineVirtualCamera _clockCamera = default;

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
    /// <summary> �h�A�ɋ߂Â�������Action </summary>
    public Action ApproachDoor { get; set; }
    /// <summary> �h�A���痣�ꂽ����Action </summary>
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
        SetPlayerPosition(GameManager.Instance.CurrentStage);
        yield return null;

        if (IsFirstArrival)
        {
            PlayerMove?.Invoke(false);
            StartCoroutine(_messagePlayer.PlayMessageCorountine(MessageType.Stage1_End, () =>
            {
                _clockCamera.Priority = 15;
                _clockCtrl.ChangeClockState(GameManager.Instance.CurrentClockState, action: () =>
                {
                    _clockCamera.Priority = 10;
                    StartCoroutine(OnPlayerMovable(3.0f));
                });
            }));
            IsFirstArrival = false;
        }
        else
        {
            _clockCamera.Priority = 15;
            _clockCtrl.ChangeClockState(GameManager.Instance.CurrentClockState, action: () =>
            {
                _clockCamera.Priority = 10;
                StartCoroutine(OnPlayerMovable(3.0f));
            });
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

    void SetPlayerPosition(Stages type)
    {
        switch (type)
        {
            case Stages.Stage1:
                _playerTrans.position = _startPlayerTrans[0].position;
                _playerTrans.rotation = _startPlayerTrans[0].rotation;
                break;
            case Stages.Stage2:
                _playerTrans.position = _startPlayerTrans[1].position;
                _playerTrans.rotation = _startPlayerTrans[1].rotation;
                break;
            case Stages.Stage3:
                _playerTrans.position = _startPlayerTrans[2].position;
                _playerTrans.rotation = _startPlayerTrans[2].rotation;
                break;
            case Stages.Stage4:
                _playerTrans.position = _startPlayerTrans[3].position;
                _playerTrans.rotation = _startPlayerTrans[3].rotation;
                break;
            case Stages.Stage5:
                _playerTrans.position = _startPlayerTrans[4].position;
                _playerTrans.rotation = _startPlayerTrans[4].rotation;
                break;
            case Stages.Stage6:
                _playerTrans.position = _startPlayerTrans[5].position;
                _playerTrans.rotation = _startPlayerTrans[5].rotation;
                break;
            default:
                break;
        }
    }

    IEnumerator OnPlayerMovable(float Interval)
    {
        yield return new WaitForSeconds(Interval);

        PlayerMove?.Invoke(true);
    }
}
[Serializable]
public class Stage
{
    public string SceneName;
    public SceneType Type;
}
