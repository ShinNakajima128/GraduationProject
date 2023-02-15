using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using DG.Tweening;

public class DOFController : MonoBehaviour
{
    #region serialize
    [Tooltip("�ڂ�������End�̐��l")]
    [SerializeField]
    float _targetValue = default;

    [Header("Components")]
    [Tooltip("���݂�Scene�̃J�����ɕt���Ă���Volume")]
    [SerializeField]
    Volume _volume = default;
    #endregion

    #region private
    DepthOfField _dof;
    #endregion

    #region public
    #endregion

    #region property
    public static DOFController Instance { get; private set; }
    #endregion

    private void Awake()
    {
        Instance = this;
        _volume.profile.TryGet(out _dof);
    }

    /// <summary>
    /// DOF��ON/OFF��؂�ւ���
    /// </summary>
    /// <param name="isActive"> �A�N�e�B�u�ɂ��邩�ǂ��� </param>
    /// <param name="time"> �؂�ւ�鎞�� </param>
    public void ActivateDOF(bool isActive, float time = 0.5f)
    {
        //DOF�A�N�e�B�u��
        if (isActive) 
        {
            DOTween.To(() => _dof.focusDistance.value,
                x => _dof.focusDistance.value = x,
                _targetValue,
                time);
        }
        else
        {
            DOTween.To(() => _dof.focusDistance.value,
                x => _dof.focusDistance.value = x,
                5,
                time);
        }
        
    }
}
