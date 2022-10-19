using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Stages
{
    Stage1,
    Stage2,
    Stage3,
    Stage4,
    Stage5,
    Stage6
}

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    [SerializeField]
    Stages _currentStage = default;
}
