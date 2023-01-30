using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;
using DG.Tweening;

public class AlbumManager : MonoBehaviour
{
    #region serialize
    [Header("Variables")]
    [SerializeField]
    float _animTime = 0.5f;

    [SerializeField]
    Ease _animEase = Ease.OutBounce;

    [Header("UI")]
    [SerializeField]
    GameObject[] _pages = default;

    [SerializeField]
    Image[] _stillImages = default;

    [SerializeField]
    GameObject[] _stickerPanels = default;
 
    [SerializeField]
    Transform _originPosTrans = default;

    [SerializeField]
    GameObject _operatePanel = default;
    #endregion

    #region private
    CanvasGroup _albumGroup;
    bool _isOpened = false;
    bool _isPressed = false;
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
        TryGetComponent(out _albumGroup);
        transform.DOLocalMoveY(_originPosTrans.localPosition.y, 0f);
    }

    public void Start()
    {
        if (GameManager.Instance.CurrentLobbyState == LobbyState.Default)
        {
            #region Lobby 
            //アルバムの開閉処理を登録
            this.UpdateAsObservable().Where(_ => UIManager.Instance.IsCanOpenUI &&
                                                 !_isOpened &&
                                                 !_isPressed &&
                                                 GameManager.Instance.CurrentLobbyState == LobbyState.Default &&
                                                 !UIManager.Instance.IsAnyPanelOpened &&
                                                 UIInput.X)
                                     .Subscribe(_ =>
                                     {
                                         UIManager.ActivatePanel(UIPanelType.Album);
                                         ActivateAlbum(true);
                                         Debug.Log("アルバム表示");
                                     })
                                     .AddTo(this);

            this.UpdateAsObservable().Where(_ => _isOpened &&
                                                 !_isPressed &&
                                                 GameManager.Instance.CurrentLobbyState == LobbyState.Default &&
                                                 UIInput.A)
                                     .Subscribe(_ =>
                                     {
                                         UIManager.InactivatePanel(UIPanelType.Album);
                                         ActivateAlbum(false);
                                         Debug.Log("アルバム非表示");
                                     })
                                     .AddTo(this);
            #endregion
        }
        else
        {
            #region UnderLobby
            //アルバムの開閉処理を登録
            this.UpdateAsObservable().Where(_ => UIManager.Instance.IsCanOpenUI &&
                                                 !_isOpened &&
                                                 !_isPressed &&
                                                 GameManager.Instance.CurrentLobbyState == LobbyState.Under &&
                                                 !UIManager.Instance.IsAnyPanelOpened &&
                                                 UIInput.X)
                                     .Subscribe(_ =>
                                     {
                                         UIManager.ActivatePanel(UIPanelType.Album);
                                         ActivateAlbum(true);
                                         Debug.Log("アルバム表示");
                                     })
                                     .AddTo(this);

            this.UpdateAsObservable().Where(_ => UIManager.Instance.IsCanOpenUI && 
                                                 _isOpened &&
                                                 !_isPressed &&
                                                 GameManager.Instance.CurrentLobbyState == LobbyState.Under &&
                                                 UIInput.A)
                                     .Subscribe(_ =>
                                     {
                                         UIManager.InactivatePanel(UIPanelType.Album);
                                         ActivateAlbum(false);
                                         Debug.Log("アルバム非表示");
                                     })
                                     .AddTo(this);
            #endregion
        }
    }

    /// <summary>
    /// アルバムのUIPanelの表示/非表示を切り替える
    /// </summary>
    /// <param name="isOpened"> 表示するか非表示にするかのフラグ </param>
    //void ActivatePanel(bool isCanOpened)
    //{
    //    _isCanOpened = isCanOpened;

    //    if (_isCanOpened)
    //    {
    //        _albumUIGroup.alpha = 1;
    //        Debug.Log("アルバムUI表示");
    //    }
    //    else
    //    {
    //        _albumUIGroup.alpha = 0;
    //        _currentPageIndex = 0;
    //        Debug.Log("アルバムUI非表示");
    //    }
    //}

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
        //_pageSwitchButtons[0].gameObject.SetActive(true);
        //_pageSwitchButtons[1].gameObject.SetActive(true);

        //全てのページを一度非表示にする
        foreach (var p in _pages)
        {
            p.SetActive(false);
        }

        //指定したページを表示する
        _pages[index].SetActive(true);

        Debug.Log(index);

        //一番左のページの時は左のボタンを非表示にする
        //if (index == 0)
        //{
        //    _pageSwitchButtons[0].gameObject.SetActive(false);
        //}
        ////最後のページの時は右のボタンを非表示にする
        //else if (index == _pages.Length - 1)
        //{
        //    _pageSwitchButtons[1].gameObject.SetActive(false);
        //}
    }

    void StillSetup()
    {
        var stages = GameManager.Instance.StageSttatusDic;

        for (int i = 0; i < stages.Count - 1; i++)
        {
            _stillImages[i].enabled = stages[(Stages)i]; //ステージのクリア状況に応じてスチルの表示を切り替える
            _stickerPanels[i].SetActive(stages[(Stages)i]);
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
    IEnumerator OnAlbumCoroutine(bool isOpened)
    {
        _isPressed = true;
        _isOpened = isOpened;

        if (GameManager.Instance.CurrentLobbyState == LobbyState.Default)
        {
            LobbyManager.Instance.PlayerMove?.Invoke(false);
        }
        else
        {
            UnderLobbyManager.Instance.PlayerMove?.Invoke(false);
        }

        if (_isOpened)
        {
            AudioManager.PlaySE(SEType.Lobby_OpenAlbum);
            StillSetup();
            _albumGroup.alpha = 1;
            ActivePage(_currentPageIndex);

            if (GameManager.Instance.CurrentLobbyState == LobbyState.Default)
            {
                LobbyManager.Instance.CurrentUIState = LobbyUIState.Album;
            }
            else
            {
                UnderLobbyManager.Instance.CurrentUIState = LobbyUIState.Album;
            }

            yield return transform.DOLocalMoveY(0, _animTime)
                                  .SetEase(_animEase)
                                  .WaitForCompletion();
            _operatePanel.SetActive(true);
        }
        else
        {
            AudioManager.PlaySE(SEType.Lobby_CloseAlbum);
            _operatePanel.SetActive(false);

            yield return transform.DOLocalMoveY(_originPosTrans.localPosition.y, _animTime)
                                  .SetEase(Ease.OutCubic)
                                  .WaitForCompletion();

            _albumGroup.alpha = 0;
            _currentPageIndex = 0;

            if (GameManager.Instance.CurrentLobbyState == LobbyState.Default)
            {
                LobbyManager.Instance.CurrentUIState = LobbyUIState.Default;
                LobbyManager.Instance.PlayerMove?.Invoke(true);
            }
            else
            {
                UnderLobbyManager.Instance.CurrentUIState = LobbyUIState.Default;
                UnderLobbyManager.Instance.PlayerMove?.Invoke(true);
            }

            if (StageDescriptionUI.Instance.IsActived)
            {
                StageDescriptionUI.Instance.ActiveButton();
            }
        }
        _isPressed = false;
        yield return null;
    }
}
