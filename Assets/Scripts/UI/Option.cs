using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UniRx;
using UniRx.Triggers;
using DG.Tweening;

public class Option : MonoBehaviour
{
    #region serialize
    [Header("Confirming")]
    [SerializeField]
    TabType _currentTabType = default;

    [SerializeField]
    SelectBarType _currentBarType = default;

    [Header("UIObjects")]
    [Tooltip("音量設定のタブ")]
    [SerializeField]
    GameObject _soundTab = default;

    [Tooltip("難易度設定のタブ")]
    [SerializeField]
    GameObject _difficultyTab = default;

    [Tooltip("音量設定タブの各ボタン")]
    [SerializeField]
    OptionButton[] _soundTabButtons = default;

    [Tooltip("難易度設定タブの各ボタン")]
    [SerializeField]
    OptionButton[] _difficultyTabButtons = default;

    [Header("Components")]
    [SerializeField]
    Pause _pause = default;
    #endregion

    #region private
    CanvasGroup _optionGroup;
    ReactiveProperty<TabType> _currentTab = new ReactiveProperty<TabType>();
    #endregion

    #region public
    #endregion

    #region property
    public CanvasGroup OptionGroup => _optionGroup;
    public bool IsActived => _optionGroup.alpha == 1;
    #endregion

    private void Awake()
    {
        TryGetComponent(out _optionGroup);

        _currentTab.Subscribe(value =>
        {
            switch (value)
            {
                case TabType.Sound:
                    _soundTab.SetActive(true);
                    _difficultyTab.SetActive(false);
                    EventSystem.current.firstSelectedGameObject = _soundTabButtons[0].Button.gameObject;
                    _soundTabButtons[0].Button.Select();
                    break;
                case TabType.Difficulty:
                    _soundTab.SetActive(false);
                    _difficultyTab.SetActive(true);
                    EventSystem.current.firstSelectedGameObject = _difficultyTabButtons[0].Button.gameObject;
                    _difficultyTabButtons[0].Button.Select();
                    break;
                default:
                    break;
            }
        })
        .AddTo(this);

        ButtonSetup();
    }

    private void Start()
    {
        RxSetup();
    }

    public void ActiveOption()
    {
        _optionGroup.alpha = 1;
        UIManager.ActivatePanel(UIPanelType.Option);
        _currentTab.Value = TabType.Sound;
        EventSystem.current.firstSelectedGameObject = _soundTabButtons[0].Button.gameObject;
    }

