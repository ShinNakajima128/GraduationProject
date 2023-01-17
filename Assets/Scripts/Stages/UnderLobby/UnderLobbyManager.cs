using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UnderLobbyManager : MonoBehaviour
{
    #region serialize
    [SerializeField]
    Stage _bossStageData = default;

    [Header("UI")]
    [SerializeField]
    GameObject _stageDescriptionPanel = default;

    [SerializeField]
    CanvasGroup _stageDescriptionCanvas = default;

    [SerializeField]
    Text _stageNameText = default;

    [SerializeField]
    Image _StageImage = default;
    #endregion

    #region private
    bool _isApproached = false;
    #endregion

    #region public
    /// <summary> �h�A�ɋ߂Â�������Action </summary>
    public Action ApproachDoor { get; set; }
    /// <summary> �h�A���痣�ꂽ����Action </summary>
    public Action StepAwayDoor { get; set; }
    #endregion

    #region property
    public static UnderLobbyManager Instance { get; private set; }
    #endregion

    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// �X�e�[�W�̏ڍׂ�\������
    /// </summary>
    /// <param name="type"> �J�ڐ�̃X�e�[�W��Scene </param>
    public static void OnStageDescription()
    {
        Instance.OnFadeDescription(1f, 0.3f);
        Instance._isApproached = true;

        Instance._stageNameText.text = Instance._bossStageData.SceneName;
        Instance._StageImage.sprite = Instance._bossStageData.StageSprite;
        Instance.ApproachDoor?.Invoke();
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
