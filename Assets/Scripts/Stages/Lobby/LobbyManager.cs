using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using AliceProject;
using DG.Tweening;

public class LobbyManager : MonoBehaviour
{
    #region serialize
    [Header("Objects")]
    [SerializeField]
    Transform _playerTrans = default;

    [SerializeField]
    Transform[] _startPlayerTrans = default;

    [SerializeField]
    Transform _heartEffectTrans = default;

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
    CanvasGroup _stageDescriptionCanvas = default;

    [SerializeField]
    Text _stageNameText = default;

    [SerializeField]
    Image _StageImage = default;

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
        SetPlayerPosition(GameManager.Instance.CurrentStage); //プレイヤー位置をプレイしたミニゲームのドアの前に移動
        _clockCtrl.ChangeClockState(GameManager.Instance.CurrentClockState, 0f, 0f); //時計の状態をオブジェクトに反映

        if (!_debugMode)
        {
            yield return null;

            _provider.enabled = false;
            PlayerMove?.Invoke(false);

            yield return new WaitForSeconds(1.5f);

            //ロビーに初めて来たときはストーリーメッセージを再生
            if (IsFirstArrival)
            {
                StartCoroutine(_messagePlayer.PlayMessageCorountine(MessageType.Stage1_End, () =>
                {
                    ClockDirection();
                }));
                IsFirstArrival = false;
            }
            else 
            {
                //未クリア且つステージをクリアしている場合は時計の演出を行う
                if (!GameManager.CheckStageStatus() && GameManager.IsClearStaged)
                {
                    ClockDirection();
                }
                else
                {
                    StartCoroutine(OnPlayerMovable(1.5f));
                    Debug.Log("クリア済みステージ");
                }
            }
        }
        else
        {
            _provider.enabled = true;
            PlayerMove?.Invoke(true);
        }
        GameManager.SaveStageResult(false);
        OnFadeDescription(0f, 0f);
    }

    /// <summary>
    /// ステージの詳細を表示する
    /// </summary>
    /// <param name="type"> 遷移先のステージのScene </param>
    public static void OnStageDescription(SceneType type)
    {
        Instance.OnFadeDescription(1f, 0.3f);
        Instance._isApproached = true;

        var data = Instance._stageDatas.FirstOrDefault(d => d.Type == type);
        
        Instance._stageNameText.text = data.SceneName;
        Instance._StageImage.sprite = data.StageSprite;
        Instance.ApproachDoor?.Invoke();
    }

    /// <summary>
    /// ステージの詳細を非表示する
    /// </summary>
    public static void OffStageDescription()
    {
        Instance.OnFadeDescription(0f, 0.3f);
        Instance._isApproached = false;
        Instance.StepAwayDoor?.Invoke();
    }

    void OnFadeDescription(float value, float fadeTime)
    {
        DOTween.To(() => _stageDescriptionCanvas.alpha,
                x => _stageDescriptionCanvas.alpha = x,
                value,
                fadeTime);
    }

    /// <summary>
    /// プレイヤーの位置をドアの前に移動
    /// </summary>
    /// <param name="type"> Stageの種類 </param>
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

    /// <summary>
    /// カメラ、時計の演出
    /// </summary>
    void ClockDirection()
    {
        _clockCamera.Priority = 15; //演出用カメラをアクティブ化
        Camera.main.LayerCullingToggle("Ornament", false); //ロビーのライトなどの装飾品を非表示にする

        _clockCtrl.ChangeClockState(GameManager.CheckGameStatus(), action: () =>
        {
            GameManager.UpdateStageStatus(GameManager.Instance.CurrentStage);

            if (GameManager.Instance.CurrentClockState == ClockState.Twelve)
            {
                //ボスステージが出現する処理
                StartCoroutine(OnBossStageaAppearCoroutine());
            }
            else
            {
                _clockCamera.Priority = 10;
                Camera.main.LayerCullingToggle("Ornament", true);
                StartCoroutine(OnPlayerMovable(3.0f));
            } 
        });
    }

    /// <summary>
    /// プレイヤーを操作可能にする
    /// </summary>
    /// <param name="Interval"> 可能になるまでの時間 </param>
    /// <returns></returns>
    IEnumerator OnPlayerMovable(float Interval)
    {
        yield return new WaitForSeconds(Interval);

        PlayerMove?.Invoke(true);
        _provider.enabled = true;
    }

    IEnumerator OnBossStageaAppearCoroutine()
    {
        EffectManager.PlayEffect(EffectType.Heart, _heartEffectTrans.position);
        
        yield return new WaitForSeconds(2.0f);
        
        Debug.Log("ボスステージ出現");
        Camera.main.LayerCullingToggle("Ornament", true);
        _clockCamera.Priority = 10;
        StartCoroutine(OnPlayerMovable(3.0f));
    }
}
[Serializable]
public class Stage
{
    public string SceneName;
    public SceneType Type;
    public Sprite StageSprite;
}

