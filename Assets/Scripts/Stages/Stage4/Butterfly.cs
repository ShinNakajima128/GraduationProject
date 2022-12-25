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
    /// バラの色に応じて蝶のマテリアルを変更する
    /// </summary>
    /// <param name="type"> バラの種類 </param>
    public void ChangeMaterial(RoseType type)
    {
        switch (type)
        {
            case RoseType.Hidden:
                break;
            case RoseType.Red:
                _butterfltRenderer.material = _whiteMat;  //バラの色と逆の色にする
                break;
            case RoseType.White:
                _butterfltRenderer.material = _redMat;
                break;
            default:
                break;
        }
    }
}

public enum ButterfltState
{
    Idle,

}
