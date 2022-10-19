using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// ステージ1の背景の動きを管理するクラス
/// </summary>
public class BackgroundController : MonoBehaviour
{
    #region serialize
    [Tooltip("落下速度")]
    [SerializeField]
    float _fallSpeed = 5.0f;

    
    [Tooltip("落下表現用の背景モデル")]
    [SerializeField]
    Transform _backgroundModel = default;
    
    [Tooltip("位置リセット用のObject")]
    [SerializeField]
    Transform _resetPositionPoint = default;

    #endregion

    #region private
    Vector3 _originPos;
    #endregion

    private void Awake()
    {
        _originPos = _backgroundModel.position;
    }
    private void Start()
    {
        _backgroundModel.DOMoveY(_resetPositionPoint.position.y, _fallSpeed)
                        .SetEase(Ease.Linear)
                        .OnComplete(() =>
                        {
                            _backgroundModel.position = _originPos;
                        })
                        .SetLoops(-1);
    }
}
