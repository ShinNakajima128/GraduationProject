using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MyScriptable/Create RoseData")]
public class RoseData : ScriptableObject
{
    #region serialize
    [SerializeField]
    Sprite[] _roseSprites = default;
    #endregion
    #region private
    #endregion
    #region property
    public Sprite[] RoseSprites => _roseSprites;
    #endregion
}
