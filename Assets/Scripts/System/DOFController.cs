using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using DG.Tweening;

public class DOFController : MonoBehaviour
{
    #region serialize
    [Tooltip("ぼかし時のEndの数値")]
    [SerializeField]
    float _targetValue = default;

    [Header("Components")]
    [Tooltip("現在のSceneのカメラに付いているVolume")]
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
    /// DOFのON/OFFを切り替える
    /// </summary>
    /// <param name="isActive"> アクティブにするかどうか </param>
    /// <param name="time"> 切り替わる時間 </param>
    public void ActivateDOF(bool isActive, float time = 0.5f)
    {
        //DOFアクティブ時
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
