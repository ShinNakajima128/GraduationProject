using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MyScriptable/Create MaskData")]
public class MaskData : ScriptableObject
{
    #region serialize
    [SerializeField]
    TransitionMask[] _masks = default;
    #endregion

    #region private
    #endregion

    #region public
    #endregion

    #region property
    public TransitionMask[] Masks => _masks;
    #endregion
}
/// <summary>
/// マスクの種類
/// </summary>
public enum MaskType
{
    CheshireCat,
    Heart,
    KeyHole
}

/// <summary>
/// 画面フェードのマスクデータ
/// </summary>
[Serializable]
public struct TransitionMask
{
    public string MaskName;
    public MaskType MaskType;
    public Texture MaskTexture;
    public Color MaskColor;
}
