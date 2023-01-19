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
    float _fadeTime = 1.0f;

    [SerializeField]
    CheshireCatState _currentState = default;

    [Header("Renderer")]
    [SerializeField]
    Renderer _cheshireRenderer = default;
    #endregion

    #region private
    Animator _anim;
    Coroutine _currentCoroutine;
    Material _cheshireBodyMat;
    Material _cheshireFaceMat;
    #endregion

    #region public
    #endregion

    #region property
    #endregion

    private void Awake()
    {
        TryGetComponent(out _anim);
    }

    IEnumerator Start()
    {
        EventManager.ListenEvents(Events.Cheshire_StartGrinning, () => 
        {
            OnAction(CheshireCatState.Jump, 0.2f); 
        });
        _cheshireBodyMat = _cheshireRenderer.materials[0];
        _cheshireFaceMat = _cheshireRenderer.materials[1];

        _cheshireRenderer.materials[0] = _cheshireBodyMat;
        _cheshireRenderer.materials[1] = _cheshireFaceMat;

        yield return null;

        ChangeState(_currentState);
    }

    /// <summary>
    /// �`�F�V���L�̃X�e�[�^�X��ύX����
    /// </summary>
    /// <param name="state"> �X�e�[�^�X�̎�� </param>
    public void ChangeState(CheshireCatState state, float animBlend = 0.2f)
    {
        _currentState = state;

        OnAction(_currentState, animBlend);
    }

    public void ActivateDissolve(bool isActive)
    {
        Debug.Log("Dissolve");
        float bodyValue;
        float faceValue;

        if (isActive)
        {
            bodyValue = -1f;

            DOTween.To(() => bodyValue,
                v => bodyValue = v,
                0f,
                _fadeTime)
                .OnUpdate(() => 
                { 
                    _cheshireBodyMat.SetVector("_DissolveParams", new Vector4(3, 1, bodyValue, 0.1f));
                });

            faceValue = -1;

            DOTween.To(() => faceValue,
                v => faceValue = v,
                0f,
                _fadeTime)
                .OnUpdate(() =>
                {
                    _cheshireFaceMat.SetVector("_DissolveParams", new Vector4(3, 1, faceValue, 0.1f));
                });
        }
        else
        {
            bodyValue = 0f;

            DOTween.To(() => bodyValue,
                v => bodyValue = v,
                -1f,
                _fadeTime)
                .OnUpdate(() =>
                {
                    _cheshireBodyMat.SetVector("_DissolveParams", new Vector4(3, 1, bodyValue, 0.1f));
                });

            faceValue = 0f;

            DOTween.To(() => faceValue,
                v => faceValue = v,
                -1f,
                _fadeTime)
                .OnUpdate(() =>
                {
                    _cheshireFaceMat.SetVector("_DissolveParams", new Vector4(3, 1, faceValue, 0.1f));
                });
        }
    }

    /// <summary>
    /// �w�肵���X�e�[�^�X�̃A�N�V���������s
    /// </summary>
    /// <param name="state"> �X�e�[�^�X�̎�� </param>
    void OnAction(CheshireCatState state, float animBlend)
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
                _anim.CrossFadeInFixedTime(CheshireCatState.Idle.ToString(), 0.25f);
                _currentCoroutine = StartCoroutine(IdleCoroutine());
                break;
            case CheshireCatState.Idle_Lick:
                break;
            case CheshireCatState.LyingDown:
                _anim.CrossFadeInFixedTime(CheshireCatState.LyingDown.ToString(), animBlend);
                break;
            case CheshireCatState.Walk:
                _anim.CrossFadeInFixedTime(CheshireCatState.Walk.ToString(), animBlend);
                break;
            case CheshireCatState.FastWalk:
                _anim.CrossFadeInFixedTime(CheshireCatState.FastWalk.ToString(), animBlend);
                break;
            case CheshireCatState.Jump:
                _anim.CrossFadeInFixedTime(CheshireCatState.Jump.ToString(), animBlend);
                break;
            case CheshireCatState.Appearance:
                break;
            case CheshireCatState.Idle_Standing:
                _anim.CrossFadeInFixedTime(CheshireCatState.Idle_Standing.ToString(), animBlend);
                break;
            default:
                break;
        }
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
            random = Random.Range(7, 10);

            yield return new WaitForSeconds(random);

            _anim.CrossFadeInFixedTime(CheshireCatState.Idle_Lick.ToString(), 0.1f);

            yield return new WaitForSeconds(2.3f);

            _anim.CrossFadeInFixedTime(CheshireCatState.Idle.ToString(), 0.25f);
        }
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
    /// <summary> �J�������[�N�t���̓o�ꉉ�o </summary>
    Appearance,
    /// <summary> �A�C�h��(����) </summary>
    Idle_Standing
}
