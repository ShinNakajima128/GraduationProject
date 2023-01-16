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

    [Tooltip("�n���֌������ۂ̃v���C���[�̈ʒu")]
    [SerializeField]
    Transform _goingUnderTrans = default;

    [Tooltip("���o�p�̃`�F�V���L")]
    [SerializeField]
    GameObject[] _cheshireCats = default;

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
    public LobbyUIState CurrentUIState { get => _currentUIState; set => _currentUIState = value; }
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
            if (!IsFirstArrival)
            {
                AudioManager.PlayBGM(BGMType.Lobby);
            }
            else
            {
                AudioManager.StopBGM(1.0f);
                EventManager.ListenEvents(Events.Lobby_MeetingCheshire, PlayMeetingBGM);
                EventManager.ListenEvents(Events.Lobby_Introduction, () => StartCoroutine(_directionCameraMng.StartDirectionCoroutine(CameraDirectionType.Lobby_Introduction)));
                EventManager.ListenEvents(Events.Alice_Surprised, () => StartCoroutine(_directionCameraMng.StartDirectionCoroutine(CameraDirectionType.Lobby_AliceAndCheshireTalking)));
                EventManager.ListenEvents(Events.Cheshire_StartGrinning, () => 
                {
                    _directionCameraMng.ResetCamera();
                });
            }

            TransitionManager.FadeIn(FadeType.Normal, 0f);
            yield return null;

            _provider.enabled = false;
            PlayerMove?.Invoke(false);

            yield return new WaitForSeconds(1.5f);

            //���r�[�ɏ��߂ė����Ƃ��̓X�g�[���[���b�Z�[�W���Đ�
            if (IsFirstArrival)
            {
                yield return _messagePlayer.PlayMessageCorountine(MessageType.Stage1_End);

                TransitionManager.FadeIn(FadeType.White_default, action: () =>
                {
                    TransitionManager.FadeOut(FadeType.Normal);
                });

                yield return new WaitForSeconds(1.5f);

                yield return _directionCameraMng.StartDirectionCoroutine(CameraDirectionType.Lobby_FirstVisit);

                TransitionManager.FadeIn(FadeType.Black_TransParent, 0f);
                TransitionManager.FadeIn(FadeType.Black_default, action: () =>
                {
                    _directionCameraMng.ResetCamera(CameraDirectionType.Lobby_FirstVisit, 0f);
                    StartCoroutine(_directionCameraMng.StartDirectionCoroutine(CameraDirectionType.Lobby_Alice_Front));
                    TransitionManager.FadeOut(FadeType.Normal);
                });

                yield return new WaitForSeconds(3.0f);

                yield return _messagePlayer.PlayMessageCorountine(MessageType.Lobby_Visit, () =>
                {
                    //�`�F�V���L�o�ꉉ�o
                    TransitionManager.FadeIn(FadeType.Black_TransParent, 0f);
                    TransitionManager.FadeIn(FadeType.Mask_CheshireCat, action: () =>
                    {
                        _directionCameraMng.ResetCamera(CameraDirectionType.Lobby_Alice_Front, 0f);
                        LetterboxController.ActivateLetterbox(true, 0f);
                        LobbyCheshireCatManager.Instance.ActiveCheshireCat(LobbyCheshireCatType.Appearance);
                        TransitionManager.FadeOut(FadeType.Normal);
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
                    //�`�F�V���L���f�B�]���u�Ŕ�\���ɂ���
                    LobbyCheshireCatManager.Instance.MovableCat.ActivateDissolve(true);
                });

                _directionCameraMng.ResetBlendTime();

                yield return new WaitForSeconds(2.0f);

                //���v�̉��o
                ClockDirection();
                AudioManager.StopBGM(1.5f); //��U�Ȃ��~�߂�
            }
            else
            {
                TransitionManager.FadeOut(FadeType.Normal);

                //��ʃt�F�[�h�I����ҋ@
                yield return new WaitForSeconds(1.5f);

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
            AudioManager.PlayBGM(BGMType.Lobby);
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
        LetterboxController.ActivateLetterbox(true);

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
                _cheshireCatCamera.Priority = 10;
                _clockCamera.Priority = 10;
                Camera.main.LayerCullingToggle("Ornament", true);
                StartCoroutine(OnPlayerMovable(3.0f));
            }
        });
    }

    /// <summary>
    /// �`�F�V���L�ƑΖʂ�������BGM���Đ�
    /// </summary>
    void PlayMeetingBGM()
    {
        AudioManager.PlayBGM(BGMType.Lobby_MeetingCheshire);
    }

    /// <summary>
    /// ���r�[�̏��񓞒B�t���O�����Z�b�g
    /// </summary>
    public static void Reset()
    {
        IsFirstArrival = true;
    }

    /// <summary>
    /// 12���ɂȂ������̎��v�̔��������̃R���[�`��
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
    /// �v���C���[�𑀍�\�ɂ���
    /// </summary>
    /// <param name="Interval"> �\�ɂȂ�܂ł̎��� </param>
    /// <returns></returns>
    IEnumerator OnPlayerMovable(float Interval)
    {
        yield return new WaitForSeconds(Interval - 1);

        if (IsFirstArrival)
        {
            AudioManager.PlayBGM(BGMType.Lobby);
            IsFirstArrival = false;
        }

        LetterboxController.ActivateLetterbox(false);

        yield return new WaitForSeconds(1.0f);

        PlayerMove?.Invoke(true);
        _provider.enabled = true;
    }

    /// <summary>
    /// �{�X��֐i�ރR���[�`��
    /// </summary>
    IEnumerator OnBossStageaAppearCoroutine()
    {
        StartCoroutine(OnHandsEmission());
        EffectManager.PlayEffect(EffectType.Heart, _heartEffectTrans.position);

        yield return new WaitForSeconds(2.0f);

        TransitionManager.FadeIn(FadeType.Normal, 0.5f, () =>
         {
             _playerTrans.localPosition = _goingUnderTrans.position;
             _brain.m_DefaultBlend.m_Time = 0;
             _clock_ShakeCamera.Priority = 20;

             TransitionManager.FadeOut(FadeType.Normal, 0.5f);
         });


        yield return new WaitForSeconds(3.5f);

        _goingUnderCamera.Priority = 25;

        float timer = 0;
        bool isFading = false;

        yield return _mainStage.DOLocalMoveY(2f, 10f)
                               .OnUpdate(() =>
                               {
                                   timer += Time.deltaTime;

                                   if (timer >= 5.0f && !isFading)
                                   {
                                       TransitionManager.SceneTransition(SceneType.Stage_Boss, FadeType.Mask_Heart);
                                       isFading = true;
                                   }
                               })
                               .SetLink(_mainStage.gameObject)
                               .WaitForCompletion();

        Debug.Log("�{�X�X�e�[�W�o��");
        //Camera.main.LayerCullingToggle("Ornament", true);
        //_clockCamera.Priority = 10;
        //StartCoroutine(OnPlayerMovable(3.0f));
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

