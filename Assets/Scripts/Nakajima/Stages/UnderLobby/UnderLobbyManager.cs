using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Cinemachine;

public class UnderLobbyManager : MonoBehaviour
{
    #region serialize
    [Header("Variables")]
    [SerializeField]
    LobbyUIState _currentUIState = default;

    [Tooltip("ロビー地下に初めて来た時の演出時間")]
    [SerializeField]
    float _startStageAnimTime = 8.0f;

    [SerializeField]
    Stage _bossStageData = default;

    [Header("Objects")]
    [SerializeField]
    Transform _underLobbyObjectTrans = default;

    [Header("UI")]
    [SerializeField]
    GameObject _stageDescriptionPanel = default;

    [SerializeField]
    CanvasGroup _stageDescriptionCanvas = default;

    [SerializeField]
    CanvasGroup _underLobbyLogoGroup = default;

    [Header("Components")]
    [SerializeField]
    LobbyClockController _clockCtrl = default;

    [SerializeField]
    CinemachineInputProvider _provider = default;

    [Header("Debug")]
    [SerializeField]
    bool _debugMode = false;
    #endregion

    #region private
    bool _isApproached = false;
    CinemachineImpulseSource _impulseSource;
    #endregion

    #region public
    /// <summary> ドアに近づいた時のAction </summary>
    public Action ApproachDoor { get; set; }
    /// <summary> ドアから離れた時のAction </summary>
    public Action StepAwayDoor { get; set; }
    public Action<bool> PlayerMove { get; set; }
    public Action<bool> IsUIOperate { get; set; }
    #endregion

    #region property
    public static UnderLobbyManager Instance { get; private set; }
    public static bool IsFirstVisit { get; set; } = true;
    public LobbyUIState CurrentUIState { get => _currentUIState; set => _currentUIState = value; }
    /// <summary> 演出中かどうか </summary>
    public bool IsDuring { get; private set; } = false;
    #endregion

    private void Awake()
    {
        Instance = this;
        TryGetComponent(out _impulseSource);

        if (_debugMode)
        {
            IsFirstVisit = false;
        }

        PlayerMove += CameraMovable;
    }

    IEnumerator Start()
    {
        TransitionManager.FadeOut(FadeType.Black_default);

        yield return null;
        LetterboxController.ActivateLetterbox(true, 0);
        AudioManager.PlayBGM(BGMType.UnderLobby);
        PlayerMove?.Invoke(false);
        IsUIOperate?.Invoke(false);

        if (IsFirstVisit)
        {
            IsFirstVisit = false;
            LobbyTipsUI.Instance.IsStageCleared = true;
            EventManager.OnEvent(Events.Alice_Overlook);
            StartCoroutine(_clockCtrl.CrazyClockCoroutine(5f, 3f));
            AudioManager.PlaySE(SEType.UnderLobby_Lowering);
            VibrationController.OnVibration(Strength.Low, _startStageAnimTime + 0.5f);

            yield return _underLobbyObjectTrans.DOLocalMoveY(0, _startStageAnimTime)
                                               .SetEase(Ease.Linear)
                                               .WaitForCompletion();

            AudioManager.StopSE();
            AudioManager.PlaySE(SEType.UnderLobby_Arrival);
            VibrationController.OnVibration(Strength.Middle, 0.3f);
            CameraShake();

            yield return new WaitForSeconds(1.0f);
        }
        else
        {
            _underLobbyObjectTrans.DOLocalMoveY(0, 0f);
        }

        LetterboxController.ActivateLetterbox(false, 1.5f);
        yield return new WaitForSeconds(1.5f);
        LobbyTipsUI.UpdateTips();

        PlayerMove?.Invoke(true);
        IsUIOperate?.Invoke(true);
        StartCoroutine(UnderLobbyNameInfomationCoroutine());
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
    }

    void CameraMovable(bool isMovable)
    {
        _provider.enabled = isMovable;
    }

    /// <summary>
    /// カメラを揺らす
    /// </summary>
    void CameraShake()
    {
        _impulseSource.GenerateImpulse();
    }

    void OnFadeDescription(float value, float fadeTime)
    {
        DOTween.To(() => _stageDescriptionCanvas.alpha,
                x => _stageDescriptionCanvas.alpha = x,
                value,
                fadeTime);
    }

    IEnumerator UnderLobbyNameInfomationCoroutine()
    {
        DOTween.To(() => _underLobbyLogoGroup.alpha,
                    x => _underLobbyLogoGroup.alpha = x,
                    1f,
                    1f)
               .OnComplete(() => { print("ロゴ表示"); });

        yield return new WaitForSeconds(3f);

        DOTween.To(() => _underLobbyLogoGroup.alpha,
                    x => _underLobbyLogoGroup.alpha = x,
                    0f,
                    1f);
    }
}
