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
    }

    IEnumerator Start()
    {
        yield return null;

        TransitionManager.FadeOut(FadeType.Normal);
        LetterboxController.ActivateLetterbox(true, 0);
        AudioManager.StopBGM();
        PlayerMove?.Invoke(false);
        IsUIOperate?.Invoke(false);

        if (IsFirstVisit)
        {
            yield return _underLobbyObjectTrans.DOLocalMoveY(0, _startStageAnimTime)
                                               .SetEase(Ease.Linear)
                                               .WaitForCompletion();
            CameraShake();

            yield return new WaitForSeconds(1.0f);
        }
        else
        {
            _underLobbyObjectTrans.DOLocalMoveY(0, 0f);
        }

        LetterboxController.ActivateLetterbox(false, 1.5f);
        yield return new WaitForSeconds(1.5f);

        PlayerMove?.Invoke(true);
        IsUIOperate?.Invoke(true);
    }

    /// <summary>
    /// ステージの詳細を表示する
    /// </summary>
    /// <param name="type"> 遷移先のステージのScene </param>
    public static void OnStageDescription(SceneType type)
    {
        Instance.OnFadeDescription(1f, 0.3f);
        Instance._isApproached = true;

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
        Instance.StepAwayDoor?.Invoke();
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
}
