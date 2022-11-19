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

    private Vector3[] _defaultPositions;

    private void Awake()
    {
        _defaultPositions = new Vector3[_mugcups.Length];

        for (int i = 0; i < _mugcups.Length; i++)
        {
            _mugcups[i].ID = i;
            _defaultPositions[i] = _mugcups[i].transform.position;
        }
    }

    /// <summary>
    /// シャッフル前の状態にする
    /// </summary>
    public void Initialise(Action action)
    {
        _mugcups[0].IsInMouse = true;
        _mugcups[0].CupDownRequest(action);
    }

    /// <summary>
    /// 配列の中身を初期状態に戻る
    /// </summary>
    public void ResetForArray()
    {
        Debug.Log("Reset");
        for (int i = 0; i < _mugcups.Length; i++)
        {
            for (int x = 0; x < _mugcups.Length; x++)
            {
                if (i == _mugcups[x].ID)
                {
                    var tmp = _mugcups[i];
                    _mugcups[i] = _mugcups[x];
                    _mugcups[x] = tmp;
                    break;
                }
            }
            // 座標のリセット
            _mugcups[i].gameObject.transform.position = _defaultPositions[i];
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
