using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// アリスのモーションを管理するController
/// </summary>
public class AliceMotionController : MonoBehaviour
{
    #region serialize
    [SerializeField]
    AliceDirectionAnimType _directionType = default;
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

    private void Start()
    {
        ChangeAnimation(_directionType);
    }

    public void ChangeAnimation(AliceDirectionAnimType type)
    {
        _anim.CrossFadeInFixedTime(type.ToString(), 0.2f);
    }
}
