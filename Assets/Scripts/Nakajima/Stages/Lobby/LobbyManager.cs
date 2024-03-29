using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using AliceProject;
using DG.Tweening;
using UniRx;

public class LobbyManager : MonoBehaviour
{
    #region serialize
    [Header("Variables")]
    [SerializeField]
    float _colorFadeTime = 2.0f;

    [SerializeField]
    LobbyUIState _currentUIState = default;

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
    Transform _mainStage = default;

    [Tooltip("地下へ向かう際のプレイヤーの位置")]
    [SerializeField]
    Transform _goingUnderTrans = default;

    [Tooltip("演出用のチェシャ猫")]
    [SerializeField]
    GameObject[] _cheshireCats = default;

    [Tooltip("未クリアステージの強調アイコンの親オブジェクト")]
    [SerializeField]
    GameObject _unclearedIconsParent = default;

    [Header("Components")]
    [SerializeField]
    MessagePlayer _messagePlayer = default;

    [SerializeField]
    LobbyClockController _clockCtrl = default;

    [SerializeField]
    CinemachineVirtualCamera _clockCamera = default;

    [SerializeField]
    CinemachineVirtualCamera _clock_ShakeCamera = default;

    [SerializeField]
    CinemachineVirtualCamera _goingUnderCamera = default;

    [SerializeField]
    CinemachineVirtualCamera _cheshireCatCamera = default;

    [SerializeField]
    CinemachineInputProvider _provider = default;

    [SerializeField]
    CinemachineBrain _brain = default;

    [SerializeField]
    Renderer[] _handsRenderers = default;

    [SerializeField]
    DirectionCameraManager _directionCameraMng = default;

    [Header("UI")]
    [SerializeField]
    GameObject _stageDescriptionPanel = default;

    [SerializeField]
    CanvasGroup _stageDescriptionCanvas = default;

    [SerializeField]
    CanvasGroup _lobbyLogoGroup = default;

    [SerializeField]
    Text _stageNameText = default;

    [SerializeField]
    Image _StageImage = default;

    [SerializeField]
    Stage[] _stageDatas = default;

    [Header("Renderer")]
    [SerializeField]
    Renderer _clockRenderer = default;

    [SerializeField]
    Material _goToUnderStageMat;

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
    public static bool IsAllStageCleared { get; private set; } = false;
    public static Stages BeforeStage { get; set; }
    /// <summary> ドアに近づいた時のAction </summary>
    public Action ApproachDoor { get; set; }
    /// <summary> ドアから離れた時のAction </summary>
    public Action StepAwayDoor { get; set; }
    public Action<bool> PlayerMove { get; set; }
    public Action<bool> IsUIOperate { get; set; }
    public Action BossStageAppear { get; set; }
    public bool IsApproached => _isApproached;
    /// <summary> 演出中かどうか </summary>
    public bool IsDuring { get; private set; } = false;
    public LobbyUIState CurrentUIState { get => _currentUIState; set => _currentUIState = value; }
    #endregion

    private void Awake()
    {
        Instance = this;
        _stageNameText.text = "";
        PlayerMove += CameraMovable;
        _unclearedIconsParent.SetActive(false);
    }

