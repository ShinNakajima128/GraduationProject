using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Glory : MonoBehaviour
{
    #region serialize
    [SerializeField]
    float _gloryRotateTime = 2.0f;
    #endregion

    #region private
    #endregion

    #region public
    #endregion

    #region property
    #endregion

    private void Start()
    {
        transform.DORotate(new Vector3(0, 0, -360), _gloryRotateTime, RotateMode.FastBeyond360)
                 .SetEase(Ease.Linear)
                 .SetLoops(-1);
    }
}
