using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MyScriptable/Create TextData")]
public class TextData : ScriptableObject
{
    #region serialize
    [Tooltip("メッセージの種類")]
    [SerializeField]
    MessageType _messageType = default;

    [Tooltip("全メッセージ")]
    [SerializeField]
    MessageText[] _texts = default;

    [Tooltip("メッセージ表示中の背景")]
    [SerializeField]
    Sprite _background = default;
    #endregion

    #region property
    public MessageType MessageType => _messageType;
    public MessageText[] Texts => _texts;
    public Sprite Background => _background;
    #endregion
}

[Serializable]
public struct MessageText
{
    /// <summary> 話し手 </summary>
    public string Actor;
    /// <summary> メッセージ内容 </summary>
    [TextArea(1, 3)]
    public string Message;
}
/// <summary>
/// メッセージの種類
/// </summary>
public enum MessageType
{
    Intro,
    Tutorial
}
