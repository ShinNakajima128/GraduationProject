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
    public Sprite Icon;
}
