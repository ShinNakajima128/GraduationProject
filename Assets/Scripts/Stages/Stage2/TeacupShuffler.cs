using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TeacupShuffler : MonoBehaviour
{
    #region serialize
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

                yield return ShuffleTeacup(null, rand);
            }
        }
    }

    /// <summary>
    /// ティーカップをシャッフルする
    /// </summary>
    /// <param name="index"> シャッフルするカップの番号 </param>
    public IEnumerator ShuffleTeacup(Teacup[] cups,int index, float shuffleTime = 1.5f)
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

        AudioManager.PlaySE(SEType.Stage2_Shuffle);
        //VibrationController.OnVibration(Strength.Low, 0.2f);

        //２つのカップを入れ替えるアニメーションを再生。処理はカップのアニメーションが終わるまで待機
        _teapots[index].DOPath(mainToTargetPath, shuffleTime, PathType.CatmullRom);
        yield return _teapots[targetIndex].DOPath(targetToMainPath, shuffleTime, PathType.CatmullRom)
                                          .WaitForCompletion();

        //タプルで交換したカップの配列位置を入れ替える
        (_teapots[index], _teapots[targetIndex]) = (_teapots[targetIndex], _teapots[index]);

        if (!_debugMode && cups != null)
        {
            (cups[index], cups[targetIndex]) = (cups[targetIndex], cups[index]);
        }

        yield return null;
    }
    public IEnumerator WarpTeacup(Teacup[] cups, int index, float warpTime = 0.5f)
    {
        var randomTarget = index;

        //ワープ先のターゲットが決まるまでループ
        while (index == randomTarget)
        {
            randomTarget = UnityEngine.Random.Range(0, 6);
        }

        EffectManager.PlayEffect(EffectType.Stage2_WarpStart, _teapots[index].transform.position);
        EffectManager.PlayEffect(EffectType.Stage2_WarpStart, _teapots[randomTarget].transform.position);
        AudioManager.PlaySE(SEType.Stage2_Warp);
        VibrationController.OnVibration(Strength.Middle, 0.3f);

        yield return new WaitForSeconds(0.4f);

        _teapots[index].gameObject.SetActive(false);
        _teapots[randomTarget].gameObject.SetActive(false);

        yield return new WaitForSeconds(warpTime);

        EffectManager.PlayEffect(EffectType.Stage2_WarpEnd, _teapots[index].transform.position);
        EffectManager.PlayEffect(EffectType.Stage2_WarpEnd, _teapots[randomTarget].transform.position);
        AudioManager.PlaySE(SEType.Stage2_Warp);
        VibrationController.OnVibration(Strength.Middle, 0.3f);

        yield return new WaitForSeconds(0.4f);

        _teapots[index].gameObject.SetActive(true);
        _teapots[randomTarget].gameObject.SetActive(true);

        var mainPos = _teapots[index].transform.position;
        var targetPos = _teapots[randomTarget].transform.position;

        _teapots[index].DOMove(targetPos, 0f);
        _teapots[randomTarget].DOMove(mainPos, 0f);

        //タプルで交換したカップの配列位置を入れ替える
        (_teapots[index], _teapots[randomTarget]) = (_teapots[randomTarget], _teapots[index]);

        if (!_debugMode && cups != null)
        {
            (cups[index], cups[randomTarget]) = (cups[randomTarget], cups[index]);
        }

        yield return new WaitForSeconds(warpTime);
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