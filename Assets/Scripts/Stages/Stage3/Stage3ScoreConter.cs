using UnityEngine;

internal class Stage3ScoreConter : MonoBehaviour
{
    public int HitedCount { get; private set; }
    public int HitedRedCount { get; private set; }
    public int HitedBlackCount { get; private set; }

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
    }
}