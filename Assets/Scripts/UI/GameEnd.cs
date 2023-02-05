using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UniRx;
using UniRx.Triggers;


public class GameEnd : MonoBehaviour
{
    #region serialize
    [Header("UIObjects")]
    [SerializeField]
    Button[] _gameEndButtons = default;

    [SerializeField]
    Transform _cursorImageTrans = default;

    [SerializeField]
    Transform[] _cursorPosTrans = default;

    [SerializeField]
    Text _endGameInfo = default;
    #endregion

    #region private
    CanvasGroup _gameEndGroup = default;
    bool _isPressed = false;
    SceneType _currentScene;
    SceneType _destinationScene;
    #endregion

    #region public
    #endregion

    #region property
    public bool IsActived => _gameEndGroup.alpha == 1;
    #endregion

    private void Awake()
    {
        TryGetComponent(out _gameEndGroup);
        Setup();
        ButtonSetup();
    }

    private void Start()
    {
        #region UniRx Suvscribe
        this.UpdateAsObservable()
            .Where(_ => !IsActived &&
                        UIInput.Exit &&
                        UIManager.Instance.IsCanOpenUI &&
                        !UIManager.Instance.IsAnyPanelOpened)
            .ThrottleFirst(TimeSpan.FromMilliseconds(1000))
            .Subscribe(_ =>
            {
                if (GameManager.Instance.CurrentScene == SceneType.Lobby ||
                    GameManager.Instance.CurrentScene == SceneType.UnderLobby)
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
                }
                StartCoroutine(ActivateCoroutine(true));
            })
            .AddTo(this);

        //画面を閉じる処理を登録
        this.UpdateAsObservable()
            .Where(_ => IsActived &&
                        UIManager.Instance.IsCanOpenUI)
            .Where(_ => UIInput.Exit || UIInput.A)
            .ThrottleFirst(TimeSpan.FromMilliseconds(1000))
            .Subscribe(_ =>
            {
                StartCoroutine(ActivateCoroutine(false));
            });
        #endregion

