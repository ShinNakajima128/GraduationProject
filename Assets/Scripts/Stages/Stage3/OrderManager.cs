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
    /// ����̍쐬
    /// </summary>
    public Order CreateOrder(Action action = null)
    {
        Debug.Log("�I�[�_�[�̍쐬");
        // �I�[�_�[�̐��� ��0�͕\�����o�O���Ă邩��1����
        var index = UnityEngine.Random.Range(1, _orders.Length);
        _order = _orders[index];
        return _order;
    }

    public bool CheckGameScore()
    {
        switch (_order.CardType)
        {
            case CardType.None:
                break;
            case CardType.Both:
                if (_scoreConter.HitedCount >= _order.Count)
                {
                    Debug.Log("Clear");
                    return true;
                }
                break;
            case CardType.Red:
                if (_scoreConter)
                {

                }
                break;
            case CardType.Black:
                break;
            default:
                break;
        }
        return false;
    }
}
