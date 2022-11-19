using System;
using UnityEngine;
using UnityEngine.UI;

public class OrderManager : MonoBehaviour
{
    [SerializeField]
    private Order[] _orders;

    [SerializeField]
    private Button _agree;

    [SerializeField]
    private GameObject _panel;

    private Order _order;

    public void CreateOrder(Action action = null)
    {
        // �I�[�_�[�̐���
        var index = UnityEngine.Random.Range(0, _orders.Length);
        _order = _orders[index];

        // �{�^���Ƀ��\�b�h��o�^
        _agree.onClick.AddListener(() => 
        {
            action();
            _panel.SetActive(false);
            _agree.gameObject.SetActive(false);
        });

        // �{�^���̗L����
        _agree.gameObject.SetActive(true);
    }
}
