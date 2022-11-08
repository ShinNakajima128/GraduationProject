using System;
using UnityEngine;

public class Stage2MugcapManager : MonoBehaviour
{
    [SerializeField] // [0]はマウスが入っている
    private Stage2MugcapController[] _mugcapArray;

    private Stage2MugcapShuffle _shuffle;

    private void Awake()
    {
        _shuffle = new();
    }

    public void Setup(Action action = null)
    {
        // カップを下げる
        _mugcapArray[0].MoveDownRequest(action);
    }
}
