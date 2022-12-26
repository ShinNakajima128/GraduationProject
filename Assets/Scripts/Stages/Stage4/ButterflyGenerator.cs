using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 蝶を生成するComponent
/// </summary>
public class ButterflyGenerator : MonoBehaviour
{
    #region serialize
    [Header("Components")]
    [SerializeField]
    ButterflyManager _butterflyMng = default;
    #endregion

    #region private
    #endregion

    #region public
    #endregion

    #region property
    #endregion

    /// <summary>
    /// バラの木に生成する
    /// </summary>
    /// <param name="trees"> バラの木 </param>
    public void OnGenerate(RoseTree[] trees)
    {
        for (int i = 0; i < trees.Length; i++)
        {
            for (int n = 0; n < trees[i].CurrentRose.Length; n++)
            {
                int random = Random.Range(0, 2);

                //1/2の確率で蝶を生成
                if (random == 0)
                {
                    Debug.Log("蝶生成");

                    var type = trees[i].CurrentRose[n].CurrentRoseType;

                    switch (type)
                    {
                        //バラが非表示の場合はランダムに色を指定して出現
                        case RoseType.Hidden:
                            int randomColor = Random.Range(0, 2);

                            if (randomColor == 0)
                            {
                                _butterflyMng.ActiveFlyingButterfly(ButterflyState.Idle, ButterflyColor.White, trees[i].CurrentRose[n].transform);
                            }
                            else
                            {
                                _butterflyMng.ActiveFlyingButterfly(ButterflyState.Idle, ButterflyColor.Red, trees[i].CurrentRose[n].transform);
                            }
                            break;
                        case RoseType.Red:
                            _butterflyMng.ActiveFlyingButterfly(ButterflyState.Idle, ButterflyColor.White, trees[i].CurrentRose[n].transform);
                            break;
                        case RoseType.White:
                            _butterflyMng.ActiveFlyingButterfly(ButterflyState.Idle, ButterflyColor.Red, trees[i].CurrentRose[n].transform);
                            break;
                        default:
                            break;
                    }
                }
            }

        }
    }

    /// <summary>
    /// 全ての蝶を非アクティブにする
    /// </summary>
    public void Return()
    {
        _butterflyMng.Return();
    }
}
