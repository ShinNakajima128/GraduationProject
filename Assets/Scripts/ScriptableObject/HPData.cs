using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MyScriptable/Create HPData")]
public class HPData : ScriptableObject
{
    #region serialize
    [SerializeField]
    Sprite _onSprite = default;

    [SerializeField]
    Sprite _offSprite = default;
    #endregion

    #region private
    #endregion

    #region public
    #endregion

    #region property
    public Sprite ONSprite => _onSprite;
    public Sprite OFFSprite => _offSprite;
    #endregion
}
