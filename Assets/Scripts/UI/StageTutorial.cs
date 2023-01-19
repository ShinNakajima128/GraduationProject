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
    //[Header("GameDescription")]
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
                    return;
                }
                _currentPageIndex--;
                OnLeftPage();
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
                    return;
                }
                _currentPageIndex++;
                OnRightPage();
            })
            .AddTo(this);

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

        var data = _tutorialData.FirstOrDefault(d => d.Stage == stage);

        _currentPageLength = data.DescriptionDatas.Length;
        _currentPageIcon.sprite = data.CurrentPageIcon;

        foreach (var image in _tutorialImages)
        {
            image.enabled = false;
        }

        //現在のページを知らせるアイコンのセットアップ
        foreach (var icon in _pageIconsTrans)
        {
            icon.gameObject.SetActive(false);
        }

        //サンプル画面のセットアップ
        for (int i = 0; i < _currentPageLength; i++)
        {
            _tutorialImages[i].sprite = data.DescriptionDatas[i].PageSprite;
            _pageIconsTrans[i].gameObject.SetActive(true);
        }
        _tutorialImages[_currentPageIndex].enabled = true;
        _currentPageIcon.transform.localPosition = _pageIconsTrans[_currentPageIndex].localPosition;
    }

    /// <summary>
    /// チュートリアル画面のON/OFFを切り替える
    /// </summary>
    /// <param name="isActivate"> 表示か非表示かどうか </param>
    public void ActivateTutorialUI(bool isActivate)
    {
        if (isActivate)
        {
            _tutorialGroup.alpha = 1;
        }
        else
        {
            _tutorialGroup.alpha = 0;
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
        
        _tutorialImages[_currentPageIndex].enabled = true;
        _tutorialImages[_currentPageIndex + 1].enabled = false;

        _currentPageIcon.transform.localPosition = _pageIconsTrans[_currentPageIndex].localPosition;
        _crossKeyImagesTrans[0].transform.DOScale(1.2f, 0.1f)
                                         .SetLoops(2, LoopType.Yoyo)
                                         .OnComplete(() =>
                                         {
                                             _isPressed = false;
                                         });

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

        _tutorialImages[_currentPageIndex].enabled = true;
        _tutorialImages[_currentPageIndex - 1].enabled = false;

        _currentPageIcon.transform.localPosition = _pageIconsTrans[_currentPageIndex].localPosition;
        _crossKeyImagesTrans[1].transform.DOScale(1.2f, 0.1f)
                                         .SetLoops(2, LoopType.Yoyo)
                                         .OnComplete(() =>
                                         {
                                             _isPressed = false;
                                         });

        if (_currentPageIndex >= _currentPageLength - 1)
        {
            _gameStartImage.DOFade(1f, 0.2f);
        }
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

}

/// <summary>
/// チュートリアルの説明データ
/// </summary>
[Serializable]
public struct StageDescriptionData
{
    public DescriptionType DescriptionType;
    public Sprite PageSprite;
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
    Tips3
}