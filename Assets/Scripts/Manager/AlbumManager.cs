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

        //�A���o���̊J������o�^
        this.UpdateAsObservable().Where(_ => _isCanOpened &&
                                             !_isOpened &&
                                             LobbyManager.Instance.CurrentUIState == LobbyUIState.Default && 
                                             UIInput.Y)
                                 .Subscribe(_ =>
                                 {
                                     ActivateAlbum(true);
                                     Debug.Log("�A���o���\��");
                                 });

        this.UpdateAsObservable().Where(_ => _isOpened && 
                                             LobbyManager.Instance.CurrentUIState == LobbyUIState.Album && 
                                             UIInput.A)
                                 .Subscribe(_ =>
                                 {
                                     ActivateAlbum(false);
                                     Debug.Log("�A���o����\��");
                                 });

        //�y�[�W��i�߂�/�߂��{�^���̏�����o�^
        _pageSwitchButtons[0].OnClickAsObservable().Where(_ => _isOpened)
                                                   .Subscribe(_ => OnPrevPage());
        
        _pageSwitchButtons[1].OnClickAsObservable().Where(_ => _isOpened)
                                                   .Subscribe(_ => OnNextPage());
    }

    /// <summary>
    /// �A���o����UIPanel�̕\��/��\����؂�ւ���
    /// </summary>
    /// <param name="isOpened"> �\�����邩��\���ɂ��邩�̃t���O </param>
    void ActivatePanel(bool isCanOpened)
    {
        _isCanOpened = isCanOpened;

        if (_isCanOpened)
        {
            _albumUIGroup.alpha = 1;
            Debug.Log("�A���o��UI�\��");
        }
        else
        {
            _albumUIGroup.alpha = 0;
            _currentPageIndex = 0;
            Debug.Log("�A���o��UI��\��");
        }
    }

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
        _pageSwitchButtons[0].gameObject.SetActive(true);
        _pageSwitchButtons[1].gameObject.SetActive(true);

        //�S�Ẵy�[�W����x��\���ɂ���
        foreach (var p in _pages)
        {
            p.SetActive(false);
        }

        //�w�肵���y�[�W��\������
        _pages[index].SetActive(true);

        Debug.Log(index);

        //��ԍ��̃y�[�W�̎��͍��̃{�^�����\���ɂ���
        if (index == 0)
        {
            _pageSwitchButtons[0].gameObject.SetActive(false);
        }
        //�Ō�̃y�[�W�̎��͉E�̃{�^�����\���ɂ���
        else if (index == _pages.Length - 1)
        {
            _pageSwitchButtons[1].gameObject.SetActive(false);
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