    void ButtonSetup()
    {
        #region SetSoundTabButtonSelectEvents
        //音量設定タブのボタンのセットアップ
        var soundTrigger1 = _soundTabButtons[0].Button.GetComponent<EventTrigger>();
        
        //ボタンにカーソルが合った時のイベントを登録
        var soundSelectEntry1 = new EventTrigger.Entry();
        soundSelectEntry1.eventID = EventTriggerType.Select;
        soundSelectEntry1.callback.AddListener(eventData => OnSoundTabButtonSelectEvent(SelectBarType.BGM, _soundTabButtons[0]));
        soundTrigger1.triggers.Add(soundSelectEntry1);

        var soundDeselectEntry1 = new EventTrigger.Entry();
        soundDeselectEntry1.eventID = EventTriggerType.Deselect;
        soundDeselectEntry1.callback.AddListener(eventData => OnDeselectEvent(_soundTabButtons[0]));
        soundTrigger1.triggers.Add(soundDeselectEntry1);

        var soundTrigger2 = _soundTabButtons[1].Button.GetComponent<EventTrigger>();

        //ボタンにカーソルが合った時のイベントを登録
        var soundSelectEntry2 = new EventTrigger.Entry();
        soundSelectEntry2.eventID = EventTriggerType.Select;
        soundSelectEntry2.callback.AddListener(eventData => OnSoundTabButtonSelectEvent(SelectBarType.SE, _soundTabButtons[1]));
        soundTrigger2.triggers.Add(soundSelectEntry2);

        var soundDeselectEntry2 = new EventTrigger.Entry();
        soundDeselectEntry2.eventID = EventTriggerType.Deselect;
        soundDeselectEntry2.callback.AddListener(eventData => OnDeselectEvent(_soundTabButtons[1]));
        soundTrigger2.triggers.Add(soundDeselectEntry2);

        var soundTrigger3 = _soundTabButtons[2].Button.GetComponent<EventTrigger>();

        //ボタンにカーソルが合った時のイベントを登録
        var soundSelectEntry3 = new EventTrigger.Entry();
        soundSelectEntry3.eventID = EventTriggerType.Select;
        soundSelectEntry3.callback.AddListener(eventData => OnSoundTabButtonSelectEvent(SelectBarType.Default, _soundTabButtons[2]));
        soundTrigger3.triggers.Add(soundSelectEntry3);

        var soundDeselectEntry3 = new EventTrigger.Entry();
        soundDeselectEntry1.eventID = EventTriggerType.Deselect;
        soundDeselectEntry1.callback.AddListener(eventData => OnDeselectEvent(_soundTabButtons[2]));
        soundTrigger1.triggers.Add(soundDeselectEntry1);

        var soundTrigger4 = _soundTabButtons[3].Button.GetComponent<EventTrigger>();

        //ボタンにカーソルが合った時のイベントを登録
        var soundSelectEntry4 = new EventTrigger.Entry();
        soundSelectEntry4.eventID = EventTriggerType.Select;
        soundSelectEntry4.callback.AddListener(eventData => OnSoundTabButtonSelectEvent(SelectBarType.Default, _soundTabButtons[3]));
        soundTrigger4.triggers.Add(soundSelectEntry4);

        var soundDeselectEntry4 = new EventTrigger.Entry();
        soundDeselectEntry4.eventID = EventTriggerType.Deselect;
        soundDeselectEntry4.callback.AddListener(eventData => OnDeselectEvent(_soundTabButtons[3]));
        soundTrigger4.triggers.Add(soundDeselectEntry4);

        var difficultTrigger1 = _difficultyTabButtons[0].Button.GetComponent<EventTrigger>();

        //ボタンにカーソルが合った時のイベントを登録
        var difficultySelectEntry1 = new EventTrigger.Entry();
        difficultySelectEntry1.eventID = EventTriggerType.Select;
        difficultySelectEntry1.callback.AddListener(eventData => OnSoundTabButtonSelectEvent(SelectBarType.Default, _difficultyTabButtons[0]));
        difficultTrigger1.triggers.Add(difficultySelectEntry1);

        var difficultyDeselectEntry1 = new EventTrigger.Entry();
        difficultyDeselectEntry1.eventID = EventTriggerType.Deselect;
        difficultyDeselectEntry1.callback.AddListener(eventData => OnDeselectEvent(_difficultyTabButtons[0]));
        difficultTrigger1.triggers.Add(difficultyDeselectEntry1);

        var difficultTrigger2 = _difficultyTabButtons[1].Button.GetComponent<EventTrigger>();

        //ボタンにカーソルが合った時のイベントを登録
        var difficultySelectEntry2 = new EventTrigger.Entry();
        difficultySelectEntry2.eventID = EventTriggerType.Select;
        difficultySelectEntry2.callback.AddListener(eventData => OnSoundTabButtonSelectEvent(SelectBarType.Default, _difficultyTabButtons[1]));
        difficultTrigger2.triggers.Add(difficultySelectEntry2);

        var difficultyDeselectEntry2 = new EventTrigger.Entry();
        difficultyDeselectEntry2.eventID = EventTriggerType.Deselect;
        difficultyDeselectEntry2.callback.AddListener(eventData => OnDeselectEvent(_difficultyTabButtons[1]));
        difficultTrigger2.triggers.Add(difficultyDeselectEntry2);

        var difficultTrigger3 = _difficultyTabButtons[2].Button.GetComponent<EventTrigger>();

        //ボタンにカーソルが合った時のイベントを登録
        var difficultySelectEntry3 = new EventTrigger.Entry();
        difficultySelectEntry3.eventID = EventTriggerType.Select;
        difficultySelectEntry3.callback.AddListener(eventData => OnSoundTabButtonSelectEvent(SelectBarType.Default, _difficultyTabButtons[2]));
        difficultTrigger3.triggers.Add(difficultySelectEntry3);

        var difficultyDeselectEntry3 = new EventTrigger.Entry();
        difficultyDeselectEntry3.eventID = EventTriggerType.Deselect;
        difficultyDeselectEntry3.callback.AddListener(eventData => OnDeselectEvent(_difficultyTabButtons[2]));
        difficultTrigger3.triggers.Add(difficultyDeselectEntry3);
        #endregion

        #region SetButtonSubmitEvents
        _soundTabButtons[2].Button.onClick.AddListener(() =>
        {
            OnCloseOptionPanel();
        });

        _soundTabButtons[3].Button.onClick.AddListener(() =>
        {
            OnCloseOptionPanel();
        });

        _difficultyTabButtons[0].Button.onClick.AddListener(() =>
        {
            OnCloseOptionPanel();
        });

        _difficultyTabButtons[1].Button.onClick.AddListener(() =>
        {
            OnCloseOptionPanel();
        });

        _difficultyTabButtons[2].Button.onClick.AddListener(() =>
        {
            OnCloseOptionPanel();
        });
        #endregion
    }

