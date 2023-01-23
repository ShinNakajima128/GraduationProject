using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UniRx;
using UniRx.Triggers;

public class Option : MonoBehaviour
{
    #region serialize
    [SerializeField]
    TabType _currentTabType = default;

    [Header("UIObjects")]
    [Tooltip("音量設定のタブ")]
    [SerializeField]
    GameObject _soundTab = default;

    [Tooltip("難易度設定のタブ")]
    [SerializeField]
    GameObject _difficultyTab = default;

    [SerializeField]
    Button _soundTabFirstSelect = default;

    [SerializeField]
    Button _difficultyFirstSelect = default;

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
                    _soundTabFirstSelect.Select();
                    break;
                case TabType.Difficulty:
                    _soundTab.SetActive(false);
                    _difficultyTab.SetActive(true);
                    _difficultyFirstSelect.Select();
                    break;
                default:
                    break;
            }
        })
        .AddTo(this);
    }

    private void Start()
    {
        _currentTab.Value = _currentTabType;
        
        this.UpdateAsObservable()
            .Where(_ => IsActived && UIInput.A)
            .Subscribe(_ => 
            {
                UIManager.InactivatePanel(UIPanelType.Option);
                EventSystem.current.firstSelectedGameObject = _pause.FirstSelectButton.gameObject;
                _pause.FirstSelectButton.Select();
            });
    }

    public void ActiveOption()
    {
        _optionGroup.alpha = 1;
        UIManager.ActivatePanel(UIPanelType.Option);
    }

    void ButtonSetup()
    {

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
