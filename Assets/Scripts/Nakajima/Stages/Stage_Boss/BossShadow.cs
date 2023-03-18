using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BossShadow : MonoBehaviour
{
    #region serialize
    [SerializeField]
    Transform _shadowTrans = default;
    #endregion

    #region private
    #endregion

    #region public
    #endregion

    #region property
    #endregion

    private void Start()
    {
        if (_shadowTrans == null)
        {
            Debug.LogError("�{�X�̉e�I�u�W�F�N�g���ݒ肳��Ă��܂���");
        }
    }

    private void LateUpdate()
    {
        if (_shadowTrans == null)
        {
            return;
        }
        _shadowTrans.position = new Vector3(transform.position.x, 0.01f, transform.position.z);
    }

    /// <summary>
    /// �e�̃T�C�Y��ύX����
    /// </summary>
    /// <param name="scale"> �ύX��̉e�̑傫�� </param>
    /// <param name="time"> �ύX�ɂ����鎞�� </param>
    public void ChangeShadowSize(float scale, float time, Ease ease = default)
    {
        _shadowTrans.DOScale(new Vector3(scale, scale, scale), time).SetEase(ease);
    }
}
