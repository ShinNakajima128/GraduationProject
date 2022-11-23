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

    [SerializeField]
    CinemachineInputProvider _provider = default;

    [Header("UI")]
    [SerializeField]
    GameObject _stageDescriptionPanel = default;

    [SerializeField]
    Text _stageNameText = default;

    [SerializeField]
    Stage[] _stageDatas = default;

    [Header("Debug")]
    [SerializeField]
    bool _debugMode = false;
    #endregion

    #region private
    bool _isApproached = false;
    #endregion
    #region property
    public static LobbyManager Instance { get; private set; }
    public static bool IsFirstArrival { get; private set; } = true;
    public static Stages BeforeStage { get; set; }
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
        _clockCtrl.ChangeClockState(GameManager.Instance.CurrentClockState, 0f, 0f);

        if (!_debugMode)
        {
            yield return null;

            _provider.enabled = false;
            PlayerMove?.Invoke(false);

            yield return new WaitForSeconds(1.5f);

            if (IsFirstArrival)
            {
                StartCoroutine(_messagePlayer.PlayMessageCorountine(MessageType.Stage1_End, () =>
                {
                    ClockDirection();
                }));
                IsFirstArrival = false;
            }
            else if (!GameManager.CheckStageStatus()) //���N���A�̎��͎��v�̉��o���s��
            {
                ClockDirection();
            }
            else
            {
                StartCoroutine(OnPlayerMovable(1.5f));
                Debug.Log("�N���A�ς݃X�e�[�W");
            }
        }
        PlayerMove?.Invoke(true);
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
        _provider.enabled = true;
    }

    /// <summary>
    /// �J�����A���v�̉��o
    /// </summary>
    void ClockDirection()
    {
        _clockCamera.Priority = 15;
        _clockCtrl.ChangeClockState(GameManager.CheckGameStatus(), action: () =>
        {
            GameManager.UpdateStageStatus(GameManager.Instance.CurrentStage);
            _clockCamera.Priority = 10;
            StartCoroutine(OnPlayerMovable(3.0f));
        });
    }
}
[Serializable]
public class Stage
{
    public string SceneName;
    public SceneType Type;
}

