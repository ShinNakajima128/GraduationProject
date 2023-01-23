using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MyScriptable/Create ButtonData")]
public class ButtonData : ScriptableObject
{
    public Sprite SelectSprite;
    public Sprite DeselectSprite;
    public Color SelectTextColor;
    public Color DeselectTextColor;
    public Color TextOutlineColor;
}
