using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// �`�F�V���L�̋@�\�����R���|�[�l���g
/// </summary>
public class CheshireCat : MonoBehaviour
{
    #region serialize
    [Header("Variables")]
    [SerializeField]
    float _moveSpeed = 1.5f;

    [SerializeField]
    CheshireCatState _currentState = default;
    #endregion

    #region private
    Animator _anim;
    Coroutine _currentCoroutine;
    #endregion

    #region public
    #endregion

    #region property
    #endregion

    private void Awake()
    {
        TryGetComponent(out _anim);
    }

    private void Start()
    {
        StartCoroutine(IdleCoroutine());
    }

    /// <summary>
    /// �A�C�h�����[�V�����̃R���[�`���B�e�X�g�p
    /// </summary>
    /// <returns></returns>
    IEnumerator IdleCoroutine()
    {
        int random;

        while (true)
        {
            random = Random.Range(5, 8);

            yield return new WaitForSeconds(random);

            _anim.CrossFadeInFixedTime(CheshireCatState.Idle_Lick.ToString(), 0.1f);

            yield return new WaitForSeconds(2.3f);

            _anim.CrossFadeInFixedTime(CheshireCatState.Idle.ToString(), 0.25f);
        }
    }

    /// <summary>
    /// �w�肵���X�e�[�^�X�̃A�N�V���������s
    /// </summary>
    /// <param name="state"> �X�e�[�^�X�̎�� </param>
    void OnAction(CheshireCatState state)
    {
        //�ȑO�̃X�e�[�^�X�̃R���[�`���𒆒f
        if (_currentCoroutine != null)
        {
            StopCoroutine(_currentCoroutine);
            _currentCoroutine = null;
        }

        switch (state)
        {
            case CheshireCatState.Idle:
                _currentCoroutine = StartCoroutine(IdleCoroutine());
                break;
            case CheshireCatState.Idle_Lick:
                break;
            case CheshireCatState.LyingDown:
                break;
            case CheshireCatState.Walk:
                break;
            case CheshireCatState.FastWalk:
                break;
            case CheshireCatState.Jump:
                break;
            case CheshireCatState.Appearance:
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// �`�F�V���L�̃X�e�[�^�X��ύX����
    /// </summary>
    /// <param name="state"> �X�e�[�^�X�̎�� </param>
    public void ChangeState(CheshireCatState state)
    {
        _currentState = state;

        OnAction(_currentState);
    }
}

/// <summary>
/// �`�F�V���L�̃X�e�[�^�X
/// </summary>
public enum CheshireCatState
{
    /// <summary> �A�C�h��(����) </summary>
    Idle,
    /// <summary> ����Ȃ������r�߂� </summary>
    Idle_Lick,
    /// <summary> �A�C�h��(������) </summary>
    LyingDown,
    /// <summary> ���� </summary>
    Walk,
    /// <summary> ������ </summary>
    FastWalk,
    /// <summary> �W�����v </summary>
    Jump,
    /// <summary> �J�������[�N�t���̓o�ꉉ�o(����g�p�s��) </summary>
    Appearance
}