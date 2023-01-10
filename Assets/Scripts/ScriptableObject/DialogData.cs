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
    Events _screenEffectType = default;
    [SerializeField]
    float _displayTime = 0.05f;
    #endregion

    #region hide inspector
    [HideInInspector]
    public string Message;
    [HideInInspector]
    public string ScreenEffect;
    #endregion

    #region property
    public string[] AllMessage => _messages;
    public Events EventType => _screenEffectType;
    public float DisplayTime => _displayTime;
    #endregion
    public void MessagesToArray()
    {
        string[] del = { "\n" };
        _messages = Message.Split(del, StringSplitOptions.None);
        _screenEffectType = (Events)Enum.Parse(typeof(Events), ScreenEffect, true);
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