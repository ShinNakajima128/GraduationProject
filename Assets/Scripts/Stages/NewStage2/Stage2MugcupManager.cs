using System;
using UnityEngine;

public class Stage2MugcupManager : MonoBehaviour
{
    [SerializeField]
    private Stage2MugcupController[] _mugcups;

    [SerializeField]
    private Stage2MugcupShuffler _shuffler;

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
        _shuffler.ShuffleRequest(fase, _mugcups, 2f, action);
    }
}
