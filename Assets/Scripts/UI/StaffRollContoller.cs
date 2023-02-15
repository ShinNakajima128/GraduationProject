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

    [Tooltip("�A�j���[�V��������\���ɂ���X�N���[���̐i�s�x")]
    [SerializeField]
    float _inactiveAnimationPercent = 0.9f;

    [Header("UIObjects")]
    [SerializeField]
    Transform _staffRollTrans = default;

    [SerializeField]
    Transform _scrollTargetTrans = default;

    [SerializeField]
    CreditAnimation _creditAnim = default;
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
                       .OnStart(() => 
                       {
                           _creditAnim.OnFadeDescription(1, 1.0f);
                       })
                       .OnComplete(() => 
                       {
                           print("�X�N���[���I��");
                           _isScrollComplited = true;
                           OnComplitedAction?.Invoke();
                       })
                       .SetDelay(_delayStartTime);

        StartCoroutine(InactiveAnimation());
    }
    IEnumerator InactiveAnimation()
    {
        yield return new WaitForSeconds(_animTime * _inactiveAnimationPercent);

        _creditAnim.OnFadeDescription(0, 1.0f);
    }
}
