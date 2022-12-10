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
    /// オーダーのテキスト表示
    /// </summary>
    public void DisplayOrder(Order order)
    {
        Debug.Log($"オーダーの表示 : {order.Name}");
        // オーダーのテキスト反映
        _orderText.text = order.DisplayText;
        // ゲーム中のオーダーのテキスト反映
        _intgameOrderText.text = order.DisplayText;
    }

    /// <summary>
    /// お題の非表示
    /// </summary>
    public void ChengeUIActivete(Type type, bool state)
    {
        Debug.Log($"{type} UIの非表示");

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
    /// 暗転する
    /// </summary>
    public void BeginBlackOut(float duration)
    {
        _blackOut.DOFade(1f, duration).SetEase(Ease.OutExpo);
    }
}
