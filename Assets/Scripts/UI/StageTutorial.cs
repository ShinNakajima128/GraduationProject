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
    GameObject[] _descriptionPanels = default;
    #endregion

    #region private
    CanvasGroup _tutorialGroup = default;
    bool _isPressed = false;
    int _currentPageIndex = 0;
    int _currentPageLength = 0;
    Dictionary<DescriptionType, GameObject> _descriptionPanelDic = new Dictionary<DescriptionType, GameObject>();
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

        //説明画面のDictionaryを作成
        for (int i = 0; i < (int)DescriptionType.Num; i++)
        {
            _descriptionPanelDic.Add((DescriptionType)i, _descriptionPanels[i]);
            _descriptionPanelDic[(DescriptionType)i].SetActive(false);
        }
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

        foreach (var panel in _descriptionPanelDic)
        {
            panel.Value.SetActive(false);
        }

        //現在のページを知らせるアイコンのセットアップ
        foreach (var icon in _pageIconsTrans)
        {
            icon.gameObject.SetActive(false);
        }

        var data = _tutorialData.FirstOrDefault(d => d.Stage == stage);

        SetDescription(data); //ステージ詳細データをUIに反映
        _tutorialBackground.sprite = data.TutorialBackground; //背景画像をセット
        _currentPageLength = data.DescriptionDatas.Length; //ページの長さを取得
        _currentPageIcon.sprite = data.CurrentPageIcon; //ステージごとのページアイコンをセット

        //サンプル画面のセットアップ
        for (int i = 0; i < _currentPageLength; i++)
        {
            _tutorialImages[i].sprite = data.DescriptionDatas[i].PageSprite;
            _pageIconsTrans[i].gameObject.SetActive(true);
        }
        _tutorialImages[_currentPageIndex].enabled = true;
        SetCurrentPageIcon();
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
        
        //左側のゲーム画面Imageを切り替える
        _tutorialImages[_currentPageIndex].enabled = true;
        _tutorialImages[_currentPageIndex + 1].enabled = false;

        //右側の説明画面を切り替える
        _descriptionPanelDic[(DescriptionType)_currentPageIndex].SetActive(true);
        _descriptionPanelDic[(DescriptionType)_currentPageIndex + 1].SetActive(false);

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
        _descriptionPanelDic[(DescriptionType)_currentPageIndex].SetActive(true);
        _descriptionPanelDic[(DescriptionType)_currentPageIndex - 1].SetActive(false);

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
        _descriptionPanelDic[DescriptionType.GameDescription].SetActive(true);
        //for (int i = 0; i < data.DescriptionDatas.Length; i++)
        //{

        //}
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
    [TextArea(1, 5)]
    public string DescriptionText;
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