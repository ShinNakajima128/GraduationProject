using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MyScriptable/Create IconData")]
public class IconData : ScriptableObject
{
    [SerializeField]
    ActorData[] _actors = default;

    public ActorData[] Actors => _actors;
}

[Serializable]
public struct ActorData
{
    public Actor Actor;
    public Sprite Icon_Default;
    public Sprite Type1;
    public Sprite Type2;
    public Sprite Type3;
}

public enum IconType
{
    Default,
    Type1,
    Type2,
    Type3,
}
