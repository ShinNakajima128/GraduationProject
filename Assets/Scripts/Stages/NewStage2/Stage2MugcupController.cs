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

    public int ID { get; set; }

    public bool IsInMouse { get; set; } = false;

    public void CupDownRequest(Action action = null)
    {
        _mugcup.MoveRequest(_downTrans.localPosition, 2f, action);
    }

    public void CupUpRequest(Action action = null)
    {
        _mugcup.MoveRequest(_upTrans.localPosition, 2f, action);
    }

    public void PlayEffect()
    {
        var pos = this.transform.position;
        EffectManager.PlayEffect(EffectType.Swap, pos);
    }
}
