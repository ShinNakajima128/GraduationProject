using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// タイトル画面の背景の機能を持つコンポーネント
/// </summary>
public class TitleBackground : MonoBehaviour
{
    #region serialize
    [Header("Variables")]
    [Tooltip("背景のスクロールの速度")]
    [SerializeField]
    float _scrollTime_BG = 10.0f;

    [SerializeField]
    float _scrollTime_CameraRoll = 10.0f;

    [SerializeField]
    float _clockAnimTime = 3.0f;

    [SerializeField]
    float _targetPos_y = 100;

    [SerializeField]
    Ease _clockAnimEase = default;

    [Header("UIObjects")]
    [Header("Background")]
    [SerializeField]
    Transform _scrollBGTrans = default;

    [SerializeField]
    Transform _scrollTargetTrans_BG = default;

    [SerializeField]
    Transform _scrollCameraRollTrans = default;

    [SerializeField]
    Transform _scrollTargetTrans_CameraRoll = default;

    [SerializeField]
    Transform _clockTrans = default;
    #endregion

    #region private
    #endregion

    #region public
    #endregion

    #region property
    #endregion

    private void Start()
    {
        Setup();
    }

    void Setup()
    {
        _scrollBGTrans.DOLocalMove(_scrollTargetTrans_BG.localPosition, _scrollTime_BG)
                      .SetEase(Ease.Linear)
                      .SetLoops(-1, LoopType.Restart);

        _scrollCameraRollTrans.DOLocalMove(_scrollTargetTrans_CameraRoll.localPosition, _scrollTime_CameraRoll)
                              .SetEase(Ease.Linear)
                              .SetLoops(-1, LoopType.Restart);

        _clockTrans.DOLocalMoveY(_clockTrans.localPosition.y + _targetPos_y, _clockAnimTime)
                   .SetEase(_clockAnimEase)
                   .SetLoops(-1, LoopType.Yoyo);
    }
}
