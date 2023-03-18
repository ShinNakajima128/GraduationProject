using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;
using DG.Tweening;

/// <summary>
/// ステージのチュートリアルを表示する機能を持つコンポーネント
/// </summary>
public class StageTutorial : MonoBehaviour
{
    #region serialize
    [Header("Datas")]
    [SerializeField]
    StageTutorialData[] _tutorialData = default;

    [Header("UIObjects")]
    [SerializeField]
    Image _tutorialBackground = default;

    [Header("Page")]
    [SerializeField]
    Image _currentPageIcon = default;

    [SerializeField]
    Transform[] _pageIconsTrans = default;

    [SerializeField]
    Transform[] _crossKeyImagesTrans = default;

    [SerializeField]
    Image[] _tutorialImages = default;

    [SerializeField]
    Image _gameStartImage = default;

    [Header("Description")]
    [SerializeField]
    Image[] _descriptionImages = default;
    #endregion

    #region private
    CanvasGroup _tutorialGroup = default;
    bool _isPressed = false;
    int _currentPageIndex = 0;
    int _currentPageLength = 0;
    #endregion

    #region public
    public event Action PlayButtonPressAction;
    #endregion

    #region property
    public bool IsActivateTutorial => _tutorialGroup.alpha == 1;
    #endregion

    private void Awake()
    {
        TryGetComponent(out _tutorialGroup);
    }

    private void Start()
    {
        //「左十字キー」を押した時の処理を登録
        this.UpdateAsObservable()
            .Where(_ => IsActivateTutorial && UIInput.LeftCrossKey && !_isPressed)
            .Subscribe(_ => 
            {
                if (_currentPageIndex <= 0)
                {
                    _isPressed = true;
                    _crossKeyImagesTrans[0].transform.DOShakePosition(0.2f, 10, 20)
                                         .OnComplete(() =>
                                         {
                                             _isPressed = false;
                                         });
                    AudioManager.PlaySE(SEType.UI_CannotSelect);
                    return;
                }
                _currentPageIndex--;
                OnLeftPage();
                AudioManager.PlaySE(SEType.UI_CursolMove);
            })
            .AddTo(this);

        //「右十字キー」を押した時の処理を登録
        this.UpdateAsObservable()
            .Where(_ => IsActivateTutorial && UIInput.RightCrossKey && !_isPressed)
            .Subscribe(_ =>
            {
                if (_currentPageIndex >= _currentPageLength - 1)
                {
                    _isPressed = true;
                    _crossKeyImagesTrans[1].transform.DOShakePosition(0.2f, 10, 20)
                                         .OnComplete(() =>
                                         {
                                             _isPressed = false;
                                         });
                    AudioManager.PlaySE(SEType.UI_CannotSelect);
                    return;
                }
                _currentPageIndex++;
                OnRightPage();
                AudioManager.PlaySE(SEType.UI_CursolMove);
            })
            .AddTo(this);

        //最後のページを表示している場合のゲームスタート処理を登録
        this.UpdateAsObservable()
            .Where(_ => IsActivateTutorial && 
                        _currentPageIndex == _currentPageLength - 1 && 
                        UIInput.B && 
                        !_isPressed)
            .Subscribe(_ =>
            {
                _isPressed = true;
                PlayButtonPressAction?.Invoke(); //登録してあるScene遷移処理を行う
            });
    }

    /// <summary>
    /// チュートリアル画面のセットアップ
    /// </summary>
    /// <param name="stage"> 表示するステージ </param>
    /// <param name="transitionAction"> ミニゲームSceneに遷移する処理 </param>
    public void TutorialSetup(Stages stage, Action transitionAction = null)
    {
        if (transitionAction != null)
        {
            PlayButtonPressAction += transitionAction;
        }

        _currentPageIndex = 0;
        _gameStartImage.DOFade(0.3f, 0f);

        foreach (var image in _tutorialImages)
        {
            image.enabled = false;
        }

        foreach (var image in _descriptionImages)
        {
            image.enabled = false;
        }

        //現在のページを知らせるアイコンのセットアップ
        foreach (var icon in _pageIconsTrans)
        {
            icon.gameObject.SetActive(false);
        }

        var data = _tutorialData.FirstOrDefault(d => d.Stage == stage);

        SetDescription(data); //ステージ詳細データをUIに反映

        //ステージ1でのチュートリアルではない場合
        if (GameManager.Instance.CurrentScene != SceneType.Stage1_Fall)
        {
            _tutorialBackground.sprite = data.TutorialBackground; //背景画像をセット
        }
        _currentPageLength = data.DescriptionDatas.Length; //ページの長さを取得
        _currentPageIcon.sprite = data.CurrentPageIcon; //ステージごとのページアイコンをセット

        //サンプル画面のセットアップ
        for (int i = 0; i < _currentPageLength; i++)
        {
            _tutorialImages[i].sprite = data.DescriptionDatas[i].PageSprite;
            _pageIconsTrans[i].gameObject.SetActive(true);
        }
        _tutorialImages[_currentPageIndex].enabled = true;
        _descriptionImages[_currentPageIndex].enabled = true;
        SetCurrentPageIcon();
    }

