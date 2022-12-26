using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;

/// <summary>
/// �X�e�[�W4�̒��̋@�\�̎���Component
/// </summary>
public class Butterfly : MonoBehaviour
{
    #region serialize
    [SerializeField]
    float _moveSpeed = 3.0f;

    [SerializeField]
    float _flyAnimTime = 2.5f;

    [Header("Components")]
    [SerializeField]
    Renderer _butterfltRenderer = default;

    [Tooltip("��ы��鎞��Path")]
    [SerializeField]
    Transform[] _flyPath = default;

    [Header("Materials")]
    [SerializeField]
    Material _redMat = default;

    [SerializeField]
    Material _whiteMat = default;
    #endregion

    #region private
    Animator _anim;
    BoxCollider _playerSearchCollider;
    Vector3 _originEulerAngles;
    bool _init = false;
    #endregion

    #region public
    #endregion

    #region property
    #endregion

    private void Awake()
    {
        TryGetComponent(out _anim);
        TryGetComponent(out _playerSearchCollider);
        _originEulerAngles = transform.localEulerAngles;
        _init = true;
    }

    private void OnDisable()
    {
        _playerSearchCollider.enabled = false;

        if (_init)
        {
            transform.localEulerAngles = _originEulerAngles;
        }
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
                _playerSearchCollider.enabled = true;
                break;
            case ButterflyState.Flapping:
                break;
            case ButterflyState.Fly_Fast:
                _playerSearchCollider.enabled = false;
                StartCoroutine(FlyingCoroutine());
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("�A���X�Ƀq�b�g");

            int random = UnityEngine.Random.Range(0, 2);

            if (random == 0)
            {
                ChangeState(ButterflyState.Flapping);
            }
            else
            {
                ChangeState(ButterflyState.Fly_Fast);
            }
        }
    }

    /// <summary>
    /// ��ԋ��郂�[�V�����̃R���[�`��
    /// </summary>
    IEnumerator FlyingCoroutine()
    {
        var path = _flyPath.Select(x => x.transform.position).ToArray();

        transform.localRotation = new Quaternion(transform.localEulerAngles.x, transform.localEulerAngles.y, -90, 0);

        yield return transform.DOPath(path, _flyAnimTime, PathType.CatmullRom)
                              .WaitForCompletion();

        gameObject.SetActive(false);
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

