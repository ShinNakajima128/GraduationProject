using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UniRx;
using UniRx.Triggers;
using DG.Tweening;

/// <summary>
/// タイトル画面の機能を管理するマネージャー
/// </summary>
public class TitleManager : MonoBehaviour
{
    #region serialize
    [Header("UI")]
    [Tooltip("タイトル画面の各UIPanel")]
    [SerializeField]
    CanvasGroup[] _titleUIGroups = default;

    [Tooltip("タイトルのButton")]
    [SerializeField]
    Button[] _titleButtons = default;

    [Tooltip("タイトル画面の現在のUIの種類")]
    [SerializeField]
    TitleUIType _currentTitleType = default;

    [Header("Components")]
    [SerializeField]
    Option _option = default;
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

        //開始画面以外で「A」を押した場合は開始画面に戻る
        this.UpdateAsObservable().Where(_ => _currentTitleType != TitleUIType.Start &&
                                             !PreviewManager.Instance.IsPreviewed &&
                                             UIInput.A)
                                 .ThrottleFirst(TimeSpan.FromMilliseconds(1000))
                                 .Subscribe(_ =>
                                 {
                                     ChangeUIPanel(TitleUIType.Start);
                                 })
                                 .AddTo(this);

        _option.OnBackToMainMenu += () =>
        {
            ChangeUIPanel(TitleUIType.Start);
        };
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
        GameManager.GameReset(); //クリアしてタイトルに戻ってきた場合用の初期化処理
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (UIInput.Option)
        {
            TransitionManager.SceneTransition(SceneType.Stage_Boss);
        }
    }
#endif
    void ButtonSetup()
    {
        foreach (var b in _buttonDic)
        {
            switch (b.Key)
            {
                case ButtonType.GameStart:
                    b.Value.OnClickAsObservable()
                           .Where(_ => !PreviewManager.Instance.IsPreviewed)
                           .ThrottleFirst(TimeSpan.FromMilliseconds(1000))
                           .Subscribe(_ =>
                           {
                               if (_currentTitleType != TitleUIType.Start)
                               {
                                   return;
                               }
                               ChangeUIPanel(TitleUIType.DifficultySelect);
                           });
                    break;
                case ButtonType.Credit:
                    b.Value.OnClickAsObservable()
                           .Where(_ => !PreviewManager.Instance.IsPreviewed)
                           .Take(1)
                           .Subscribe(_ =>
                           {
                               if (_currentTitleType != TitleUIType.Start)
                               {
                                   return;
                               }
                               TransitionManager.SceneTransition(SceneType.Credit);
                               AudioManager.PlaySE(SEType.UI_GameStart);
                               b.Value.transform.DOLocalMoveY(b.Value.transform.localPosition.y - 15, 0.05f)
                                                                        .SetLoops(2, LoopType.Yoyo);
                               EventSystem.current.SetSelectedGameObject(null);
                               Debug.Log("Credit");
                           });
                    break;
                case ButtonType.Option:
                    b.Value.OnClickAsObservable()
                           .Where(_ => !PreviewManager.Instance.IsPreviewed)
                           .ThrottleFirst(TimeSpan.FromMilliseconds(1000))
                           .Subscribe(_ =>
                           {
                               if (_currentTitleType != TitleUIType.Start)
                               {
                                   return;
                               }
                               _option.ActiveOption();
                               _currentTitleType = TitleUIType.Option;
                               Debug.Log("Option");
                           });
                    break;
                case ButtonType.Difficulty_Easy:
                    b.Value.OnClickAsObservable()
                           .Where(_ => _currentTitleType == TitleUIType.DifficultySelect &&
                                       !PreviewManager.Instance.IsPreviewed)
                           .Take(1)
                           .Subscribe(_ =>
                           {
                               GameManager.ChangeGameDifficult(DifficultyType.Easy);
                               TransitionManager.SceneTransition(SceneType.Intro);
                               AudioManager.PlaySE(SEType.UI_GameStart);
                               b.Value.transform.DOLocalMoveY(b.Value.transform.localPosition.y - 15, 0.05f)
                                                                        .SetLoops(2, LoopType.Yoyo);
                               EventSystem.current.SetSelectedGameObject(null);
                           });
                    break;
                case ButtonType.Difficulty_Normal:
                    b.Value.OnClickAsObservable()
                           .Where(_ => _currentTitleType == TitleUIType.DifficultySelect &&
                                       !PreviewManager.Instance.IsPreviewed)
                           .Take(1)
                           .Subscribe(_ =>
                           {
                               GameManager.ChangeGameDifficult(DifficultyType.Normal);
                               TransitionManager.SceneTransition(SceneType.Intro);
                               AudioManager.PlaySE(SEType.UI_GameStart);
                               b.Value.transform.DOLocalMoveY(b.Value.transform.localPosition.y - 15, 0.05f)
                                                                        .SetLoops(2, LoopType.Yoyo);
                               EventSystem.current.SetSelectedGameObject(null);
                           });
                    break;
                case ButtonType.Difficulty_Hard:
                    b.Value.OnClickAsObservable()
                           .Where(_ => _currentTitleType == TitleUIType.DifficultySelect &&
                                       !PreviewManager.Instance.IsPreviewed)
                           .Take(1)
                           .Subscribe(_ =>
                           {
                               GameManager.ChangeGameDifficult(DifficultyType.Hard);
                               TransitionManager.SceneTransition(SceneType.Intro);
                               AudioManager.PlaySE(SEType.UI_GameStart);
                               b.Value.transform.DOLocalMoveY(b.Value.transform.localPosition.y - 15, 0.05f)
                                                                        .SetLoops(2, LoopType.Yoyo);
                               EventSystem.current.SetSelectedGameObject(null);
                           });
                    break;
                default:
                    break;
            }
        }
    }

    /// <summary>
    /// タイトルのUIPanelを変更する
    /// </summary>
    /// <param name="titleUIType"> タイトルのUIの種類 </param>
    public void ChangeUIPanel(TitleUIType titleUIType)
    {
        StartCoroutine(ChangeUIPanelCoroutine(titleUIType));
    }

    IEnumerator ChangeUIPanelCoroutine(TitleUIType titleUIType)
    {
        //一旦すべてのPanelをOFFにする
        foreach (var p in _titleUIGroups)
        {
            p.alpha = 0;
        }

        yield return null;

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

    public void SetSelectButton()
    {
        _titleButtons[0].Select();
    }
}

/// <summary>
/// タイトル画面のUIの種類
/// </summary>
public enum TitleUIType
{
    Start,
    DifficultySelect,
    Credit,
    Option
}

/// <summary>
/// ボタンの種類
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