    /// <summary>
    /// チュートリアル画面のON/OFFを切り替える
    /// </summary>
    /// <param name="isActivate"> 表示か非表示かどうか </param>
    public void ActivateTutorialUI(bool isActivate, float fadeTime = 0f)
    {
        if (isActivate)
        {
            UIManager.ActivatePanel(UIPanelType.Tutorial);
            DOTween.To(() => _tutorialGroup.alpha,
                x => _tutorialGroup.alpha = x,
                1,
                fadeTime);
        }
        else
        {
            UIManager.InactivatePanel(UIPanelType.Tutorial);
            DOTween.To(() => _tutorialGroup.alpha,
                x => _tutorialGroup.alpha = x,
                0,
                fadeTime);
            PlayButtonPressAction = null;
        }
    }

    /// <summary>
    /// 現在のページの左ページを表示する
    /// </summary>
    void OnLeftPage()
    {
        Debug.Log($"{_currentPageIndex + 1}ページ目を表示");
        _isPressed = true;
        
        //左側のゲーム画面Imageを切り替える
        _tutorialImages[_currentPageIndex].enabled = true;
        _tutorialImages[_currentPageIndex + 1].enabled = false;

        //右側の説明画面を切り替える
        _descriptionImages[_currentPageIndex].enabled = true;
        _descriptionImages[_currentPageIndex + 1].enabled = false;

        _currentPageIcon.transform.position = _pageIconsTrans[_currentPageIndex].position;
        _crossKeyImagesTrans[0].transform.DOScale(1.2f, 0.1f)
                                         .SetLoops(2, LoopType.Yoyo)
                                         .OnComplete(() =>
                                         {
                                             _isPressed = false;
                                         });

        //最後のページ以外の場合はゲームスタートボタンを非アクティブにする
        if (_currentPageIndex < _currentPageLength - 1)
        {
            _gameStartImage.DOFade(0.3f, 0.2f);
        }
    }

    // <summary>
    /// 現在のページの右ページを表示する
    /// </summary>
    void OnRightPage()
    {
        Debug.Log($"{_currentPageIndex + 1}ページ目を表示");

        _isPressed = true;

        //左側のゲーム画面Imageを切り替える
        _tutorialImages[_currentPageIndex].enabled = true;
        _tutorialImages[_currentPageIndex - 1].enabled = false;

        //右側の説明画面を切り替える
        _descriptionImages[_currentPageIndex].enabled = true;
        _descriptionImages[_currentPageIndex - 1].enabled = false;

        _currentPageIcon.transform.position = _pageIconsTrans[_currentPageIndex].position;
        _crossKeyImagesTrans[1].transform.DOScale(1.2f, 0.1f)
                                         .SetLoops(2, LoopType.Yoyo)
                                         .OnComplete(() =>
                                         {
                                             _isPressed = false;
                                         });

        //最後のページの場合はゲームスタートボタンをアクティブにする
        if (_currentPageIndex >= _currentPageLength - 1)
        {
            _gameStartImage.DOFade(1f, 0.2f);
        }
    }

    void SetDescription(StageTutorialData data)
    {
        for (int i = 0; i < data.DescriptionDatas.Length; i++)
        {
            _descriptionImages[i].sprite = data.DescriptionDatas[i].DescriptionSprite;
        }
    }

    /// <summary>
    /// 現在のページを表すアイコンの位置をセットする
    /// </summary>
    void SetCurrentPageIcon()
    {
        StartCoroutine(SetCurrentPageIconCoroutine());
    }

    IEnumerator SetCurrentPageIconCoroutine()
    {
        yield return null;

        _currentPageIcon.transform.position = _pageIconsTrans[_currentPageIndex].position;
    }
}

/// <summary>
/// ステージのチュートリアルデータ
/// </summary>
[Serializable]
public struct StageTutorialData
{
    public string StageName;
    public Stages Stage;
    public StageDescriptionData[] DescriptionDatas;
    public Sprite CurrentPageIcon;
    public Sprite TutorialBackground;

}

/// <summary>
/// チュートリアルの説明データ
/// </summary>
[Serializable]
public struct StageDescriptionData
{
    public string DescriptionName;
    public DescriptionType DescriptionType;
    public Sprite PageSprite;
    public Sprite DescriptionSprite;
}

/// <summary>
/// 説明の種類
/// </summary>
public enum DescriptionType
{
    /// <summary> ゲーム説明 </summary>
    GameDescription,
    /// <summary> 操作方法 </summary>
    HowToOperate,
    /// <summary> ヒント1 </summary>
    Tips1,
    /// <summary> ヒント2 </summary>
    Tips2,
    /// <summary> ヒント3 </summary>
    Tips3,
    Num
}