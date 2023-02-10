using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class StaffRollContoller : MonoBehaviour
{
    #region serialize
    [Header("Variables")]
    [Tooltip("�S�̂�����I���܂ł̎���")]
    [SerializeField]
    float _animTime = 30f;

    [Tooltip("�X�^�b�t���[��������n�߂�܂ł̎���")]
    [SerializeField]
    float _delayStartTime = 2.0f;

    [Header("UIObjects")]
    [SerializeField]
    Transform _staffRollTrans = default;

    [SerializeField]
    Transform _scrollTargetTrans = default;
    #endregion

    #region private
    bool _isScrollComplited = false;
    #endregion

    #region public
    public event Action OnComplitedAction;
    #endregion

    #region property
    public bool IsScrollComplited => _isScrollComplited;
    #endregion

    private void Start()
    {
        _staffRollTrans.DOLocalMove(_scrollTargetTrans.localPosition, _animTime)
                       .SetEase(Ease.Linear)
                       .OnComplete(() => 
                       {
                           print("�X�N���[���I��");
                           _isScrollComplited = true;
                           OnComplitedAction?.Invoke();
                       })
                       .SetDelay(_delayStartTime);
    }
}
