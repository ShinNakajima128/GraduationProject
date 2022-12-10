using System;
using System.Collections;
using UnityEngine;

public class CardmanManager : MonoBehaviour
{
    [Header("カードの間隔")]
    [SerializeField]
    private float _distance;

    [SerializeField]
    private float _gap;

    [SerializeField]
    private Transform _origin;

    [SerializeField]
    private Stage3ScoreConter _stage3ScoreConter;

    private CardManController[] _cardmanArray;

    private void Start()
    {
        _cardmanArray = GetComponentsInChildren<CardManController>();
        StartCoroutine(SetCardAsync());
    }

    private IEnumerator SetCardAsync()
    {
        var setPos = _origin.position;

        for (int index = 0; index < _cardmanArray.Length; index++)
        {
            _cardmanArray[index].gameObject.transform.position = setPos;
            // 参照の追加
            _cardmanArray[index].SetCounter(_stage3ScoreConter);

            var card = _cardmanArray[index].gameObject.GetComponent<TrumpSolder>();
            if (index % 2 == 0)
            {
                card.ChangeRandomPattern(TrumpColorType.Red);
                _cardmanArray[index].SetCardType(CardType.Red);
            }
            else
            {
                card.ChangeRandomPattern(TrumpColorType.Black);
                _cardmanArray[index].SetCardType(CardType.Black);
            }
            setPos.x = setPos.x + _gap;
            setPos.z = setPos.z + _distance;
            yield return null;
        }
    }

    public void Reset()
    {
        foreach (var item in _cardmanArray)
        {
            item.gameObject.SetActive(true);
        }
    }
}