    IEnumerator Start()
    {
        SetPlayerPosition(GameManager.Instance.CurrentStage); //プレイヤー位置をプレイしたミニゲームのドアの前に移動
        _clockCtrl.ChangeClockState(GameManager.Instance.CurrentClockState, 0f, 0f); //時計の状態をオブジェクトに反映
        IsDuring = true;
        SkipButton.Instance.Isrespond += () => IsDuring;
        if (!_debugMode)
        {
            if (!IsFirstArrival)
            {
                //AudioManager.PlayBGM(BGMType.Lobby);
            }
            else
            {
                SkipButton.Instance.OnSkip.Subscribe(_ =>
                {
                    StopAllCoroutines();
                    StartCoroutine(SkipCoroutine());
                });

                AudioManager.StopBGM(1.0f);
                EventManager.ListenEvents(Events.Lobby_MeetingCheshire, PlayMeetingBGM);
                EventManager.ListenEvents(Events.Lobby_Introduction, () =>
                {
                    StartCoroutine(_directionCameraMng.StartDirectionCoroutine(CameraDirectionType.Lobby_Introduction));
                });
                EventManager.ListenEvents(Events.Alice_Surprised, () => StartCoroutine(_directionCameraMng.StartDirectionCoroutine(CameraDirectionType.Lobby_AliceAndCheshireTalking)));
                EventManager.ListenEvents(Events.Cheshire_StartGrinning, () =>
                {
                    _directionCameraMng.ResetCamera();
                });
            }

            TransitionManager.FadeIn(FadeType.Normal, 0f);
            yield return null;

            PlayerMove?.Invoke(false);
            IsUIOperate?.Invoke(false);

            yield return new WaitForSeconds(1.5f);

            //ロビーに初めて来たときはストーリーメッセージを再生
            if (IsFirstArrival)
            {
                yield return _messagePlayer.PlayMessageCorountine(MessageType.Stage1_End);

                TransitionManager.FadeIn(FadeType.White_default, action: () =>
                {
                    TransitionManager.FadeOut(FadeType.Normal);
                });

                yield return new WaitForSeconds(1.5f);

                AudioManager.PlaySE(SEType.Lobby_FirstVisit);
                yield return _directionCameraMng.StartDirectionCoroutine(CameraDirectionType.Lobby_FirstVisit);

                TransitionManager.FadeIn(FadeType.Black_Transparent, 0f);
                TransitionManager.FadeIn(FadeType.Black_default, action: () =>
                {
                    _directionCameraMng.ResetCamera(CameraDirectionType.Lobby_FirstVisit, 0f);
                    StartCoroutine(_directionCameraMng.StartDirectionCoroutine(CameraDirectionType.Lobby_Alice_Front));
                    TransitionManager.FadeOut(FadeType.Normal);
                });

                yield return new WaitForSeconds(3.0f);

                yield return _messagePlayer.PlayMessageCorountine(MessageType.Lobby_Visit, () =>
                {
                    //チェシャ猫登場演出
                    TransitionManager.FadeIn(FadeType.Black_Transparent, 0f);
                    TransitionManager.FadeIn(FadeType.Mask_CheshireCat, action: () =>
                    {
                        _directionCameraMng.ResetCamera(CameraDirectionType.Lobby_Alice_Front, 0f);
                        _directionCameraMng.SetBlendTime(3.0f);
                        LetterboxController.ActivateLetterbox(true, 0f);
                        LobbyCheshireCatManager.Instance.ActiveCheshireCat(LobbyCheshireCatType.Appearance);
                        TransitionManager.FadeOut(FadeType.Normal);
                        //AudioManager.PlaySE(SEType.Lobby_MeetingCheshire);
                    });
                });

                yield return new WaitForSeconds(10.5f);

                TransitionManager.FadeIn(FadeType.Normal, action: () =>
                {
                    LetterboxController.ActivateLetterbox(false, 0f);
                    _cheshireCatCamera.Priority = 14;
                    LobbyCheshireCatManager.Instance.ActiveCheshireCat(LobbyCheshireCatType.Movable);
                    TransitionManager.FadeOut(FadeType.Normal, 0f, () =>
                    {
                        TransitionManager.FadeOut(FadeType.Mask_CheshireCat);
                    });
                });

                yield return new WaitForSeconds(3.0f);

                yield return _messagePlayer.PlayMessageCorountine(MessageType.Lobby_CheshireCat, () =>
                {
                    //チェシャ猫をディゾルブで非表示にする
                    LobbyCheshireCatManager.Instance.MovableCat.ActivateDissolve(true);
                });

                _directionCameraMng.ResetBlendTime();

                yield return new WaitForSeconds(2.0f);

                //時計の演出
                ClockDirection();
                AudioManager.StopBGM(1.5f); //一旦曲を止める
            }
            else
            {
                TransitionManager.FadeOut(FadeType.Black_default, 2.0f);

                //画面フェード終了を待機
                yield return new WaitForSeconds(2.0f);

                //未クリア且つステージをクリアしている場合は時計の演出を行う
                if (!GameManager.CheckStageStatus() && GameManager.IsClearStaged)
                {
                    ClockDirection();
                }
                else
                {
                    LobbyTipsUI.UpdateTips();
                    AudioManager.PlayBGM(BGMType.Lobby);
                    StartCoroutine(OnPlayerMovable(0.2f));
                    Debug.Log("クリア済みステージ、またはステージ失敗");
                }
            }
        }
        else
        {
            TransitionManager.FadeOut(FadeType.Black_default, 1.5f);
            yield return new WaitForSeconds(1.5f);

            AudioManager.PlayBGM(BGMType.Lobby);
            PlayerMove?.Invoke(true);
            IsUIOperate?.Invoke(true);
            _unclearedIconsParent.SetActive(true);
            IsDuring = false;
            StartCoroutine(LobbyNameInfomationCoroutine());
            GameManager.UpdateStageStatus(GameManager.Instance.CurrentStage);
            LobbyCheshireCatManager.Instance.ActiveCheshireCat(LobbyCheshireCatType.Movable);
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
        UIManager.SwitchIsCanOpenFlag(false);

        var data = Instance._stageDatas.FirstOrDefault(d => d.Type == type);

        Instance._stageNameText.text = data.SceneName;
        Instance._StageImage.sprite = data.StageSprite;
        Instance.ApproachDoor?.Invoke();

        AudioManager.PlaySE(SEType.Lobby_NearDoor);
        StageDescriptionUI.Instance.ActiveDescription(type);
    }

    /// <summary>
    /// ステージの詳細を非表示する
    /// </summary>
    public static void OffStageDescription()
    {
        Instance.OnFadeDescription(0f, 0.3f);
        Instance._isApproached = false;
        UIManager.SwitchIsCanOpenFlag(true);
        Instance.StepAwayDoor?.Invoke();

        StageDescriptionUI.Instance.InActiceDescription();
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
        LetterboxController.ActivateLetterbox(true);

        _clockCtrl.ChangeClockState(GameManager.CheckGameStatus(), action: () =>
        {
            GameManager.UpdateStageStatus(GameManager.Instance.CurrentStage);
            ClockUI.Instance.SetClockUI(GameManager.Instance.CurrentClockState);
            LobbyTipsUI.UpdateTips();

            if (GameManager.Instance.CurrentClockState == ClockState.Twelve)
            {
                //ボスステージが出現する処理
                StartCoroutine(OnBossStageAppearCoroutine());
            }
            else
            {
                StartCoroutine(ReturnPlayerCamera());
            }
        },
        isPlaySE: true);
    }

    /// <summary>
    /// チェシャ猫と対面した時のBGMを再生
    /// </summary>
    void PlayMeetingBGM()
    {
        AudioManager.PlayBGM(BGMType.Lobby_MeetingCheshire);
    }

    /// <summary>
    /// カメラ操作の切り替え
    /// </summary>
    /// <param name="isMovable"> 動かせるかどうか </param>
    void CameraMovable(bool isMovable)
    {
        _provider.enabled = isMovable;
    }

    /// <summary>
    /// ロビーの初回到達フラグをリセット
    /// </summary>
    public static void Reset()
    {
        IsFirstArrival = true;
        IsAllStageCleared = false;
    }

    /// <summary>
    /// 12時になった時の時計の発光処理のコルーチン
    /// </summary>
    IEnumerator OnHandsEmission()
    {
        Debug.Log("Call");
        _handsRenderers[0].material.SetColor("_EmissionColor", Color.white);
        _handsRenderers[1].material.SetColor("_EmissionColor", Color.white);

        yield return new WaitForSeconds(0.1f);

        Material mat = _handsRenderers[0].material;

        mat.DOColor(Color.black, _colorFadeTime).OnUpdate(() =>
        {
            _handsRenderers[0].material.SetColor("_EmissionColor", mat.color);
            _handsRenderers[1].material.SetColor("_EmissionColor", mat.color);
        });
    }

    /// <summary>
    /// プレイヤーを操作可能にする
    /// </summary>
    /// <param name="Interval"> 可能になるまでの時間 </param>
    /// <returns></returns>
    IEnumerator OnPlayerMovable(float Interval, Action action = null)
    {
        yield return new WaitForSeconds(Interval);

        if (IsFirstArrival)
        {
            IsFirstArrival = false;
        }

        LetterboxController.ActivateLetterbox(false);


        yield return new WaitForSeconds(0.5f);

        IsDuring = false;
        PlayerMove?.Invoke(true);
        IsUIOperate?.Invoke(true);
        _unclearedIconsParent.SetActive(true);
        action?.Invoke();
        Debug.Log("call");
        LobbyCheshireCatManager.Instance.ActiveCheshireCat(LobbyCheshireCatType.Movable, true);
        StartCoroutine(LobbyNameInfomationCoroutine());
    }

    /// <summary>
    /// ボス戦へ進むコルーチン
    /// </summary>
    IEnumerator OnBossStageAppearCoroutine()
    {
        StartCoroutine(OnHandsEmission());
        _clockRenderer.material = _goToUnderStageMat;
        IsAllStageCleared = true;

        yield return new WaitForSeconds(2.0f);

        TransitionManager.FadeIn(FadeType.Normal, 0.5f, () =>
         {
             _playerTrans.localPosition = _goingUnderTrans.position;
             _brain.m_DefaultBlend.m_Time = 0;
             _clock_ShakeCamera.Priority = 20;
             _clockCtrl.OnCrazyClock();
             VibrationController.OnVibration(Strength.Low, 10f);
             AudioManager.PlaySE(SEType.UnderLobby_Lowering);

             TransitionManager.FadeOut(FadeType.Normal, 0.5f);
         });

        yield return new WaitForSeconds(3.5f);

        _goingUnderCamera.Priority = 25;
        BossStageAppear?.Invoke();

        float timer = 0;
        bool isFading = false;

        yield return _mainStage.DOLocalMoveY(2f, 10f)
                               .OnUpdate(() =>
                               {
                                   timer += Time.deltaTime;

                                   if (timer >= 5.0f && !isFading)
                                   {
                                       GameManager.ChangeLobbyState(LobbyState.Under);
                                       TransitionManager.SceneTransition(SceneType.UnderLobby, action: () =>
                                       {
                                           AudioManager.StopSE();
                                       });
                                       isFading = true;
                                   }
                               })
                               .SetLink(_mainStage.gameObject)
                               .WaitForCompletion();
    }

    /// <summary>
    /// アクティブカメラをプレイヤーカメラに戻す
    /// </summary>
    /// <returns></returns>
    IEnumerator ReturnPlayerCamera()
    {
        yield return new WaitForSeconds(2.0f);

        _cheshireCatCamera.Priority = 10;
        _clockCamera.Priority = 10;
        Camera.main.LayerCullingToggle("Ornament", true);
        StartCoroutine(OnPlayerMovable(3.0f, () => AudioManager.PlayBGM(BGMType.Lobby)));
    }

    /// <summary>
    /// ロビーの名前をしばらく表示した後に非表示にするコルーチン
    /// </summary>
    /// <returns></returns>
    IEnumerator LobbyNameInfomationCoroutine()
    {
        DOTween.To(() => _lobbyLogoGroup.alpha,
                    x => _lobbyLogoGroup.alpha = x,
                    1f,
                    1f)
                .OnComplete(() => { print("ロゴ表示"); });

        yield return new WaitForSeconds(3f);

        DOTween.To(() => _lobbyLogoGroup.alpha,
                    x => _lobbyLogoGroup.alpha = x,
                    0f,
                    1f);
    }

    IEnumerator SkipCoroutine()
    {
        AudioManager.StopSE();
        //TransitionManager.FadeIn(FadeType.Black_Transparent, 0f);
        TransitionManager.FadeIn(FadeType.Black_default, action: () =>
        {
            MessagePlayer.Instance.FadeMessageCanvas(0f, 0f);
            LobbyCheshireCatManager.Instance.InactiveCheshireCat(LobbyCheshireCatType.Appearance);
            LobbyCheshireCatManager.Instance.MovableCat.ActivateDissolve(true);
            ClockUI.Instance.SetClockUI(GameManager.CheckGameStatus());
            _clockCtrl.ChangeClockState(GameManager.CheckGameStatus(), 0f, 0f);
            GameManager.UpdateStageStatus(GameManager.Instance.CurrentStage);
            SkipButton.Instance.gameObject.SetActive(false);
            _cheshireCatCamera.Priority = 10;
            _clockCamera.Priority = 10;
        });

        yield return new WaitForSeconds(2.0f);

        LobbyTipsUI.UpdateTips();
        _directionCameraMng.SetBlendTime(0f);
        _directionCameraMng.ResetCamera();

        yield return new WaitForSeconds(1.0f);

        TransitionManager.FadeOut(FadeType.Normal);
        _directionCameraMng.ResetBlendTime();

        //AudioManager.StopBGM(1.5f); //一旦曲を止める
        StartCoroutine(OnPlayerMovable(1.5f, () => AudioManager.PlayBGM(BGMType.Lobby)));
    }
}
[Serializable]
public class Stage
{
    public string SceneName;
    public SceneType Type;
    public Sprite StageSprite;
}

public enum LobbyUIState
{
    Default,
    Album,
    Option
}

