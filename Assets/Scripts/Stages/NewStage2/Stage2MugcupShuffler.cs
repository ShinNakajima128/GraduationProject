using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;

public class Stage2MugcupShuffler : MonoBehaviour
{
    [Header("シャッフルの回数")]
    [SerializeField]
    private int _shuffleCount;

    /// <summary>
    /// シャッフルのリクエスト
    /// </summary>
    public void ShuffleRequest(ShuffleFase fase,Stage2MugcupController[] mugcups, float duration, Action action = null)
    {
        switch (fase)
        {
            case ShuffleFase.One:
                StartCoroutine(Type1ShuffuleAsync(mugcups, duration, action));
                break;
            case ShuffleFase.Two:
                break;
            case ShuffleFase.Three:
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// シャッフル
    /// </summary>
    private IEnumerator Type1ShuffuleAsync(Stage2MugcupController[] mugcups, float duration, Action action = null)
    {
        var a = false;
        var b = false;

        for (int count = 0; count < _shuffleCount; count++)
        {
            // フラグのリセット
            a = false;
            b = false;

            // 交換するカップのIndexを取得
            var index = GetNumber(mugcups.Length);

            // 交換の処理
            Replace(mugcups, index.Item1, index.Item2, duration, () => a = true);
            Replace(mugcups, index.Item2, index.Item1, duration, () => b = true);

            // 配列の中身を入れ替える
            ReplaceForItemOfArray(mugcups, index.Item1, index.Item2);

            // フラグか切り替わるまで、待つ
            while (!a || !b)
            {
                yield return null;
            }

            yield return null;
        }
        action?.Invoke();
    }

    /// <summary>
    /// 配列の順番を入れ替える
    /// </summary>
    private void ReplaceForItemOfArray(Stage2MugcupController[] mugcups, int index1, int index2)
    {
        var item = mugcups[index1];
        mugcups[index1] = mugcups[index2];
        mugcups[index2] = item;
    }

    /// <summary>
    /// 入れ替える配列のIndexを取得
    /// </summary>
    private (int, int) GetNumber(int max)
    {
        var num1 = UnityEngine.Random.Range(0, max - 1);
        var num2 = num1 + 1;
        if (num2 > 6)
        {
            num2 = 0;
        }

        return (num1, num2);
    }

    /// <summary>
    /// 二つのオブジェクトの位置を入れ替える
    /// </summary>
    private void Replace(Stage2MugcupController[] mugcups, int index1, int index2, float duration, Action action = null)
    {
        mugcups[index1].transform.DOLocalMove(mugcups[index2].transform.localPosition, duration).OnComplete(() => action?.Invoke());
    }
}