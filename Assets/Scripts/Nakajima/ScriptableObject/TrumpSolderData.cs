using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MyScriptable/Create TrumpSolderData")]
public class TrumpSolderData : ScriptableObject
{
    [SerializeField]
    Trump[] _trumps = default;

    public Trump[] Trumps => _trumps;
}

[Serializable]
public class Trump
{
    public Stage4TrumpType TrumpType;
    public Sprite TrumpSprite;
        
}

public enum Stage4TrumpType
{
    Trump_Red,
    Trump_Black,
    OFF
}

//public enum TrumpType
//{
//    Spade_A,
//    Spade_One,
//    Spade_Two,
//    Spade_Three,
//    Spade_Four,
//    Spade_Five,
//    Spade_Six,
//    Spade_Seven,
//    Spade_Eight,
//    Spade_Nine,
//    Spade_Ten,
//    Spade_J,
//    Spade_Q,
//    Spade_K,
//    Heart_A,
//    Heart_Two,
//    Heart_Three,
//    Heart_Four,
//    Heart_Five,
//    Heart_Six,
//    Heart_Seven,
//    Heart_Eight,
//    Heart_Nine,
//    Heart_Ten,
//    Heart_J,
//    Heart_Q,
//    Heart_K,
//    Diamond_A,
//    Diamond_Two,
//    Diamond_Three,
//    Diamond_Four,
//    Diamond_Five,
//    Diamond_Six,
//    Diamond_Seven,
//    Diamond_Eight,
//    Diamond_Nine,
//    Diamond_Ten,
//    Diamond_J,
//    Diamond_Q,
//    Diamond_K,
//    Club_A,
//    Club_Two,
//    Club_Three,
//    Club_Four,
//    Club_Five,
//    Club_Six,
//    Club_Seven,
//    Club_Eight,
//    Club_Nine,
//    Club_Ten,
//    Club_J,
//    Club_Q,
//    Club_K,
//    Joker,
//    All_NUM
//}
