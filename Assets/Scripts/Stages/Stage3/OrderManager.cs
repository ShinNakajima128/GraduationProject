using System;
using UnityEngine;

public class OrderManager : MonoBehaviour
{
    [SerializeField]
    private Order[] _orders;

    [SerializeField]
    private Stage3ScoreConter _scoreConter;

    private Order _order;

    /// <summary>
    /// ‚¨‘è‚Ìì¬
    /// </summary>
    public Order CreateOrder(Action action = null)
    {
        var index = UnityEngine.Random.Range(0, _orders.Length);
        _order = _orders[index];
        return _order;
    }

    /// <summary>
    /// ƒQ[ƒ€‚ÌƒNƒŠƒA”»’è
    /// </summary>
    public bool IsCameClear()
    {
        switch (_order.TargetCardType)
        {
            case CardType.None:
                break;
            case CardType.Both:
                if (_scoreConter.HitedCount >= _order.ClearCount)
                {
                    Debug.Log("Clear");
                    return true;
                }
                break;
            case CardType.Red:
                if (_scoreConter.HitedRedCount >= _order.ClearCount)
                {
                    Debug.Log("Clear");
                    return true;
                }
                break;
            case CardType.Black:
                if (_scoreConter.HitedBlackCount >= _order.ClearCount)
                {
                    Debug.Log("Clear");
                    return true;
                }
                break;
            default:
                break;
        }
        Debug.Log("Faild");
        return false;
    }
}
