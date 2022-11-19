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
    public void ShuffleRequest(ShuffleFase fase, Stage2MugcupController[] mugcups, float duration, Action action = null)
    {
        Debug.Log($"{fase}のShuffle");
        switch (fase)
        {
            case ShuffleFase.One:
                StartCoroutine(Type1ShuffleAsync(mugcups, duration, action));
                break;
            case ShuffleFase.Two:
                StartCoroutine(Type2ShuffleAsync(mugcups, duration, action));
                break;
            case ShuffleFase.Three:
                break;
            default:
                break;
        }
    }

    #region Type1
    /// <summary>
    /// シャッフル
    /// </summary>
    private IEnumerator Type1ShuffleAsync(Stage2MugcupController[] mugcups, float duration, Action action = null)
    {
        var isSwaped1 = false;
        var isSwaped2 = false;

        for (int count = 0; count < _shuffleCount; count++)
        {
            // フラグのリセット
            isSwaped1 = false;
            isSwaped2 = false;

            // 交換するカップのIndexを取得
            var indexes = Type1GetNumber(mugcups.Length);

            // 交換の処理
            Replace(mugcups, indexes.Item1, indexes.Item2, duration / _shuffleCount, () => isSwaped1 = true);
            Replace(mugcups, indexes.Item2, indexes.Item1, duration / _shuffleCount, () => isSwaped2 = true);

            // 配列の中身を入れ替える
            ReplaceForItemOfArray(mugcups, indexes.Item1, indexes.Item2);

            // フラグか切り替わるまで、待つ
            while (!isSwaped1 || !isSwaped2)
            {
                yield return null;
            }
        }
        action?.Invoke();
    }

    /// <summary>
    /// 入れ替える配列のIndexを取得
    /// </summary>
    private (int, int) Type1GetNumber(int length)
    {
        var num1 = UnityEngine.Random.Range(0, length - 1);
        var num2 = num1 + 1;

        if (num2 > 6)
        {
            num2 = 0;
        }

        return (num1, num2);
    }
    #endregion

    #region Type2
    private IEnumerator Type2ShuffleAsync(Stage2MugcupController[] mugcups, float duration, Action action)
    {
        // 入れ替わりが終わったかのフラグ
        var isSwaped = false;

        // 最初は0番にマウスが入ってる ※現状？？
        var hasMouseIndex = 0;

        for (int i = 0; i < _shuffleCount; i++)
        {
            isSwaped = false;

            // 交換するIndex
            var indexes = Type2GetNumber(mugcups.Length, hasMouseIndex);

            // エフェクトの再生
            StartCoroutine(PlayEffects(mugcups, indexes.Item1, indexes.Item2, () => isSwaped = true));

            // 中身の入れ替え
            ReplaceForItemOfArray(mugcups, indexes.Item1, indexes.Item2);

            // フラグか切り替わるまで、待つ
            while (!isSwaped)
            {
                yield return null;
            }
        }

        action?.Invoke();
    }

    /// <summary>
    /// エフェクトの再生
    /// </summary>
    private IEnumerator PlayEffects(Stage2MugcupController[] mugcups, int item1, int item2, Action action = null)
    {
        mugcups[item1].PlayEffect();
        mugcups[item2].PlayEffect();
        yield return new WaitForSeconds(0.5f);
        action?.Invoke();
    }

    private (int, int) Type2GetNumber(int length, int hasMouseIndex)
    {
        var num1 = hasMouseIndex;
        var num2 = UnityEngine.Random.Range(0, length - 1);

        while (num1 == num2)
        {
            num2 = UnityEngine.Random.Range(0, length - 1);
        }

        return (num1, num2);
    }
    #endregion

    /// <summary>
    /// 配列の中身を入れ替える
    /// </summary>
    private void ReplaceForItemOfArray(Stage2MugcupController[] mugcups, int index1, int index2, Action action = null)
    {
        var item = mugcups[index1];
        mugcups[index1] = mugcups[index2];
        mugcups[index2] = item;
        action?.Invoke();
    }

    /// <summary>
    /// 二つのオブジェクトの位置を入れ替える
    /// </summary>
    private void Replace(Stage2MugcupController[] mugcups, int index1, int index2, float duration, Action action = null)
    {
        mugcups[index1].transform.DOLocalMove(mugcups[index2].transform.localPosition, duration).OnComplete(() => action?.Invoke());
    }
}