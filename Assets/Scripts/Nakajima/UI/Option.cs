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

    [Tooltip("難易度変更時の通知画面")]
    [SerializeField]
    CanvasGroup _changeDifficultyInfoGroup = default;

    [Tooltip("難易度変更時のText")]
    [SerializeField]
    Text _changeInfoText = default;

    [SerializeField]
    Image _bgmFillBarImage = default;

    [SerializeField]
    Transform _bgmCurrentPointImage = default;

    [SerializeField]
    Image _seFillBarImage = default;

    [SerializeField]
    Transform _seCurrentPointImage = default;

    [Header("Components")]
    [SerializeField]
    Pause _pause = default;
    #endregion

    #region private
    CanvasGroup _optionGroup;
    ReactiveProperty<TabType> _currentTab = new ReactiveProperty<TabType>();
    bool _init = false;
    bool _isPressed = false;
    bool _isBgmVolumeChanging = false;
    bool _isSeVolumeChanging = false;
    float _beforeBgmVolume;
    float _beforeSeVolume;
    #endregion

    #region public
    public event Action OnBackToMainMenu;
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
                    if (_init)
                    {
                        EventSystem.current.firstSelectedGameObject = _soundTabButtons[0].Button.gameObject;
                        _soundTabButtons[0].Button.Select();
                    }
                    break;
                case TabType.Difficulty:
                    _soundTab.SetActive(false);
                    _difficultyTab.SetActive(true);
                    if (_init)
                    {
                        EventSystem.current.firstSelectedGameObject = _difficultyTabButtons[0].Button.gameObject;
                        _difficultyTabButtons[0].Button.Select();
                    }
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
        _init = true;
    }

    public void ActiveOption()
    {
        StartCoroutine(OnOptionCotoutine());
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
        soundDeselectEntry3.eventID = EventTriggerType.Deselect;
        soundDeselectEntry3.callback.AddListener(eventData => OnDeselectEvent(_soundTabButtons[2]));
        soundTrigger3.triggers.Add(soundDeselectEntry3);

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
            if (!_isPressed)
            {
                _isPressed = true;
                StartCoroutine(ChangeDifficultyCoroutine(DifficultyType.Easy));
            }
        });

        _difficultyTabButtons[1].Button.onClick.AddListener(() =>
        {
            if (!_isPressed)
            {
                _isPressed = true;
                StartCoroutine(ChangeDifficultyCoroutine(DifficultyType.Normal));
            }
        });

        _difficultyTabButtons[2].Button.onClick.AddListener(() =>
        {
            if (!_isPressed)
            {
                _isPressed = true;
                StartCoroutine(ChangeDifficultyCoroutine(DifficultyType.Hard));
            }
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
        StartCoroutine(OffOptionCotoutine());
    }

    void OnChangeLeftBar(SelectBarType type)
    {
        switch (type)
        {
            case SelectBarType.BGM:
                if (AudioManager.Instance.CurrentBGMVolume <= 0 || _isBgmVolumeChanging)
                {
                    print($"現在のBGM音量:{AudioManager.Instance.CurrentBGMVolume}");
                    return;
                }
                _isBgmVolumeChanging = true;
                var bgmVol = --AudioManager.Instance.CurrentBGMVolume; //AudioManagerのプロパティを変更しつつ変数に代入する記述
                SetUIVolumeUI(type, bgmVol);
                AudioManager.BgmVolChange(bgmVol / 10.0f);
                break;
            case SelectBarType.SE:
                if (AudioManager.Instance.CurrentSEVolume <= 0 || _isSeVolumeChanging)
                {
                    print($"現在のSE音量:{AudioManager.Instance.CurrentSEVolume}");
                    return;
                }
                _isSeVolumeChanging = true;
                var seVol = --AudioManager.Instance.CurrentSEVolume;
                SetUIVolumeUI(type, seVol);
                AudioManager.SeVolChange(seVol / 10.0f);
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
                if (AudioManager.Instance.CurrentBGMVolume >= 10)
                {
                    print($"現在のBGM音量:{AudioManager.Instance.CurrentBGMVolume}");
                    return;
                }
                _isBgmVolumeChanging = true;
                var bgmVol = ++AudioManager.Instance.CurrentBGMVolume;
                SetUIVolumeUI(type, bgmVol);
                AudioManager.BgmVolChange(bgmVol / 10f);
                break;
            case SelectBarType.SE:
                if (AudioManager.Instance.CurrentSEVolume >= 10)
                {
                    print($"現在のSE音量:{AudioManager.Instance.CurrentSEVolume}");
                    return;
                }
                _isSeVolumeChanging = true;
                var seVol = ++AudioManager.Instance.CurrentSEVolume;
                SetUIVolumeUI(type, seVol);
                AudioManager.SeVolChange(seVol / 10.0f);
                break;
            default:
                break;
        }
    }

    void SetUIVolumeUI(SelectBarType type, float volume, float animTime = 0.15f)
    {
        switch (type)
        {
            case SelectBarType.BGM:
                _bgmFillBarImage.DOFillAmount(volume / 10.0f, animTime);
                _bgmCurrentPointImage.DOLocalMoveX(560 * (volume / 10.0f), animTime)
                                     .OnComplete(() => 
                                     {
                                         _isBgmVolumeChanging = false;
                                         print($"BGM音量:{volume}, FillImage.Fill:{_bgmFillBarImage.fillAmount}, Point.x:{_bgmCurrentPointImage.localPosition.x}");
                                     });
                break;
            case SelectBarType.SE:
                AudioManager.PlaySE(SEType.UI_Select);
                _seFillBarImage.DOFillAmount(volume / 10.0f, animTime);
                _seCurrentPointImage.DOLocalMoveX(560 * (volume / 10), animTime)
                                    .OnComplete(() =>
                                    {
                                        _isSeVolumeChanging = false;
                                        print($"SE音量:{volume}, FillImage.Fill:{_bgmFillBarImage.fillAmount}, Point.x:{_bgmCurrentPointImage.localPosition.x}");
                                    }); ;
                break;
            default:
                break;
        }
    }
    void RxSetup()
    {
        //オプションを閉じる
        this.UpdateAsObservable()
            .Where(_ => IsActived && UIInput.A && !_isPressed)
            .ThrottleFirst(TimeSpan.FromSeconds(0.15f))
            .Subscribe(_ =>
            {
                OnCloseOptionPanel();
            })
            .AddTo(this);

        #region Change Tab Action
        this.UpdateAsObservable()
            .Where(_ => IsActived &&
                        _currentTab.Value == TabType.Sound &&
                        UIInput.RB)
            .ThrottleFirst(TimeSpan.FromSeconds(0.15f))
            .Subscribe(_ =>
            {
                _currentTab.Value = TabType.Difficulty;

                foreach (var b in _soundTabButtons)
                {
                    b.ButtonImage.sprite = b.ButtonData.DeselectSprite;
                }
            })
            .AddTo(this);

        this.UpdateAsObservable()
            .Where(_ => IsActived &&
                        _currentTab.Value == TabType.Difficulty &&
                        UIInput.LB)
            .ThrottleFirst(TimeSpan.FromSeconds(0.15f))
            .Subscribe(_ =>
            {
                _currentTab.Value = TabType.Sound;

                foreach (var b in _difficultyTabButtons)
                {
                    b.ButtonImage.sprite = b.ButtonData.DeselectSprite;
                }
            })
            .AddTo(this);
        #endregion

        #region BGMBarAction
        this.UpdateAsObservable()
            .Where(_ =>
                   IsActived &&
                   _currentBarType == SelectBarType.BGM &&
                   UIInput.LeftCrossKey)
            .ThrottleFirst(TimeSpan.FromSeconds(0.15f))
            .Subscribe(_ =>
            {
                OnChangeLeftBar(SelectBarType.BGM);
            })
            .AddTo(this);

        this.UpdateAsObservable()
            .Where(_ =>
                   IsActived &&
                   _currentBarType == SelectBarType.BGM &&
                   UIInput.RightCrossKey)
            .ThrottleFirst(TimeSpan.FromSeconds(0.15f))
            .Subscribe(_ =>
            {
                OnChangeRightBar(SelectBarType.BGM);
            })
            .AddTo(this);
        #endregion

        #region SEBarAction
        this.UpdateAsObservable()
            .Where(_ =>
                   IsActived &&
                   _currentBarType == SelectBarType.SE &&
                   UIInput.LeftCrossKey)
            .ThrottleFirst(TimeSpan.FromSeconds(0.15f))
            .Subscribe(_ =>
            {
                OnChangeLeftBar(SelectBarType.SE);
            })
            .AddTo(this);

        this.UpdateAsObservable()
            .Where(_ =>
                   IsActived &&
                   _currentBarType == SelectBarType.SE &&
                   UIInput.RightCrossKey)
            .ThrottleFirst(TimeSpan.FromSeconds(0.15f))
            .Subscribe(_ =>
            {
                OnChangeRightBar(SelectBarType.SE);
            })
            .AddTo(this);
        #endregion
    }
    IEnumerator OnOptionCotoutine()
    {
        yield return new WaitForSeconds(0.03f);

        _optionGroup.alpha = 1;
        if (GameManager.Instance.CurrentScene != SceneType.Title)
        {
            UIManager.ActivatePanel(UIPanelType.Option);
        }
        _currentTab.Value = TabType.Sound;
        EventSystem.current.firstSelectedGameObject = _soundTabButtons[0].Button.gameObject;
        _soundTabButtons[0].Button.Select();
        SetUIVolumeUI(SelectBarType.BGM, AudioManager.Instance.CurrentBGMVolume, 0);
        SetUIVolumeUI(SelectBarType.SE, AudioManager.Instance.CurrentSEVolume, 0);
        _beforeBgmVolume = AudioManager.Instance.CurrentBGMVolume;
        _beforeSeVolume = AudioManager.Instance.CurrentSEVolume;
    }

    IEnumerator OffOptionCotoutine()
    {
        yield return new WaitForSeconds(0.1f);

        _isPressed = false;
        _optionGroup.alpha = 0;
        _changeDifficultyInfoGroup.alpha = 0;

        if (GameManager.Instance.CurrentScene != SceneType.Title)
        {
            _pause.PauseActivate(true);
            UIManager.InactivatePanel(UIPanelType.Option);
        }
        else
        {
            OnBackToMainMenu?.Invoke();
        }
    }

    IEnumerator ChangeDifficultyCoroutine(DifficultyType type)
    {
        GameManager.ChangeGameDifficult(type);

        switch (type)
        {
            case DifficultyType.Easy:
                _changeInfoText.text = "難易度を「かんたん」に変更しました";
                break;
            case DifficultyType.Normal:
                _changeInfoText.text = "難易度を「ふつう」に変更しました";
                break;
            case DifficultyType.Hard:
                _changeInfoText.text = "難易度を「むずかしい」に変更しました";
                break;
            default:
                break;
        }

        _changeDifficultyInfoGroup.alpha = 1;
        yield return new WaitForSeconds(2.0f);

        OnCloseOptionPanel();
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
