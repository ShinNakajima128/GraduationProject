using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlbumManager : MonoBehaviour
{
    #region serialize
    [Header("UI")]
    [SerializeField]
    CanvasGroup _albumPanelGroup = default;
    #endregion

    #region private
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
}
