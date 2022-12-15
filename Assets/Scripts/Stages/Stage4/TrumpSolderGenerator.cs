using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrumpSolderGenerator : MonoBehaviour
{
    #region serialize
    [SerializeField]
    TrumpSolderController _trumpCtrl = default;

    [SerializeField]
    int _generateMaxCount = 5;

    [SerializeField]
    GeneratePoint[] _points = default;
    #endregion
    #region private
    #endregion
    #region public
    #endregion
    #region property
    #endregion

    /// <summary>
    /// トランプ兵を生成
    /// </summary>
    public void Generate()
    {
        for (int i = 0; i < _points.Length; i++)
        {
            int generateCount = UnityEngine.Random.Range(0, _generateMaxCount);

            if (generateCount <= 0)
            {
                continue;
            }
            for (int n = 0; n < generateCount; n++)
            {
                int randomIndex = UnityEngine.Random.Range(0, _points[i].Positions.Length);

                _trumpCtrl.Use(_points[i].Positions[randomIndex].position);
            }
        }
    }
    /// <summary>
    /// 全て非アクティブにする
    /// </summary>
    public void Return()
    {
        _trumpCtrl.Return();
    }
}

[Serializable]
public struct GeneratePoint
{
    public Transform[] Positions;
}
