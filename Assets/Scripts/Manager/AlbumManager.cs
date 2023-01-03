using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;

public class AlbumManager : MonoBehaviour
{
    #region serialize
    [Header("UI")]
    [SerializeField]
    CanvasGroup _albumUIGroup = default;

    [SerializeField]
    CanvasGroup _albumGroup = default;

    [SerializeField]
    GameObject[] _pages = default; 

    [SerializeField]
    Button[] _pageSwitchButtons = default;
    #endregion

    #region private
    bool _isCanOpened = false;
    bool _isOpened = false;
    int _currentPageIndex = 0;
    #endregion

    #region public
    #endregion

    #region property
    public static AlbumManager Instance { get; private set; }
    #endregion

    private void Awake()
    {
        Instance = this;
    }

    public void Start()
    {
        LobbyManager.Instance.PlayerMove += ActivatePanel;

        //アルバムの開閉処理を登録
        this.UpdateAsObservable().Where(_ => _isCanOpened &&
                                             !_isOpened &&
                                             LobbyManager.Instance.CurrentUIState == LobbyUIState.Default && 
                                             UIInput.Y)
                                 .Subscribe(_ =>
                                 {
                                     ActivateAlbum(true);
                                     Debug.Log("アルバム表示");
                                 });

        this.UpdateAsObservable().Where(_ => _isOpened && 
                                             LobbyManager.Instance.CurrentUIState == LobbyUIState.Album && 
                                             UIInput.A)
                                 .Subscribe(_ =>
                                 {
                                     ActivateAlbum(false);
                                     Debug.Log("アルバム非表示");
                                 });

        //ページを進める/戻すボタンの処理を登録
        _pageSwitchButtons[0].OnClickAsObservable().Where(_ => _isOpened)
                                                   .Subscribe(_ => OnPrevPage());
        
        _pageSwitchButtons[1].OnClickAsObservable().Where(_ => _isOpened)
                                                   .Subscribe(_ => OnNextPage());
    }

    /// <summary>
    /// アルバムのUIPanelの表示/非表示を切り替える
    /// </summary>
    /// <param name="isOpened"> 表示するか非表示にするかのフラグ </param>
    void ActivatePanel(bool isCanOpened)
    {
        _isCanOpened = isCanOpened;

        if (_isCanOpened)
        {
            _albumUIGroup.alpha = 1;
            Debug.Log("アルバムUI表示");
        }
        else
        {
            _albumUIGroup.alpha = 0;
            _currentPageIndex = 0;
            Debug.Log("アルバムUI非表示");
        }
    }

    /// <summary>
    /// アルバムの表示/非表示を切り替える
    /// </summary>
    /// <param name="isOpened"></param>
    void ActivateAlbum(bool isOpened)
    {
        StartCoroutine(OnAlbumCoroutine(isOpened));
    }

    void ActivePage(int index)
    {
        //ページを送るボタンオブジェクトを表示する
        _pageSwitchButtons[0].gameObject.SetActive(true);
        _pageSwitchButtons[1].gameObject.SetActive(true);

        //全てのページを一度非表示にする
        foreach (var p in _pages)
        {
            p.SetActive(false);
        }

        //指定したページを表示する
        _pages[index].SetActive(true);

        Debug.Log(index);

        //一番左のページの時は左のボタンを非表示にする
        if (index == 0)
        {
            _pageSwitchButtons[0].gameObject.SetActive(false);
        }
        //最後のページの時は右のボタンを非表示にする
        else if (index == _pages.Length - 1)
        {
            _pageSwitchButtons[1].gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 次のページを表示する
    /// </summary>
    #region button method
    void OnNextPage()
    {
        _currentPageIndex++;

        if (_currentPageIndex >= _pages.Length)
        {
            _currentPageIndex = _pages.Length - 1;
            return;
        }

        ActivePage(_currentPageIndex);
    }

    /// <summary>
    /// 前のページを表示する
    /// </summary>
    void OnPrevPage()
    {
        _currentPageIndex--;

        if (_currentPageIndex < 0)
        {
            _currentPageIndex = 0;
            return;
        }

        ActivePage(_currentPageIndex);
    }
    #endregion

    /// <summary>
    /// アルバムを開く処理のコルーチン
    /// </summary>
    /// <param name="isOpened"> 開けるか閉じるかのフラグ </param>
    /// <returns></returns>
    IEnumerator OnAlbumCoroutine(bool isOpened)
    {
        _isOpened = isOpened;

        if (_isOpened)
        {
            _albumGroup.alpha = 1;
            ActivePage(_currentPageIndex);
            LobbyManager.Instance.CurrentUIState = LobbyUIState.Album;
        }
        else
        {
            _albumGroup.alpha = 0;
            _currentPageIndex = 0;
            LobbyManager.Instance.CurrentUIState = LobbyUIState.Default;
        }
        yield return null;
    }
}
