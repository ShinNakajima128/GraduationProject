using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

/// <summary>
/// �S�̂�UI��\��/��\����؂�ւ���@�\�����R���g���[���[
/// </summary>
public class HiddenUIController : MonoBehaviour
{
    #region serialize
    [SerializeField]
    CanvasGroup _hiddenTargetGroup = default;
    #endregion

    #region private
    bool _isOnHidden = false;
    #endregion

    #region public
    #endregion

    #region property
    #endregion

    private void Start()
    {
        this.UpdateAsObservable()
            .Where(_ => UIInput.Hidden)
            .ThrottleFirst(TimeSpan.FromMilliseconds(200))
            .Subscribe(_ =>
            {
                ActivateUI();
            });
    }

    /// <summary>
    /// 
    /// </summary>
    void ActivateUI()
    {
        _isOnHidden = !_isOnHidden;

        _hiddenTargetGroup.alpha = _isOnHidden ? 0 : 1;
    }
}
