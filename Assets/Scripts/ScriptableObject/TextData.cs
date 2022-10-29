using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MyScriptable/Create TextData")]
public class TextData : ScriptableObject
{
    #region serialize
    [SerializeField]
    MessageText[] _texts = default;
    #endregion

    #region property
    public MessageText[] Texts => _texts;
    #endregion
}

[Serializable]
public struct MessageText
{
    /// <summary> �b���� </summary>
    public string actor;
    /// <summary> ���b�Z�[�W���e </summary>
    [TextArea(1, 3)]
    public string _message;
}
