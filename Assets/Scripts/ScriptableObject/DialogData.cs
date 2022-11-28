using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DialogData
{
    #region view inspector
    public int Id;
    public string Actor;
    [SerializeField]
    string[] _messages;
    [SerializeField]
    ScreenEffectType _screenEffectType = default;
    #endregion

    #region hide inspector
    [HideInInspector]
    public string Message;
    [HideInInspector]
    public string ScreenEffect;
    #endregion

    #region property
    public string[] AllMessage => _messages;
    public ScreenEffectType EffectType => _screenEffectType;
    #endregion
    public void MessagesToArray()
    {
        string[] del = { "\n" };
        _messages = Message.Split(del, StringSplitOptions.None);
        _screenEffectType = (ScreenEffectType)Enum.Parse(typeof(ScreenEffectType), ScreenEffect, true);
    }                                                 
}

public class ScenarioMasterDataClass<T>
{
    public T[] Data;
}
public enum ScreenEffectType
{
    None,
    CameraShake,
    ConcentratedLine,
    Closeup,
    Reset
}