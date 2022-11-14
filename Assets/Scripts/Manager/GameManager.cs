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
    #region serialize
    [SerializeField]
    Stages _currentStage = default;

    [SerializeField]
    ClockState _currentClockState = ClockState.Zero;
    #endregion
    #region private
    #endregion
    #region property
    public Stages CurrentStage => _currentStage;
    public ClockState CurrentClockState => _currentClockState;
    #endregion
}
