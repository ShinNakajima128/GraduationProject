using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 柱を生成する機能を持つコンポーネント
/// </summary>
public class FallPoleGenerator : MonoBehaviour
{
    #region serialize
    [Header("Variables")]
    [SerializeField]
    int _generateCount = 3;

    [SerializeField]
    Transform[] _generatePoints = default;

    [Header("Components")]
    [SerializeField]
    FallPoleController _fallpoleCtrl = default;
    #endregion

    #region private
    #endregion

    #region public
    #endregion

    #region property
    #endregion

    /// <summary>
    /// 柱を生成する
    /// </summary>
    /// <param name="generateCount"> 生成する数 </param>
    public void Generate(int generateCount = 0)
    {
        //生成数が指定されていない場合はGeneratorに設定されている数を反映
        if (generateCount == 0)
        {
            generateCount = _generateCount;
        }
        else if (generateCount > _generatePoints.Length)
        {
            generateCount = _generatePoints.Length;
        }

        for (int i = 0; i < generateCount; i++)
        {
            _fallpoleCtrl.Use(_generatePoints[i]);
        }
    }

    /// <summary>
    /// 使用中の柱を全て非アクティブにする
    /// </summary>
    public void Return()
    {
        _fallpoleCtrl.Return();
    }
}
