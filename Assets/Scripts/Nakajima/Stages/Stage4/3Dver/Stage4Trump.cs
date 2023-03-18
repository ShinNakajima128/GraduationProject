using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage4Trump : TrumpSolder
{
    #region serialize
    [SerializeField]
    Stage4TrumpAnimType _trumpType = default;
    #endregion

    #region private
    private Animator _anim;
    #endregion

    #region public
    #endregion

    #region property
    #endregion

    protected override void Awake()
    {
        base.Awake();
        TryGetComponent(out _anim);

    }
    protected override void Start()
    {
        base.Start();
        OnAnimation(_trumpType);
    }

    /// <summary>
    /// 指定したアニメーションを再生
    /// </summary>
    /// <param name="type"> アニメーションの種類 </param>
    void OnAnimation(Stage4TrumpAnimType type)
    {
        switch (type)
        {
            case Stage4TrumpAnimType.Walk:
                _anim.CrossFadeInFixedTime("Stage4_Walk", 0.1f);
                break;
            case Stage4TrumpAnimType.Standing:
                _anim.CrossFadeInFixedTime("Stage4_Standing", 0.1f);
                break;
            case Stage4TrumpAnimType.Paint:
                _anim.CrossFadeInFixedTime("Stage4_Paint", 0.1f);
                break;
            case Stage4TrumpAnimType.Loaf:
                _anim.CrossFadeInFixedTime("Stage4_Loaf", 0.1f);
                break;
            case Stage4TrumpAnimType.Dip:
                _anim.CrossFadeInFixedTime("Stage4_Dip", 0.1f);
                break;
            default:
                break;
        }
    }
}
public enum Stage4TrumpAnimType
{
    Walk,
    Standing,
    Paint,
    Loaf,
    Dip
}
