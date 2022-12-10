using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Stage3UIManager : MonoBehaviour
{
    public enum Type
    {
        OrderUI,
        IngameUI,
        BlackOutImage,
        GameClear
    }

    [SerializeField]
    private GameObject _orderUI;

    [SerializeField]
    private Text _orderText;

    [SerializeField]
    private GameObject _ingameOrder;

    [SerializeField]
    private Text _intgameOrderText;

    [SerializeField]
    private Image _blackOut;

    [SerializeField]
    private GameObject _gameClearPanel;

    /// <summary>
    /// �I�[�_�[�̃e�L�X�g�\��
    /// </summary>
    public void DisplayOrder(Order order)
    {
        Debug.Log($"�I�[�_�[�̕\�� : {order.Name}");
        // �I�[�_�[�̃e�L�X�g���f
        _orderText.text = order.DisplayText;
        // �Q�[�����̃I�[�_�[�̃e�L�X�g���f
        _intgameOrderText.text = order.DisplayText;
    }

    /// <summary>
    /// ����̔�\��
    /// </summary>
    public void ChengeUIActivete(Type type, bool state)
    {
        Debug.Log($"{type} UI�̔�\��");

        switch (type)
        {
            case Type.OrderUI:
                _orderUI.SetActive(state);
                break;
            case Type.IngameUI:
                _ingameOrder.SetActive(state);
                break;
            case Type.BlackOutImage:
                _blackOut.gameObject.SetActive(state);
                break;
            case Type.GameClear:
                _gameClearPanel.SetActive(state);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// �Ó]����
    /// </summary>
    public void BeginBlackOut(float duration)
    {
        _blackOut.DOFade(1f, duration).SetEase(Ease.OutExpo);
    }
}
