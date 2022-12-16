using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MyScriptable/Create RoseData")]
public class RoseData : ScriptableObject
{
    #region serialize
    [SerializeField]
    GameObject[] _roseObject = default;
    #endregion
    #region private
    #endregion
    #region property
    public GameObject[] RoseObject => _roseObject;
    #endregion
}
