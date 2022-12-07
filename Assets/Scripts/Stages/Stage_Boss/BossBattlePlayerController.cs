using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class BossBattlePlayerController : PlayerBase
{
    #region serialize
    [Header("Variables")]
    [SerializeField]
    int _maxHP = 5;

    #endregion
    #region private
    ReactiveProperty<int> _currentHP = new ReactiveProperty<int>();
    #endregion
    #region public
    #endregion
    #region property
    #endregion

    protected override void Awake()
    {
        base.Awake();
        _currentHP.Value = _maxHP;
    }
    void Start()
    {
        _fc.ChangeFaceType(FaceType.Blink);
    }
}
