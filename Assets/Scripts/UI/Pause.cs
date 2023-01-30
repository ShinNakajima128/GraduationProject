using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UniRx;
using UniRx.Triggers;

/// <summary>
/// �|�[�Y��ʂ̋@�\�����R���|�[�l���g
/// </summary>
public class Pause : MonoBehaviour
{
    #region serialize
    [Header("Variables")]
    [Tooltip("���݂�Scene�̎��")]
    [SerializeField]
    SceneType _sceneType = default;

    [Header("UIObjects")]
    [SerializeField]
    Button[] _pauseButtons = default;

    [Header("Components")]
    [SerializeField]
    Option _option = default;
    #endregion

    #region private
    CanvasGroup _pauseGroup = default;
    bool _isPressed = false;
    #endregion

    #region public
    public event Action SubmitOptionAction;
    #endregion

    #region property
    public static Pause Instance { get; private set; }
    public bool IsActived => _pauseGroup.alpha == 1;
    public Button FirstSelectButton => _pauseButtons[0];
    #endregion

    private void Awake()
    {
        Instance = this;
        TryGetComponent(out _pauseGroup);
    }
    private void Start()
    {
        Setup();
        ButtonSetup();
    }

    void Setup()
    {
        this.UpdateAsObservable()
            .Where(_ => !IsActived &&
                        UIInput.Option &&
                        UIManager.Instance.IsCanOpenUI &&
                        !UIManager.Instance.IsAnyPanelOpened)
            .Subscribe(_ =>
            {
                if (GameManager.Instance.CurrentLobbyState == LobbyState.Default)
                {
                    if (LobbyManager.Instance.IsDuring)
                    {
                        return;
                    }
                }
                else
                {
                    if (UnderLobbyManager.Instance.IsDuring)
                    {
                        return;
                    }
                }
                StartCoroutine(ActivateCoroutine(true));
            })
            .AddTo(this);

        this.UpdateAsObservable()
            .Where(_ => IsActived && 
                        UIManager.Instance.IsCanOpenUI && 
                        !_option.IsActived)
            .Where(_ => UIInput.Option || UIInput.A)
            .Subscribe(_ =>
            {
                #region IsLobbyDuringJudge
                if (GameManager.Instance.CurrentLobbyState == LobbyState.Default)
                {
                    if (LobbyManager.Instance.IsDuring)
                    {
                        return;
                    }
                }
                else
                {
                    if (UnderLobbyManager.Instance.IsDuring)
                    {
                        return;
                    }
                }
                #endregion
                StartCoroutine(ActivateCoroutine(false));
            })
            .AddTo(this);

        _pauseGroup.alpha = 0;

        foreach (var b in _pauseButtons)
        {
            b.gameObject.SetActive(false);
        }

        _pauseButtons[0].gameObject.SetActive(true);
        _pauseButtons[1].gameObject.SetActive(true);

        switch (_sceneType)
        {
            case SceneType.Lobby:
                _pauseButtons[4].gameObject.SetActive(true);
                break;
            case SceneType.UnderLobby:
                _pauseButtons[2].gameObject.SetActive(true);
                _pauseButtons[4].gameObject.SetActive(true);
                break;
            case SceneType.Stage_Boss:
                _pauseButtons[3].gameObject.SetActive(true);
                break;
            default:
                _pauseButtons[2].gameObject.SetActive(true);
                break;
        }
        ButtonCursor.MoveCursor(new Vector3(_pauseButtons[0].gameObject.transform.position.x + 2.5f, 
                                            _pauseButtons[0].gameObject.transform.position.y + 5f, 
                                            _pauseButtons[0].gameObject.transform.position.z), 
                                            _pauseButtons[0].gameObject.transform);
    }

    void ButtonSetup()
    {
        //�u�Q�[���ɂ��ǂ�v�{�^����I�����̃A�N�V����
        _pauseButtons[0].onClick.AddListener(() =>
        {
            StartCoroutine(ActivateCoroutine(false));
            Debug.Log("�Q�[���ɖ߂�");
        });

        //�u�I�v�V�����v�{�^����I�����̃A�N�V����
        _pauseButtons[1].onClick.AddListener(() =>
        {
            _option.ActiveOption();
            PauseActivate(false);
        });

        //�u�L���̊Ԃɂ��ǂ�v�{�^����I�����̃A�N�V����
        _pauseButtons[2].onClick.AddListener(() =>
        {
            TransitionManager.SceneTransition(SceneType.Lobby, FadeType.Mask_KeyHole);
        });

        //�u�Y�p�̊Ԃɂ��ǂ�v�{�^����I�����̃A�N�V����
        _pauseButtons[3].onClick.AddListener(() =>
        {
            TransitionManager.SceneTransition(SceneType.UnderLobby, FadeType.Mask_KeyHole);
        });

        //�u�^�C�g���ɂ��ǂ�v�{�^����I�����̃A�N�V����
        _pauseButtons[4].onClick.AddListener(() =>
        {
            TransitionManager.SceneTransition(SceneType.Title, FadeType.Mask_KeyHole);
        });
    }

    IEnumerator ActivateCoroutine(bool isActivate)
    {
        _isPressed = true;

        yield return new WaitForSeconds(0.1f);

        _isPressed = false;
 
        if (isActivate)
        {
            if (GameManager.Instance.CurrentLobbyState == LobbyState.Default)
            {
                LobbyManager.Instance.PlayerMove?.Invoke(false);
            }
            else
            {
                UnderLobbyManager.Instance.PlayerMove?.Invoke(false);
            }
            PauseActivate(true);
        }
        else
        {
            if (GameManager.Instance.CurrentLobbyState == LobbyState.Default)
            {
                LobbyManager.Instance.PlayerMove?.Invoke(true);
            }
            else
            {
                UnderLobbyManager.Instance.PlayerMove?.Invoke(true);
            }
            
            PauseActivate(false);

            if (StageDescriptionUI.Instance.IsActived)
            {
                StageDescriptionUI.Instance.ActiveButton();
            }
        }
    }

    public void PauseActivate(bool isActivate)
    {
        if (isActivate)
        {
            UIManager.ActivatePanel(UIPanelType.Pause);
            _pauseGroup.alpha = 1;
            EventSystem.current.firstSelectedGameObject = _pauseButtons[0].gameObject;
            _pauseButtons[0].Select();
            print("�|�[�Y���j���[ON");
        }
        else
        {
            UIManager.InactivatePanel(UIPanelType.Pause);
            _pauseGroup.alpha = 0;
            print("�|�[�Y���j���[OFF");
        }
    }
}

public enum PauseButtonType
{
    ReturnGame,
    Option,
    Return_Lobby,
    Return_UnderLobby,
    Return_Title
}