    /// <summary>
    /// 選択時のボタンのEvent
    /// </summary>
    /// /// <param name="type"> バーの種類 </param>
    /// <param name="button"> オプションボタン </param>
    void OnSoundTabButtonSelectEvent(SelectBarType type, OptionButton button)
    {
        _currentBarType = type;

        button.ButtonImage.sprite = button.ButtonData.SelectSprite;
    }

    /// <summary>
    /// 非選択時のボタンのEvent
    /// </summary>
    /// <param name="button"> オプションボタン </param>
    void OnDeselectEvent(OptionButton button)
    {
        button.ButtonImage.sprite = button.ButtonData.DeselectSprite;
    }

    void OnCloseOptionPanel()
    {
        _optionGroup.alpha = 0;
        UIManager.InactivatePanel(UIPanelType.Option);
        _pause.PauseActivate(true);
    }

    void OnChangeLeftBar(SelectBarType type)
    {
        switch (type)
        {
            case SelectBarType.BGM:
                break;
            case SelectBarType.SE:
                break;
            default:
                break;
        }
    }

    void OnChangeRightBar(SelectBarType type)
    {
        switch (type)
        {
            case SelectBarType.BGM:
                break;
            case SelectBarType.SE:
                break;
            default:
                break;
        }
    }

    void RxSetup()
    {
        //オプションを閉じる
        this.UpdateAsObservable()
            .Where(_ => IsActived && UIInput.A)
            .Subscribe(_ =>
            {
                OnCloseOptionPanel();
            });

        #region Change Tab Action
        this.UpdateAsObservable()
            .Where(_ => IsActived &&
                        _currentTab.Value == TabType.Sound &&
                        UIInput.RB)
            .Subscribe(_ =>
            {
                _currentTab.Value = TabType.Difficulty;

                foreach (var b in _soundTabButtons)
                {
                    b.ButtonImage.sprite = b.ButtonData.DeselectSprite;
                }
            });

        this.UpdateAsObservable()
            .Where(_ => IsActived &&
                        _currentTab.Value == TabType.Difficulty &&
                        UIInput.LB)
            .Subscribe(_ =>
            {
                _currentTab.Value = TabType.Sound;

                foreach (var b in _difficultyTabButtons)
                {
                    b.ButtonImage.sprite = b.ButtonData.DeselectSprite;
                }
            });
        #endregion

        #region BGMBarAction
        this.UpdateAsObservable()
            .Where(_ =>
                   IsActived &&
                   _currentBarType == SelectBarType.BGM &&
                   UIInput.LeftCrossKey)
            .Subscribe(_ =>
            {
                OnChangeLeftBar(SelectBarType.BGM);
            });

        this.UpdateAsObservable()
            .Where(_ =>
                   IsActived &&
                   _currentBarType == SelectBarType.BGM &&
                   UIInput.RightCrossKey)
            .Subscribe(_ =>
            {
                OnChangeRightBar(SelectBarType.BGM);
            });
        #endregion

        #region SEBarAction
        this.UpdateAsObservable()
            .Where(_ =>
                   IsActived &&
                   _currentBarType == SelectBarType.SE &&
                   UIInput.LeftCrossKey)
            .Subscribe(_ =>
            {
                OnChangeLeftBar(SelectBarType.SE);
            });

        this.UpdateAsObservable()
            .Where(_ =>
                   IsActived &&
                   _currentBarType == SelectBarType.SE &&
                   UIInput.RightCrossKey)
            .Subscribe(_ =>
            {
                OnChangeRightBar(SelectBarType.SE);
            });
        #endregion
    }
}

/// <summary>
/// タブの種類
/// </summary>
public enum TabType
{
    Sound,
    Difficulty
}

/// <summary>
/// 選択中の音量設定バーの種類
/// </summary>
public enum SelectBarType
{
    Default,
    BGM,
    SE
}

[Serializable]
public struct OptionButton
{
    public string ButtonName;
    public Button Button;
    public Image ButtonImage;
    public ButtonData ButtonData;
}
