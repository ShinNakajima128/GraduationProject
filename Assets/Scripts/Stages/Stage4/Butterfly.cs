using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �X�e�[�W4�̒��̋@�\�̎���Component
/// </summary>
public class Butterfly : MonoBehaviour
{
    #region serialize
    [SerializeField]
    float _moveSpeed = 3.0f;

    [Header("Components")]
    [SerializeField]
    Renderer _butterfltRenderer = default;

    [Header("Materials")]
    [SerializeField]
    Material _redMat = default;

    [SerializeField]
    Material _whiteMat = default;
    #endregion

    #region private
    Animator _anim;
    #endregion

    #region public
    #endregion

    #region property
    #endregion

    private void Awake()
    {
        TryGetComponent(out _anim);
    }

    /// <summary>
    /// ���̃}�e���A��(�F)��ύX����
    /// </summary>
    /// <param name="color"> �F </param>
    public void ChangeMaterial(ButterflyColor color)
    {
        switch (color)
        {
            case ButterflyColor.Red:
                _butterfltRenderer.material = _redMat;
                break;
            case ButterflyColor.White:
                _butterfltRenderer.material = _whiteMat;
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// �w�肵���X�e�[�^�X�ɕύX����
    /// </summary>
    /// <param name="state"> ���̃X�e�[�^�X </param>
    public void ChangeState(ButterflyState state)
    {
        switch (state)
        {
            case ButterflyState.Idle:
                break;
            case ButterflyState.Flapping:
                break;
            case ButterflyState.Fly_Fast:
                break;
            case ButterflyState.Fly_Slow:
                break;
            case ButterflyState.Fly_VerySlow:
                break;
            default:
                break;
        }
        _anim.CrossFadeInFixedTime(state.ToString(), 0.1f);
    }
}

public enum ButterflyState
{
    /// <summary> ���̏�� </summary>
    Idle,
    /// <summary> �~�܂�Ȃ���H�𓮂��� </summary>
    Flapping,
    /// <summary> �H�΂���(����) </summary>
    Fly_Fast,
    /// <summary> �H�΂���(�x��) </summary>
    Fly_Slow,
    /// <summary> �H�΂���(���Ȃ�x��) </summary>
    Fly_VerySlow
}

/// <summary>
/// ���̐F
/// </summary>
public enum ButterflyColor
{
    Red,
    White
}