        switch (GameManager.Instance.CurrentScene)
        {
            case SceneType.Title:
                _endGameInfo.text = "ゲームを終了しますか？";
                _currentScene = SceneType.Title;
                break;
            case SceneType.Lobby:
                _endGameInfo.text = "ゲームを終了しますか？";
                _currentScene = SceneType.Lobby;
                _destinationScene = SceneType.Title;
                break;
            case SceneType.UnderLobby:
                _endGameInfo.text = "ゲームを終了しますか？";
                _currentScene = SceneType.UnderLobby;
                _destinationScene = SceneType.Title;
                break;
            case SceneType.Stage1_Fall:
                if (!FallGameManager.IsSecondTry)
                {
                    _endGameInfo.text = "ゲームを終了しますか？";
                    _currentScene = SceneType.Stage1_Fall;
                    _destinationScene = SceneType.Title;
                }
                else
                {
                    _endGameInfo.text = "記憶の間にもどりますか？";
                    _currentScene = SceneType.Stage1_Fall;
                    _destinationScene = SceneType.Lobby;
                }
                break;
            case SceneType.RE_Stage2:
                _endGameInfo.text = "記憶の間にもどりますか？";
                _currentScene = SceneType.RE_Stage2;
                _destinationScene = SceneType.Lobby;
                break;
            case SceneType.RE_Stage3:
                _endGameInfo.text = "記憶の間にもどりますか？";
                _currentScene = SceneType.RE_Stage3;
                _destinationScene = SceneType.Lobby;
                break;
            case SceneType.Stage4:
                _endGameInfo.text = "記憶の間にもどりますか？";
                _currentScene = SceneType.Stage4;
                _destinationScene = SceneType.Lobby;
                break;
            case SceneType.Stage_Boss:
                _endGameInfo.text = "忘却の間にもどりますか？";
                _currentScene = SceneType.Stage_Boss;
                _destinationScene = SceneType.Stage_Boss;
                break;
            default:
                break;
        }
    }

    void Setup()
    {
        var trigger1 = _gameEndButtons[0].gameObject.GetComponent<EventTrigger>();
        //ボタンにカーソルが合った時のイベントを登録
        var selectEntry1 = new EventTrigger.Entry();
        selectEntry1.eventID = EventTriggerType.Select;
        selectEntry1.callback.AddListener(eventData => OnSelectEvent(0));
        trigger1.triggers.Add(selectEntry1);

        var deselectEntry1 = new EventTrigger.Entry();
        deselectEntry1.eventID = EventTriggerType.Deselect;
        deselectEntry1.callback.AddListener(eventData => OnDeselectEvent());
        trigger1.triggers.Add(deselectEntry1);


        var trigger2 = _gameEndButtons[1].gameObject.GetComponent<EventTrigger>();
        //ボタンにカーソルが合った時のイベントを登録
        var selectEntry2 = new EventTrigger.Entry();
        selectEntry2.eventID = EventTriggerType.Select;
        selectEntry2.callback.AddListener(eventData => OnSelectEvent(1));
        trigger2.triggers.Add(selectEntry2);

        var deselectEntry2 = new EventTrigger.Entry();
        deselectEntry2.eventID = EventTriggerType.Deselect;
        deselectEntry2.callback.AddListener(eventData => OnDeselectEvent());
        trigger2.triggers.Add(deselectEntry2);
    }

    void ButtonSetup()
    {
        _gameEndButtons[0].OnClickAsObservable()
                          .ThrottleFirst(TimeSpan.FromMilliseconds(1000))
                          .Subscribe(_ =>
                          {
                              StartCoroutine(ActivateCoroutine(false));
                          });

        _gameEndButtons[1].OnClickAsObservable()
                          .Take(1)
                          .Subscribe(_ =>
                          {
                              Time.timeScale = 1;
                              TransitionManager.SceneTransition(SceneType.Title, FadeType.Mask_KeyHole);
                              AudioManager.StopBGM();
                              AudioManager.PlaySE(SEType.GoToStage);
                          });
    }

    void OnSelectEvent(int index)
    {
        //_cursorImageTrans.SetParent(_gameEndButtons[index].transform);
        //_cursorImageTrans.localPosition = _cursorPosTrans[index].localPosition;
        AudioManager.PlaySE(SEType.UI_CursolMove);
    }

    void OnDeselectEvent()
    {

    }

    IEnumerator ActivateCoroutine(bool isActivate)
    {
        _isPressed = true;

        yield return new WaitForSecondsRealtime(0.1f);

        _isPressed = false;

        if (isActivate)
        {
            Time.timeScale = 0;
            if (GameManager.Instance.CurrentScene == SceneType.Lobby ||
                GameManager.Instance.CurrentScene == SceneType.UnderLobby)
            {
                UIManager.ActivatePanel(UIPanelType.GameEnd);

                if (GameManager.Instance.CurrentLobbyState == LobbyState.Default)
                {
                    LobbyManager.Instance.PlayerMove?.Invoke(false);
                }
                else
                {
                    UnderLobbyManager.Instance.PlayerMove?.Invoke(false);
                }
            }
            _gameEndGroup.alpha = 1;
            EventSystem.current.firstSelectedGameObject = _gameEndButtons[0].gameObject;
            _gameEndButtons[0].Select();
            print("ゲーム終了画面ON");
        }
        else
        {
            Time.timeScale = 1;
            if (GameManager.Instance.CurrentScene == SceneType.Lobby ||
                GameManager.Instance.CurrentScene == SceneType.UnderLobby)
            {
                UIManager.InactivatePanel(UIPanelType.GameEnd);

                if (GameManager.Instance.CurrentLobbyState == LobbyState.Default)
                {
                    LobbyManager.Instance.PlayerMove?.Invoke(true);
                }
                else
                {
                    UnderLobbyManager.Instance.PlayerMove?.Invoke(true);
                }

                if (StageDescriptionUI.Instance.IsActived)
                {
                    StageDescriptionUI.Instance.ActiveButton();
                }
            }
            else
            {

            }
            _gameEndGroup.alpha = 0;
            AudioManager.PlaySE(SEType.UI_CursolMove);
            print("ゲーム終了画面OFF");
        }
    }
}
