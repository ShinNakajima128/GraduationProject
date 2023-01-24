using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UniRx;
using UniRx.Triggers;

/// <summary>
/// �^�C�g����ʂ̋@�\���Ǘ�����}�l�[�W���[
/// </summary>
public class TitleManager : MonoBehaviour
{
    #region serialize
    [Header("UI")]
    [Tooltip("�^�C�g����ʂ̊eUIPanel")]
    [SerializeField]
    CanvasGroup[] _titleUIGroups = default;

    [Tooltip("�^�C�g����Button")]
    [SerializeField]
    Button[] _titleButtons = default;

    [Tooltip("�^�C�g����ʂ̌��݂�UI�̎��")]
    [SerializeField]
    TitleUIType _currentTitleType = default;

    #endregion

    #region private
    Dictionary<ButtonType, Button> _buttonDic = new Dictionary<ButtonType, Button>();
    #endregion

    #region property
    public static TitleManager Instance { get; private set; }
    #endregion

    private void Awake()
    {
        Instance = this;

        for (int i = 0; i < _titleButtons.Length; i++)
        {
            _buttonDic.Add((ButtonType)i, _titleButtons[i]);
        }

        this.UpdateAsObservable().Where(_ => _currentTitleType != TitleUIType.Start)
                                 .Subscribe(_ => { });
    }

    IEnumerator Start()
    {
        TransitionManager.FadeIn(FadeType.Black_default, 0f);
        yield return new WaitForSeconds(1.5f);

        AudioManager.PlayBGM(BGMType.Title);
        TransitionManager.FadeOut(FadeType.Black_default, 2.0f);
        ButtonSetup();

        yield return null;

        ChangeUIPanel(TitleUIType.Start);
        GameManager.GameReset(); //�N���A���ă^�C�g���ɖ߂��Ă����ꍇ�p�̏���������
    }
    void ButtonSetup()
    {
        foreach (var b in _buttonDic)
        {
            switch (b.Key)
            {
                case ButtonType.GameStart:
                    b.Value.onClick.AddListener(() =>
                    {
                        if (_currentTitleType != TitleUIType.Start)
                        {
                            return;
                        }
                        ChangeUIPanel(TitleUIType.DifficultySelect);
                    });
                    break;
                case ButtonType.Credit:
                    b.Value.onClick.AddListener(() =>
                    {
                        if (_currentTitleType != TitleUIType.Start)
                        {
                            return;
                        }
                        Debug.Log("Credit");
                    });
                    break;
                case ButtonType.Option:
                    b.Value.onClick.AddListener(() =>
                    {
                        if (_currentTitleType != TitleUIType.Start)
                        {
                            return;
                        }
                        Debug.Log("Option");
                    });
                    break;
                case ButtonType.Difficulty_Easy:
                    b.Value.onClick.AddListener(() =>
                    {
                        if (_currentTitleType != TitleUIType.DifficultySelect)
                        {
                            return;
                        }
                        GameManager.ChangeGameDifficult(DifficultyType.Easy);
                        TransitionManager.SceneTransition(SceneType.Intro);
                        AudioManager.PlaySE(SEType.UI_GameStart);
                    });
                    break;
                case ButtonType.Difficulty_Normal:
                    b.Value.onClick.AddListener(() =>
                    {
                        if (_currentTitleType != TitleUIType.DifficultySelect)
                        {
                            return;
                        }
                        GameManager.ChangeGameDifficult(DifficultyType.Normal);
                        TransitionManager.SceneTransition(SceneType.Intro);
                        AudioManager.PlaySE(SEType.UI_GameStart);
                    });
                    break;
                case ButtonType.Difficulty_Hard:
                    b.Value.onClick.AddListener(() =>
                    {
                        if (_currentTitleType != TitleUIType.DifficultySelect)
                        {
                            return;
                        }
                        GameManager.ChangeGameDifficult(DifficultyType.Hard);
                        TransitionManager.SceneTransition(SceneType.Intro);
                        AudioManager.PlaySE(SEType.UI_GameStart);
                    });
                    break;
                default:
                    break;
            }
        }
    }

    /// <summary>
    /// �^�C�g����UIPanel��ύX����
    /// </summary>
    /// <param name="titleUIType"> �^�C�g����UI�̎�� </param>
    void ChangeUIPanel(TitleUIType titleUIType)
    {
        StartCoroutine(ChangeUIPanelCoroutine(titleUIType));
    }

    IEnumerator ChangeUIPanelCoroutine(TitleUIType titleUIType)
    {
        //��U���ׂĂ�Panel��OFF�ɂ���
        foreach (var p in _titleUIGroups)
        {
            p.alpha = 0;
        }

        yield return new WaitForSeconds(0.05f);

        _currentTitleType = titleUIType;

        switch (_currentTitleType)
        {
            case TitleUIType.Start:
                _titleUIGroups[0].alpha = 1;
                _buttonDic[ButtonType.GameStart].Select();
                break;
            case TitleUIType.DifficultySelect:
                _titleUIGroups[1].alpha = 1;
                _buttonDic[ButtonType.Difficulty_Easy].Select();
                break;
            case TitleUIType.Credit:
                _titleUIGroups[2].alpha = 1;
                break;
            case TitleUIType.Option:
                _titleUIGroups[3].alpha = 1;
                break;
            default:
                break;
        }
    }
}

/// <summary>
/// �^�C�g����ʂ�UI�̎��
/// </summary>
public enum TitleUIType
{
    Start,
    DifficultySelect,
    Credit,
    Option
}

/// <summary>
/// �{�^���̎��
/// </summary>
public enum ButtonType
{
    GameStart,
    Credit,
    Option,
    Difficulty_Easy,
    Difficulty_Normal,
    Difficulty_Hard
}
