using System;
using UnityEngine;

public class OrderManager : MonoBehaviour
{
    [SerializeField]
    private Order[] _orders;

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
}
