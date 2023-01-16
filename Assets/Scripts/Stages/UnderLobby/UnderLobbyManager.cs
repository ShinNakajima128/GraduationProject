using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnderLobbyManager : MonoBehaviour
{
    #region serialize
    #endregion

    #region private
    #endregion

    #region public
    #endregion

    #region property
    public static UnderLobbyManager Instance { get; private set; }
    #endregion

    private void Awake()
    {
        Instance = this;
    }
}
