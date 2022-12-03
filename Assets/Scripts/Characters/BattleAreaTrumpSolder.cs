using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleAreaTrumpSolder : TrumpSolder
{
    #region serialize
    [Tooltip("”z’u‚³‚ê‚½ˆÊ’u")]
    [SerializeField]
    DirectionType _dirType = default;
    #endregion
    #region private
    #endregion
    #region public
    #endregion
    #region property
    public DirectionType DirType { get => _dirType; set => _dirType = value; }
    #endregion

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
    }
}
