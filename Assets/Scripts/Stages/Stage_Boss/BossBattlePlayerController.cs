using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBattlePlayerController : PlayerBase
{
    void Start()
    {
        _fc.ChangeFaceType(FaceType.Blink);
    }
}
