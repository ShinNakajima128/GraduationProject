using System;
using System.Collections;
using UnityEngine;

public class MugcapManager : MonoBehaviour
{
    [SerializeField]
    private MugcapController[] _mugArray;

    [SerializeField]
    private int _shuffleCount;

    private StageGameSystem _shuffle = new StageGameSystem();

    private int[] Numbers;

    private void Awake()
    {
        Numbers = new int[_mugArray.Length];
        for (int i = 0; i < Numbers.Length; i++)
        {
            Numbers[i] = i;
        }
    }

    public void DownRequest(Action action)
    {
        StartCoroutine(Wait(() => Debug.Log("a")));
    }

    private IEnumerator Wait(Action action)
    {
        yield return DownAsync();
        action();
    }

    private IEnumerator DownAsync()
    {
        foreach (var item in _mugArray)
        {
            yield return item.DownAsync();
        }
    }

    public void Shuffle()
    {
        StartCoroutine(_shuffle.ShuffleAsync(_mugArray[0], _mugArray[1]));
    }
}
