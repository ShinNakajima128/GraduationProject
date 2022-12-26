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
    /// 蝶のマテリアル(色)を変更する
    /// </summary>
    /// <param name="color"> 色 </param>
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
    /// 指定したステータスに変更する
    /// </summary>
    /// <param name="state"> 蝶のステータス </param>
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
    /// <summary> 蝶の状態 </summary>
    Idle,
    /// <summary> 止まりながら羽を動かす </summary>
    Flapping,
    /// <summary> 羽ばたき(速め) </summary>
    Fly_Fast,
    /// <summary> 羽ばたき(遅め) </summary>
    Fly_Slow,
    /// <summary> 羽ばたき(かなり遅め) </summary>
    Fly_VerySlow
}

/// <summary>
/// 蝶の色
/// </summary>
public enum ButterflyColor
{
    Red,
    White
}

