using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class BossAreaController : MonoBehaviour
{
    #region serialize
    [Header("Objects")]
    [Tooltip("�g�����v���̍U���͈�")]
    [SerializeField]
    AttackArea[] _attackAreas = default;

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
        
    }
}
