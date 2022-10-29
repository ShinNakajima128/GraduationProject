using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// イントロSceneの背景を操作するコンポーネント
/// </summary>
public class BackgroundChanger : MonoBehaviour
{
    [SerializeField]
    Image[] _backgrounds = default;

    [SerializeField]
    float _changeTime = 2.0f;

    void Start()
    {
        
    }
}
