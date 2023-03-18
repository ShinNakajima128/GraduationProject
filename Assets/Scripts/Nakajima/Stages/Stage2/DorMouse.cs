using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �X�e�[�W2�̃J�b�v�ɉB���l�Y�~�̃R���|�[�l���g
/// </summary>
public class DorMouse : MonoBehaviour
{
    #region serialize
    [Header("variable")]
    [Tooltip("�ڂ̏u���̊Ԋu")]
    [SerializeField]
    float _interval = 1.5f;

    [SerializeField]
    Transform _effectTrans = default;

    [Header("Material")]
    [Tooltip("�l�Y�~�̕\���ύX����p��Material")]
    [SerializeField]
    Material[] _mouseMaterials = default;

    [Header("Debug")]
    [SerializeField]
    bool _debugMode = false;
    #endregion
    #region private
    Animator _anim;
    Renderer _mouseRenderer;
    bool _isAwaking = false;
    Coroutine blinkCoroutine;
    #endregion
    #region property
    #endregion

    private void Awake()
    {
        TryGetComponent(out _anim);
        TryGetComponent(out _mouseRenderer);
    }

    IEnumerator Start()
    {
        if (_debugMode)
        {
            while (true)
            {
                yield return StartCoroutine(TestAnimation());
            }
        }
        OnAnimation(MouseState.OpenEar, 0.05f);
    }

    /// <summary>
    /// �}�E�X�̃A�j���[�V�������Đ�
    /// </summary>
    /// <param name="state"> �Đ�����A�j���[�V���� </param>
    public void OnAnimation(MouseState state, float duration)
    {
        switch (state)
        {
            case MouseState.WakeUp:
                _isAwaking = true;
                blinkCoroutine = StartCoroutine(WakeUpMouseCoroutine());
                break;
            default:
                _isAwaking = false;
                if (blinkCoroutine != null)
                {
                    StopCoroutine(blinkCoroutine);
                    blinkCoroutine = null;
                    _mouseRenderer.material = _mouseMaterials[0];
                }
                break;
        }
        _anim.CrossFadeInFixedTime(state.ToString(), duration);
    }

    /// <summary>
    /// ��������SE���Đ�
    /// </summary>
    void PlayFindSE()
    {
        try
        {
            AudioManager.PlaySE(SEType.Finding);
            EffectManager.PlayEffect(EffectType.DorMouse_Find, _effectTrans.position);
        }
        catch
        {
            Debug.LogError("AudioManager��Hierarchy�ɂȂ����߁A�Đ��ł��܂���");
        }
    }

    IEnumerator WakeUpMouseCoroutine()
    {
        yield return new WaitForSeconds(1f);

        _mouseRenderer.material = _mouseMaterials[1];

        yield return new WaitForSeconds(0.05f);

        _mouseRenderer.material = _mouseMaterials[2];

        //�u���̏����̃��[�v
        while (_isAwaking)
        {
            //�����Ŏw�肵���u���̕b���̊ԁA������ҋ@
            yield return new WaitForSeconds(_interval);

            yield return new WaitForSeconds(0.05f);

            _mouseRenderer.material = _mouseMaterials[1];

            yield return new WaitForSeconds(0.05f);

            _mouseRenderer.material = _mouseMaterials[0];

            yield return new WaitForSeconds(0.05f);

            _mouseRenderer.material = _mouseMaterials[1];

            yield return new WaitForSeconds(0.05f);

            _mouseRenderer.material = _mouseMaterials[2];
        }

        //�ڂ���Ă���e�N�X�`���ɕύX
        _mouseRenderer.material = _mouseMaterials[0];
        Debug.Log("�u���A�j���[�V�����I��");
    }

    IEnumerator TestAnimation()
    {
        yield return new WaitForSeconds(1.0f);

        // OnAnimation(MouseState.WakeUp);

        yield return new WaitForSeconds(8.0f);

        // OnAnimation(MouseState.CloseEar);
    }
}
public enum MouseState
{
    OpenEar,
    CloseEar,
    WakeUp
}

