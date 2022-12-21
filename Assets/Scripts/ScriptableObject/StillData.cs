using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MyScriptable/Create StillData")]
public class StillData : ScriptableObject
{
    [SerializeField]
    StageStill[] _stageStills = default;

    public StageStill[] StageStills => _stageStills;
}

/// <summary>
/// ステージ毎のスチル
/// </summary>
[Serializable]
public struct StageStill
{
    public string StillDataName;
    public Stages Stage;
    public Sprite StillSprite;
}
