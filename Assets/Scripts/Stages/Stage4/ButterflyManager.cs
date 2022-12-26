using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stage4の蝶を管理するマネージャークラス
/// </summary>
public class ButterflyManager : MonoBehaviour
{
    #region serialize
    [Tooltip("蝶の生成数")]
    [SerializeField]
    int _butterflyGenerateNum = 10;

    [Tooltip("蝶の親オブジェクトのTransform")]
    [SerializeField]
    Transform _butterflyParent = default;

    [Header("Components")]
    [SerializeField]
    Butterfly _butterflPrefab = default;
    #endregion

    #region private
    /// <summary> 蝶のComponentList </summary>
    List<Butterfly> _butterflyList = new List<Butterfly>();
    #endregion

    #region public
    #endregion

    #region property
    #endregion

    private void Awake()
    {
        for (int i = 0; i < _butterflyGenerateNum; i++)
        {
            var butterfly = Instantiate(_butterflPrefab, _butterflyParent);
            butterfly.transform.localPosition = Vector3.zero;
            _butterflyList.Add(butterfly);
            butterfly.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 蝶をアクティブにする
    /// </summary>
    /// <param name="state"> 蝶のステータス </param>
    /// <param name="color"> 蝶の色 </param>
    /// <param name="target"> アクティブにする位置 </param>
    public void ActiveFlyingButterfly(ButterflyState state, ButterflyColor color, Transform target)
    {
        foreach (var b in _butterflyList)
        {
            b.gameObject.SetActive(true);
            b.transform.localPosition = target.position;
            b.ChangeMaterial(color);
            b.ChangeState(state);
        }
    }
}
