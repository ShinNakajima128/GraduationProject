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
        Debug.Log("オーダーの作成");
        // オーダーの生成 ※0は表示がバグってるから1から
        var index = UnityEngine.Random.Range(1, _orders.Length);
        _order = _orders[index];
        return _order;
    }

    /// <summary>
    /// ゲームのクリア判定
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
