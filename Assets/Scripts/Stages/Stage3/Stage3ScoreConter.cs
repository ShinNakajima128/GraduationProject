using UnityEngine;
using UnityEngine.UI;

public class Stage3ScoreConter : MonoBehaviour
{
    [SerializeField]
    private GameObject _counterUI;

    // テキスト
    private Text _counterText;

    public CardType DisplayTarget { get; set; } = CardType.None;

    public int HitedCount { get; private set; }
    public int HitedRedCount { get; private set; }
    public int HitedBlackCount { get; private set; }

    private void Start()
    {
        _counterUI.gameObject.SetActive(false);
        _counterText = _counterUI.GetComponentInChildren<Text>();
        _counterText.text = "0体";
    }

    /// <summary>
    /// カウンターの状態を切り替える
    /// </summary>
    public void ChengeCounterActivate(bool state)
    {
        _counterUI.gameObject.SetActive(state);
    }

    /// <summary>
    /// スコアを加算
    /// </summary>
    public void AddCount(CardType cardType)
    {
        HitedCount++;
        switch (cardType)
        {
            case CardType.None:
                break;
            case CardType.Red:
                HitedRedCount++;
                break;
            case CardType.Black:
                HitedBlackCount++;
                break;
            default:
                break;
        }

        SetCounter();
    }

    public void Reset()
    {
        _counterText.text = "0体";
    }

    /// <summary>
    /// テキストに倒した数を表示
    /// </summary>
    public void SetCounter()
    {
        switch (DisplayTarget)
        {
            case CardType.None:
                break;
            case CardType.Both:
                _counterText.text = HitedCount.ToString() + "体";
                break;
            case CardType.Red:
                _counterText.text = HitedRedCount.ToString() + "体";
                break;
            case CardType.Black:
                _counterText.text = HitedBlackCount.ToString() + "体";
                break;
            default:
                break;
        }

    }
}