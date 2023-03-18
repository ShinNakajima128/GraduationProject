using System;
using UnityEngine;

[Serializable]
public class Order
{
    [SerializeField]
    public string Name;

    [Header("倒すトランプ兵の種類")]
    public CardType TargetType;

    [Header("お題の表示するテキスト")]
    public string DisplayText;

    [Header("倒す数\n片方の場合は上限に気を付ける。")]
    [Range(0,24)]
    public int ClearCount;

    [Header("奇数のお題か")]
    public bool OddNumder;

    [Header("偶数のお題か")]
    public bool EvenNumber;
}