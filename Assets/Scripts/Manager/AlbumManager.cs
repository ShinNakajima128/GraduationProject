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
            //�A���o���̊J������o�^
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
                                         Debug.Log("�A���o���\��");
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
                                         Debug.Log("�A���o����\��");
                                     })
                                     .AddTo(this);
            #endregion
        }
        else
        {
            #region UnderLobby
            //�A���o���̊J������o�^
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
                                         Debug.Log("�A���o���\��");
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
                                         Debug.Log("�A���o����\��");
                                     })
                                     .AddTo(this);
            #endregion
        }
    }

    /// <summary>
    /// �A���o����UIPanel�̕\��/��\����؂�ւ���
    /// </summary>
    /// <param name="isOpened"> �\�����邩��\���ɂ��邩�̃t���O </param>
    //void ActivatePanel(bool isCanOpened)
    //{
    //    _isCanOpened = isCanOpened;

    //    if (_isCanOpened)
    //    {
    //        _albumUIGroup.alpha = 1;
    //        Debug.Log("�A���o��UI�\��");
    //    }
    //    else
    //    {
    //        _albumUIGroup.alpha = 0;
    //        _currentPageIndex = 0;
    //        Debug.Log("�A���o��UI��\��");
    //    }
    //}

    /// <summary>
    /// �A���o���̕\��/��\����؂�ւ���
    /// </summary>
    /// <param name="isOpened"></param>
    void ActivateAlbum(bool isOpened)
    {
        StartCoroutine(OnAlbumCoroutine(isOpened));
    }

    void ActivePage(int index)
    {
        //�y�[�W�𑗂�{�^���I�u�W�F�N�g��\������
        //_pageSwitchButtons[0].gameObject.SetActive(true);
        //_pageSwitchButtons[1].gameObject.SetActive(true);

        //�S�Ẵy�[�W����x��\���ɂ���
        foreach (var p in _pages)
        {
            p.SetActive(false);
        }

        //�w�肵���y�[�W��\������
        _pages[index].SetActive(true);

        Debug.Log(index);

        //��ԍ��̃y�[�W�̎��͍��̃{�^�����\���ɂ���
        //if (index == 0)
        //{
        //    _pageSwitchButtons[0].gameObject.SetActive(false);
        //}
        ////�Ō�̃y�[�W�̎��͉E�̃{�^�����\���ɂ���
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
            _stillImages[i].enabled = stages[(Stages)i]; //�X�e�[�W�̃N���A�󋵂ɉ����ăX�`���̕\����؂�ւ���
            _stickerPanels[i].SetActive(stages[(Stages)i]);
        }
    }

    /// <summary>
    /// ���̃y�[�W��\������
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
    /// �O�̃y�[�W��\������
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
    /// �A���o�����J�������̃R���[�`��
    /// </summary>
    /// <param name="isOpened"> �J���邩���邩�̃t���O </param>
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
