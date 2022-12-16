using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrumpSolderGenerator : MonoBehaviour
{
    #region serialize
    [Header("Variables")]
    [SerializeField]
    GeneratePoint[] _generatePoints = default;

    [Header("Components")]
    [SerializeField]
    Stage4TrumpManager _trumpMng = default;

    [SerializeField]
    RoseTree[] _trees = default;

    [Header("Debug")]
    [SerializeField]
    bool _isAllPosGenerate = false;

    [SerializeField]
    bool _isRootLineViewing = false;

    [SerializeField]
    Transform[] _walkTrumpRootPath = default;
    #endregion

    #region private
    #endregion

    #region public
    #endregion

    #region property
    public int CurrentActiveTrumpCount => _trumpMng.GetTrumpActiveCount();
    #endregion

    /// <summary>
    /// トランプ兵を生成
    /// </summary>
    public void Generate()
    {
        int generateCount;
        bool generateJudge;

        for (int i = 0; i < _generatePoints.Length; i++)
        {
            generateCount = 0;

            for (int n = 0; n < _generatePoints[i].Positions.Length; n++)
            {
                if (!_isAllPosGenerate)
                {
                    if (generateCount >= _generatePoints[i].GenerateMaxCount)
                    {
                        break;
                    }

                    //「塗る」トランプ兵を生成する処理の場合
                    if (i == 2)
                    {
                        //バラの木の「左」のバラが「赤」で出現している場合のみトランプ兵を生成
                        if (_trees[n].CurrentRose[0].CurrentRoseType == RoseType.Red)
                        {
                            _trumpMng.Use((Stage4TrumpDirectionType)i, _generatePoints[i].Positions[n]);
                            generateCount++;
                        }
                    }
                    else
                    {
                        generateJudge = UnityEngine.Random.Range(0, 2) == 0;

                        if (!generateJudge)
                        {
                            continue;
                        }

                        _trumpMng.Use((Stage4TrumpDirectionType)i, _generatePoints[i].Positions[n]);
                        generateCount++;
                    }
                }
                else
                {
                    _trumpMng.Use((Stage4TrumpDirectionType)i, _generatePoints[i].Positions[n]);
                }
                
            }
        }
    }

    /// <summary>
    /// 全て非アクティブにする
    /// </summary>
    public void Return()
    {
        _trumpMng.Return();
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!_isRootLineViewing)
        {
            return;
        }
        Gizmos.color = Color.green;
        Gizmos.DrawLine(_walkTrumpRootPath[0].position, _walkTrumpRootPath[1].position);

    }
#endif
}

[Serializable]
public struct GeneratePoint
{
    public int GenerateMaxCount;
    public Transform[] Positions;
}
