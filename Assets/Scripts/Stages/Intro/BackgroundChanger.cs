using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// �C���g��Scene�̔w�i�𑀍삷��R���|�[�l���g
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
