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
        SetPlayerPosition(GameManager.Instance.CurrentStage); //�v���C���[�ʒu���v���C�����~�j�Q�[���̃h�A�̑O�Ɉړ�
        _clockCtrl.ChangeClockState(GameManager.Instance.CurrentClockState, 0f, 0f); //���v�̏�Ԃ��I�u�W�F�N�g�ɔ��f

        if (!_debugMode)
        {
            yield return null;

            _provider.enabled = false;
            PlayerMove?.Invoke(false);

            yield return new WaitForSeconds(1.5f);

            //���r�[�ɏ��߂ė����Ƃ��̓X�g�[���[���b�Z�[�W���Đ�
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
                //���N���A���X�e�[�W���N���A���Ă���ꍇ�͎��v�̉��o���s��
                if (!GameManager.CheckStageStatus() && GameManager.IsClearStaged)
                {
                    ClockDirection();
                }
                else
                {
                    StartCoroutine(OnPlayerMovable(1.5f));
                    Debug.Log("�N���A�ς݃X�e�[�W");
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
    /// �X�e�[�W�̏ڍׂ�\������
    /// </summary>
    /// <param name="type"> �J�ڐ�̃X�e�[�W��Scene </param>
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

    /// <summary>
    /// �v���C���[�̈ʒu���h�A�̑O�Ɉړ�
    /// </summary>
    /// <param name="type"> Stage�̎�� </param>
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
    /// �J�����A���v�̉��o
    /// </summary>
    void ClockDirection()
    {
        _clockCamera.Priority = 15; //���o�p�J�������A�N�e�B�u��
        Camera.main.LayerCullingToggle("Ornament", false); //���r�[�̃��C�g�Ȃǂ̑����i���\���ɂ���

        _clockCtrl.ChangeClockState(GameManager.CheckGameStatus(), action: () =>
        {
            GameManager.UpdateStageStatus(GameManager.Instance.CurrentStage);

            if (GameManager.Instance.CurrentClockState == ClockState.Twelve)
            {
                //�{�X�X�e�[�W���o�����鏈��
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
    /// �v���C���[�𑀍�\�ɂ���
    /// </summary>
    /// <param name="Interval"> �\�ɂȂ�܂ł̎��� </param>
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
        
        Debug.Log("�{�X�X�e�[�W�o��");
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

