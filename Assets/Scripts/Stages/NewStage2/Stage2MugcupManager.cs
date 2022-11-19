using System;
using System.Linq;
using UnityEngine;

public class Stage2MugcupManager : MonoBehaviour
{
    [SerializeField]
    private Stage2MugcupController[] _mugcups;

    [SerializeField]
    private Stage2MugcupShuffler _shuffler;

    [Header("最初にネズミが入ってる場所(Index)")]
    [SerializeField]
    private int _indexOfInMouse;

    [Header("シャッフルの時間")]
    [SerializeField]
    private float _shuffleTime;

    private Vector3[] _defaultPositions;

    private void Awake()
    {
        _defaultPositions = new Vector3[_mugcups.Length];

        for (int index = 0; index < _mugcups.Length; index++)
        {
            _mugcups[index].ID = index;
            _defaultPositions[index] = _mugcups[index].transform.position;
        }
    }

    /// <summary>
    /// シャッフル前の状態にする
    /// </summary>
    public void Initialise(Action action)
    {
        _mugcups[_indexOfInMouse].IsInMouse = true;
        _mugcups[_indexOfInMouse].CupDownRequest(action);
    }

    /// <summary>
    /// 配列の中身を初期状態に戻る
    /// </summary>
    public void ResetForArray()
    {
        // ID順にソート
        _mugcups = _mugcups.OrderBy(item => item.ID).ToArray();

        // 座標の初期化
        for (int index = 0; index < _mugcups.Length; index++)
        {
            _mugcups[index].transform.position = _defaultPositions[index];
        }
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
