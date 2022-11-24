using System;
using UnityEngine;

public class OrderManager : MonoBehaviour
{
    [SerializeField]
    private Order[] _orders;

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
}
