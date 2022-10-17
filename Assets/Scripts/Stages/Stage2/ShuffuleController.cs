using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// ステージ2のティーカップをシャッフルするController
/// </summary>
public class ShuffuleController : MonoBehaviour
{
    [Tooltip("ティーカップ")]
    [SerializeField]
    Transform[] _teacups = default;

    [Tooltip("ティーカップに隠れるキャラクター")]
    [SerializeField]
    Transform _hidingCharacter = default;

    [Tooltip("シャッフルする回数")]
    [SerializeField]
    int _shuffuleCount = 10;

    [Tooltip("シャッフルの初期スピード")]
    [SerializeField]
    float _initualShuffuleSpeed = 2.0f;

    private void Start()
    {
        StartCoroutine(InGame());
    }

    void Shuffule()
    {

    }

    IEnumerator InGame()
    {
        yield return null;
    }
}
