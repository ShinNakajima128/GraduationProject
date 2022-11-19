using System;
using UnityEngine;

public class Stage2MugcupController : MonoBehaviour
{
    [SerializeField]
    private Stage2Mugcup _mugcup;

    [SerializeField]
    private Transform _upTrans;

    [SerializeField]
    private Transform _downTrans;

    public bool IsInMouse { get; set; } = false;

    public void CupDownRequest(Action action)
    {
        _mugcup.MoveRequest(_downTrans.localPosition, 2f, action);
    }
    
    public void PlayEffect()
    {
        
        Debug.Log("エフェクトの再生");
    }
}
