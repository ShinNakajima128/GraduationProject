using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class BossAreaController : MonoBehaviour
{
    #region serialize
    [Header("Objects")]
    [Tooltip("ƒgƒ‰ƒ“ƒv•º‚ÌUŒ‚”ÍˆÍ")]
    [SerializeField]
    AttackAreaController[] _attackAreas = default;

    [SerializeField]
    TrumpSolderManager _trumpSolderMng = default;
    #endregion
    #region private
    #endregion
    #region public
    #endregion
    #region property
    #endregion
    void Start()
    {
        BossStageManager.Instance.OnInGame += AreaActivate;
    }

    void AreaActivate(bool isActive)
    {
        foreach (var a in _attackAreas)
        {
            a.AttackAreaCollider.enabled = isActive;
        }
    }
}
