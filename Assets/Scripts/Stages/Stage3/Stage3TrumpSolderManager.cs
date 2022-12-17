using System;
using System.Collections;
using UnityEngine;

public class Stage3TrumpSolderManager : MonoBehaviour
{
    [Header("トランプ兵の前後の間隔")]
    [SerializeField]
    private float _forwardDistance;

    [Header("トランプ兵のズラス幅")]
    [SerializeField]
    private float _gap;

    [Header("斜めに並ぶ時の基準")]
    [SerializeField]
    private Transform _pointOfXSlant;

    [Header("直線上に並ぶ時の基準")]
    [SerializeField]
    private Transform _pointOfStraight;

    [Header("交差に並ぶ時の基準")]
    [SerializeField]
    private Transform _pointOfXrossLeft;

    [SerializeField]
    private Transform _pointOfXrossRight;

    [Space]
    [SerializeField]
    private Stage3ScoreConter _stage3ScoreConter;

    private Stage3TrumpSolderController[] _trumpsArray;

    // 道幅
    const float ROAD_WIDTH = 2;

    /// <summary>
    /// 並ぶ種類
    /// </summary>
    private enum LineUpPattern
    {
        // 直線
        straight,
        // 交差
        Xross,
        // 斜め
        Slant
    }

    private void Start()
    {
        _trumpsArray = GetComponentsInChildren<Stage3TrumpSolderController>();

        foreach (var item in _trumpsArray)
        {
            item.SetCounter(_stage3ScoreConter);
        }
    }

    /// <summary>
    /// 整列のリクエスト
    /// </summary>
    public void RequestSetSolder()
    {
        Debug.Log("整列");
        // 整列の種類を取得
        var num = UnityEngine.Random.Range(0, (int)LineUpPattern.Slant + 1);
        // 整列
        SetSolder(num);
    }

    /// <summary>
    /// 整列の処理
    /// </summary>
    private void SetSolder(int patternLength)
    {
        switch (patternLength)
        {
            case (int)LineUpPattern.straight:
                StartCoroutine(SetStraightAsync());
                break;
            case (int)LineUpPattern.Xross:
                StartCoroutine(SetXrossAsync());
                break;
            case (int)LineUpPattern.Slant:
                StartCoroutine(SetSlantAsync());
                break;
        }
    }

    /// <summary>
    /// 斜めに並ぶ
    /// </summary>
    private IEnumerator SetXrossAsync()
    {
        var leftPoint = _pointOfXrossLeft.position;
        var rightPoint = _pointOfXrossRight.position;

        for (int i = 0; i < _trumpsArray.Length; i++)
        {
            var trump = _trumpsArray[i];

            if (i % 2 == 0)
            {
                trump.GetComponent<TrumpSolder>().ChangeRandomPattern(TrumpColorType.Red);
                trump.gameObject.transform.position = leftPoint;
            }
            else
            {
                trump.GetComponent<TrumpSolder>().ChangeRandomPattern(TrumpColorType.Black);
                trump.gameObject.transform.position = rightPoint;
            }

            leftPoint.z = leftPoint.z + _forwardDistance;
            leftPoint.x = leftPoint.x + _gap;

            rightPoint.z = rightPoint.z + _forwardDistance;
            rightPoint.x = rightPoint.x - _gap;
        }

        yield return null;
    }

    /// <summary>
    /// 直線に並ぶ
    /// </summary>
    private IEnumerator SetStraightAsync()
    {
        var pos = _pointOfStraight.position;

        for (int index = 0; index < _trumpsArray.Length; index++)
        {
            var trump = _trumpsArray[index];

            if (index % 2 == 0)
            {
                // トランプの種類を指定
                trump.GetComponent<TrumpSolder>().ChangeRandomPattern(TrumpColorType.Red);
                _trumpsArray[index].SetCardType(CardType.Red);
            }
            else
            {
                // トランプの種類を指定
                trump.GetComponent<TrumpSolder>().ChangeRandomPattern(TrumpColorType.Black);
                _trumpsArray[index].SetCardType(CardType.Black);
            }

            // トランプの設定
            trump.Reset();
            trump.gameObject.transform.position = pos;
            pos.z = pos.z + _forwardDistance;

            yield return null;
        }
    }

    /// <summary>
    /// 斜めに並ぶ
    /// </summary>
    private IEnumerator SetSlantAsync()
    {
        var pos = _pointOfXSlant.position;

        for (int index = 0; index < _trumpsArray.Length; index++)
        {
            _trumpsArray[index].gameObject.transform.position = pos;
            var card = _trumpsArray[index].gameObject.GetComponent<TrumpSolder>();
            if (index % 2 == 0)
            {
                // トランプの種類を指定
                card.ChangeRandomPattern(TrumpColorType.Red);
                _trumpsArray[index].SetCardType(CardType.Red);
            }
            else
            {
                // トランプの種類を指定
                card.ChangeRandomPattern(TrumpColorType.Black);
                _trumpsArray[index].SetCardType(CardType.Black);
            }
            pos.x = pos.x + _gap;
            pos.z = pos.z + _forwardDistance;
            yield return null;
        }
    }

    public void Reset()
    {
        foreach (var item in _trumpsArray)
        {
            item.gameObject.SetActive(true);
        }
    }
}