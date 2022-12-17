using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TeapotShuffle : MonoBehaviour
{
    #region serialize
    [Header("Variables")]
    [Tooltip("シャッフルにかける時間")]
    [SerializeField]
    float _shuffleTime = 1.5f;

    [Header("Objects")]
    [Tooltip("各ティーポットのTransform")]
    [SerializeField]
    Transform[] _teapots = default;

    [Tooltip("ティーポットのアニメーションPath")]
    [SerializeField]
    ShufflePath[] _shufflePaths = default;

    [Header("Debug")]
    [SerializeField]
    bool _debugMode = false;
    #endregion
    #region private
    #endregion
    #region public
    /// <summary> 現在のシャッフルにかける時間 </summary>
    public float CurrentShuffleTime { get => _shuffleTime; set => _shuffleTime = value; }
    #endregion
    #region property
    #endregion

    //メモ
    //シャッフルする関数に指定する数字は0～5まで

    IEnumerator Start()
    {
        if (_debugMode)
        {
            while (true)
            {
                int rand = UnityEngine.Random.Range(0, 6);

                yield return ShuffleTeapot(null, rand);
            }
        }
    }

    /// <summary>
    /// ティーカップをシャッフルする
    /// </summary>
    /// <param name="index"> シャッフルするカップの番号 </param>
    public IEnumerator ShuffleTeapot(Stage2MugcupController[] mugcups,int index)
    {
        var mainPositionType = (TeapotPositionType)index;  //メインのIndexを位置の種類へ変換
        var mainRandomTarget = (ReplaceTargetType)UnityEngine.Random.Range(0, 2); //交換先のカップを前にするか次にするかランダムで決める

        //メインからターゲットへのPathを取得
        var mainToTargetPath = _shufflePaths.Where(t => t.PositionType == mainPositionType)
                                .FirstOrDefault(p => p.TargetType == mainRandomTarget)
                                .PathTrans.Select(x => x.transform.position).ToArray();

        int targetIndex = default;
        ReplaceTargetType targetToMainType = default;
        Vector3[] targetToMainPath = default;

        //メインの交換先が「Next」の場合は1つ先のTeaCupの「Prev」のPathを取得
        if (mainRandomTarget == ReplaceTargetType.Next)
        {
            //最後の要素がメインだった場合は「0」に戻る
            if (index == 5)
            {
                targetIndex = 0;
            }
            else
            {
                targetIndex = index + 1;
            }
            targetToMainType = ReplaceTargetType.Previous;
        }
        else
        {
            if (index == 0)
            {
                targetIndex = 5;
            }
            else
            {
                targetIndex = index - 1;
            }
            targetToMainType = ReplaceTargetType.Next;
        }

        //ターゲットからメインへのPathを取得
        targetToMainPath = _shufflePaths.Where(t => t.PositionType == (TeapotPositionType)targetIndex)
                                .FirstOrDefault(p => p.TargetType == targetToMainType)
                                .PathTrans.Select(x => x.transform.position).ToArray();

        //２つのカップを入れ替えるアニメーションを再生。処理はカップのアニメーションが終わるまで待機
        _teapots[index].DOPath(mainToTargetPath, _shuffleTime, PathType.CatmullRom);
        yield return _teapots[targetIndex].DOPath(targetToMainPath, _shuffleTime, PathType.CatmullRom)
                                          .WaitForCompletion();

        //タプルで交換したカップの配列位置を入れ替える
        (_teapots[index], _teapots[targetIndex]) = (_teapots[targetIndex], _teapots[index]);

        if (!_debugMode)
        {
            (mugcups[index], mugcups[targetIndex]) = (mugcups[targetIndex], mugcups[index]);
        }

        yield return null;
    }
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        //処理が重くなるため、デバッグモード中以外は描写しない
        if (!_debugMode)
        {
            return;
        }

        Gizmos.color = Color.red;

        for (int i = 0; i < _shufflePaths.Length; i++)
        {
            for (int n = 0; n < _shufflePaths[i].PathTrans.Length - 1; n++)
            {
                Gizmos.DrawLine(_shufflePaths[i].PathTrans[n].position, _shufflePaths[i].PathTrans[n + 1].position);
            }
        }
    }
#endif
}

/// <summary>
/// ティーカップの位置の種類
/// </summary>
public enum TeapotPositionType
{
    UpperRight,
    Right,
    LowerRight,
    LowerLeft,
    Left,
    UpperLeft
}

/// <summary>
/// ターゲットがどちらにあるかの種類
/// </summary>
public enum ReplaceTargetType
{
    Previous, //前
    Next      //次
}

/// <summary>
/// シャッフル時に扱うPathデータ
/// </summary>
[Serializable]
public struct ShufflePath
{
    public string PathName;
    public TeapotPositionType PositionType;
    public ReplaceTargetType TargetType;
    public Transform[] PathTrans;
}