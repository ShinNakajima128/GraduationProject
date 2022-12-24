using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ステージ4の蝶の機能の持つComponent
/// </summary>
public class Butterfly : MonoBehaviour
{
    #region serialize
    [SerializeField]
    float _moveSpeed = 3.0f;

    [Header("Components")]
    [SerializeField]
    Renderer _butterfltRenderer = default;
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
}

public enum ButterfltState
{
    Idle,

}
