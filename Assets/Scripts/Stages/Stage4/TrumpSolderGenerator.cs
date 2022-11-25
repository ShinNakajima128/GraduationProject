using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrumpSolderGenerator : MonoBehaviour
{
    #region serialize
    [SerializeField]
    GeneratePoint[] _points = default;
    #endregion
    #region private
    #endregion
    #region public
    #endregion
    #region property
    #endregion
}

[Serializable]
public struct GeneratePoint
{
    public Transform[] Positions;
}
