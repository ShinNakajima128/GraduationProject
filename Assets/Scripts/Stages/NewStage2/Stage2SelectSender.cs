using System;
using UnityEngine;

internal class Stage2SelectSender : MonoBehaviour
{
    [SerializeField]
    private Stage2GameManager _manager;

    internal void SendSelectNumber(int currentSelectNum)
    {
        _manager.Judge(currentSelectNum);
    }
}