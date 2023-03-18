using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HedgehogFollow : MonoBehaviour
{
    #region serialize
    [SerializeField]
    Transform _followTrans = default;
    #endregion

    #region private
    #endregion

    #region public
    #endregion

    #region property
    #endregion

    private void FixedUpdate()
    {
        transform.localPosition = new Vector3(0, 0, _followTrans.position.z);
    }
}
