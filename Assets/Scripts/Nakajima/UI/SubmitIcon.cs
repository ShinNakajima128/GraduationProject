using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SubmitIcon : MonoBehaviour
{
    #region serialize
    [Tooltip("�ړ��̕�")]
    [SerializeField]
    float _moveRange = 0.5f;
    
    [Tooltip("�A�j���[�V�����̎���")]
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
    /// �A�C�R���̃A�j���[�V�������Đ�
    /// </summary>
    void OnAnimation()
    {
        transform.localPosition = _originPos;
        transform.DOLocalMoveY(transform.localPosition.y + _moveRange, _animTime)
                 .SetLoops(-1, LoopType.Yoyo);
    }
}
