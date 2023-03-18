using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SubmitIcon : MonoBehaviour
{
    #region serialize
    [Tooltip("移動の幅")]
    [SerializeField]
    float _moveRange = 0.5f;
    
    [Tooltip("アニメーションの時間")]
    [SerializeField]
    float _animTime = 0.5f;
    #endregion
    #region private
    Vector3 _originPos = default;
    #endregion

    #region public
    #endregion
    
    #region property
    #endregion

    private void Start()
    {
        _originPos = transform.localPosition;
        OnAnimation();
    }

    /// <summary>
    /// アイコンのアニメーションを再生
    /// </summary>
    void OnAnimation()
    {
        transform.localPosition = _originPos;
        transform.DOLocalMoveY(transform.localPosition.y + _moveRange, _animTime)
                 .SetLoops(-1, LoopType.Yoyo);
    }
}
