using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeacupSelecter : MonoBehaviour
{
    #region serialize
    [SerializeField]
    TeacupSelectButton[] _buttons = default;
    #endregion

    #region private
    #endregion

    #region public
    #endregion

    #region property
    public TeacupSelectButton[] SelectButtons => _buttons;
    #endregion
}