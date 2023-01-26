using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UnderLobbyManager : MonoBehaviour
{
    #region serialize
    [Header("Variables")]
    [SerializeField]
    LobbyUIState _currentUIState = default;

    [Tooltip("���r�[�n���ɏ��߂ė������̉��o����")]
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
    #endregion

    #region public
    /// <summary> �h�A�ɋ߂Â�������Action </summary>
    public Action ApproachDoor { get; set; }
    /// <summary> �h�A���痣�ꂽ����Action </summary>
    public Action StepAwayDoor { get; set; }
    public Action<bool> PlayerMove { get; set; }
    public Action<bool> IsUIOperate { get; set; }
    #endregion

    #region property
    public static UnderLobbyManager Instance { get; private set; }
    public static bool IsFirstVisit { get; set; } = true;
    public LobbyUIState CurrentUIState { get => _currentUIState; set => _currentUIState = value; }
    /// <summary> ���o�����ǂ��� </summary>
    public bool IsDuring { get; private set; } = false;
    #endregion

    private void Awake()
    {
        Instance = this;

        if (_debugMode)
        {
            IsFirstVisit = false;
        }
    }

    IEnumerator Start()
    {
        yield return null;

        TransitionManager.FadeOut(FadeType.Normal);
        AudioManager.StopBGM();
        PlayerMove?.Invoke(false);
        IsUIOperate?.Invoke(false);

        if (IsFirstVisit)
        {
            yield return _underLobbyObjectTrans.DOLocalMoveY(0, _startStageAnimTime)
                                               .SetEase(Ease.Linear)
                                               .WaitForCompletion();
        }
        else
        {
            _underLobbyObjectTrans.DOLocalMoveY(0, 0f);
        }

        yield return new WaitForSeconds(1.5f);

        PlayerMove?.Invoke(true);
        IsUIOperate?.Invoke(true);
    }

    /// <summary>
    /// �X�e�[�W�̏ڍׂ�\������
    /// </summary>
    /// <param name="type"> �J�ڐ�̃X�e�[�W��Scene </param>
    public static void OnStageDescription(SceneType type)
    {
        Instance.OnFadeDescription(1f, 0.3f);
        Instance._isApproached = true;

        Instance.ApproachDoor?.Invoke();

        AudioManager.PlaySE(SEType.Lobby_NearDoor);
        StageDescriptionUI.Instance.ActiveDescription(type);
    }
    /// <summary>
    /// �X�e�[�W�̏ڍׂ��\������
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
}
