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
    /// お題の作成
    /// </summary>
    public Order CreateOrder(Action action = null)
    {
        var index = UnityEngine.Random.Range(0, _orders.Length);
        _order = _orders[index];
        _scoreConter.DisplayTarget = _order.TargetType;
        return _order;
    }

    /// <summary>
    /// ゲームのクリア判定
    /// </summary>
    public bool IsCameClear()
    {
        switch (_order.TargetType)
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
