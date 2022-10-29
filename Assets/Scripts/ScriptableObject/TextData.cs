using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MyScriptable/Create TextData")]
public class TextData : ScriptableObject
{
    #region serialize
    [Tooltip("���b�Z�[�W�̎��")]
    [SerializeField]
    MessageType _messageType = default;

    [Tooltip("�S���b�Z�[�W")]
    [SerializeField]
    MessageText[] _texts = default;

    [Tooltip("���b�Z�[�W�\�����̔w�i")]
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
    /// <summary> �b���� </summary>
    public string Actor;
    /// <summary> ���b�Z�[�W���e </summary>
    [TextArea(1, 3)]
    public string Message;
}
/// <summary>
/// ���b�Z�[�W�̎��
/// </summary>
public enum MessageType
{
    Intro,
    Tutorial
}
