using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage4TrumpSolder : TrumpSolder
{
    #region serialize
    [Header("Variable")]
    [SerializeField]
    Stage4TrumpDirectionType _directionType = default;

    [SerializeField]
    float _maxSpeed = 5.0f;

    [SerializeField]
    float _moveSpeed = 0f;

    [SerializeField]
    bool _isDirectionTrump = false;
    #endregion
    #region private
    Animator _anim;
    #endregion
    #region public
    #endregion
    #region property
    public Stage4TrumpDirectionType CurrentDirType
    {
        get => _directionType;
    }

    public float CurrentMoveSpeed
    {
        get => _moveSpeed;
        set
        {
            _moveSpeed = value;
        }
    }
    #endregion

    protected override void Awake()
    {
        base.Awake();
        TryGetComponent(out _anim);
    }

    protected override void Start()
    {
        base.Start();
        ChangeAnimation(_directionType);
    }

    private void OnEnable()
    {
        ChangeAnimation(_directionType);
    }

    private void OnDisable()
    {
        transform.localPosition = Vector3.zero;
    }

    private void FixedUpdate()
    {
        //�X�e�[�^�X���uWalk�v�̃g�����v���͍��֐i��
        if (!_isDirectionTrump && _directionType == Stage4TrumpDirectionType.Walk)
        {
            transform.localPosition -= new Vector3(_moveSpeed * Time.deltaTime, 0f, 0f);
        }
    }

    public void ChangeAnimation(Stage4TrumpDirectionType type)
    {
        switch (type)
        {
            case Stage4TrumpDirectionType.Standing:
                _anim.CrossFadeInFixedTime("Stage4_Standing", 0.1f);
                break;
            case Stage4TrumpDirectionType.Walk:
                _anim.CrossFadeInFixedTime("Stage4_Walk", 0.1f);
                break;
            case Stage4TrumpDirectionType.Paint:
                _anim.CrossFadeInFixedTime("Stage4_Paint", 0.1f);
                break;
            case Stage4TrumpDirectionType.Loaf:
                _anim.CrossFadeInFixedTime("Stage4_Loaf", 0.1f);
                break;
            case Stage4TrumpDirectionType.Dip:
                _anim.CrossFadeInFixedTime("Stage4_Dip", 0.1f);
                break;
            case Stage4TrumpDirectionType.HideTree:
                _anim.CrossFadeInFixedTime("Stage4_HideTree", 0.1f);
                break;
            default:
                break;
        }
    }
}

/// <summary>
/// Stage4�̃g�����v���̉��o�̎��
/// </summary>
public enum Stage4TrumpDirectionType
{
    /// <summary> ���� </summary>
    Standing,
    /// <summary> ���� </summary>
    Walk,
    /// <summary> �o����h���Ă��� </summary>
    Paint,
    /// <summary> �T�{�� </summary>
    Loaf,
    /// <summary> �y���L�����������Ă��� </summary>
    Dip,
    /// <summary> �؂̗��ɉB��Ă��� </summary>
    HideTree,
    /// <summary> �ς܂ꂽ�o�P�c�̗��ɉB��Ă��� </summary>
    HideBucket
}