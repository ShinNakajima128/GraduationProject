using System;
using UnityEngine;

public class Stage2MugcupManager : MonoBehaviour
{
    [SerializeField]
    private Stage2MugcupController[] _mugcups;

    [SerializeField]
    private Stage2MugcupShuffler _shuffler;

    [SerializeField]
    private float _shuffleTime;

    /// <summary>
    /// シャッフル前の状態にする
    /// </summary>
    public void Initialise(Action action)
    {
        _mugcups[0].IsInMouse = true;
        _mugcups[0].CupDownRequest(action);
    }

    /// <summary>
    /// シャッフル
    /// </summary>
    public void Shuffle(ShuffleFase fase, Action action = null)
    {
        Debug.Log("シャッフル開始のリクエスト");
        _shuffler.ShuffleRequest(fase, _mugcups, _shuffleTime, action);
    }

    /// <summary>
    /// ネズミが入っている番号を返す
    /// </summary>
    public int GetInMouseCupNumber()
    {
        for (int i = 0; i < _mugcups.Length; i++)
        {
            // ネズミが入っている番号を返す
            if (_mugcups[i].IsInMouse) return i;
        }
        return -1;
    }
}
