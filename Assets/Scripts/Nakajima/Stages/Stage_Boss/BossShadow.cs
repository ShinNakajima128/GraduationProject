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
            Debug.LogError("ボスの影オブジェクトが設定されていません");
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
    /// 影のサイズを変更する
    /// </summary>
    /// <param name="scale"> 変更後の影の大きさ </param>
    /// <param name="time"> 変更にかける時間 </param>
    public void ChangeShadowSize(float scale, float time, Ease ease = default)
    {
        _shadowTrans.DOScale(new Vector3(scale, scale, scale), time).SetEase(ease);
    }
}
