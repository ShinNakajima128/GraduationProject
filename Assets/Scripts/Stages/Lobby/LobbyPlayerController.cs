using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyPlayerController : PlayerBase
{
    #region serialize
    #endregion
    #region private
    #endregion
    #region property
    #endregion

    private void Start()
    {
        _fc.ChangeFaceType(FaceType.Blink);
    }
}
