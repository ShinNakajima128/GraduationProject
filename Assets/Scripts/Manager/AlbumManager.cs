using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class AlbumManager : MonoBehaviour
{
    #region serialize
    [Header("UI")]
    [SerializeField]
    CanvasGroup _albumPanelGroup = default;

    [SerializeField]
    Transform _albumTrans = default;
    #endregion

    #region private
    bool _isCanOpened = false;
    bool _isOpened = false;
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
    }

    /// <summary>
    /// �A���o���̃p�l���̕\��/��\����؂�ւ���
    /// </summary>
    /// <param name="isOpened"> �\�����邩��\���ɂ��邩�̃t���O </param>
    void ActivatePanel(bool isCanOpened)
    {
        _isCanOpened = isCanOpened;

        if (_isOpened)
        {
            _albumPanelGroup.alpha = 1;
        }
        else
        {
            _albumPanelGroup.alpha = 0;
        }
    }

    void ActiveAlbum(bool isOpened)
    {
        _isOpened = isOpened;

        if (_isOpened)
        {

        }
    }
}
