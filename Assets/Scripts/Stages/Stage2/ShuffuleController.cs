using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// �X�e�[�W2�̃e�B�[�J�b�v���V���b�t������Controller
/// </summary>
public class ShuffuleController : MonoBehaviour
{
    [Tooltip("�e�B�[�J�b�v")]
    [SerializeField]
    Transform[] _teacups = default;

    [Tooltip("�e�B�[�J�b�v�ɉB���L�����N�^�[")]
    [SerializeField]
    Transform _hidingCharacter = default;

    [Tooltip("�V���b�t�������")]
    [SerializeField]
    int _shuffuleCount = 10;

    [Tooltip("�V���b�t���̏����X�s�[�h")]
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
