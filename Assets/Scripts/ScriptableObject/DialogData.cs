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
    #endregion

    #region hide inspector
    [HideInInspector]
    public string Message;
    #endregion

    #region property
    public string[] AllMessage => _messages;
    #endregion
    public void MessagesToArray()
    {
        string[] del = { "\n" };
        _messages = Message.Split(del, StringSplitOptions.None);
    }
}

public class ScenarioMasterDataClass<T>
{
    public T[] Data;
}