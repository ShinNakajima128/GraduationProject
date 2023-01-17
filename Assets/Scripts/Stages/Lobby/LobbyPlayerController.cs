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
        EventManager.ListenEvents(Events.FinishTalking, () => _fc.ChangeFaceType(FaceType.Blink));
        EventManager.ListenEvents(Events.Alice_Surprised, () => _fc.ChangeFaceType(FaceType.Fancy));
        EventManager.ListenEvents(Events.Alice_Yes, () => _fc.ChangeFaceType(FaceType.Smile));
        EventManager.ListenEvents(Events.Alice_No, () => _fc.ChangeFaceType(FaceType.Angry));
    }
}
