using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAreaController : MonoBehaviour
{
    #region serialize
    [Tooltip("範囲の位置")]
    [SerializeField]
    DirectionType _directionType = default;

    [Header("Objects")]
    [Tooltip("索敵範囲")]
    [SerializeField]
    BoxCollider _searchArea = default;

    [Tooltip("ダメージを与える範囲")]
    [SerializeField]
    AttackArea _attackArea = default;

    [Header("Debug")]
    [SerializeField]
    TrumpSolderManager _trumpMng = default;
    #endregion

    #region private
    #endregion

    #region public
    #endregion
    
    #region property
    public BoxCollider AttackAreaCollider => _searchArea;
    public AttackArea AttackArea => _attackArea;
    public bool IsAttacked { get; set; } = false;
    #endregion

    private void OnTriggerEnter(Collider other)
    {
        //既に攻撃中、または戦闘中ではない場合は処理を行わない
        if (IsAttacked || !BossStageManager.Instance.IsInBattle)
        {
            return;
        }

        if (other.CompareTag("Player"))
        {
            IsAttacked = true;
            
            _trumpMng.OnTrumpSoldersAttack(_directionType,
            start: () => 
            {
                _attackArea.AttackCollider.enabled = true;
            },
            finish:() => 
            {
                IsAttacked = false;
                _attackArea.AttackCollider.enabled = false;
            });
        }
    }
}
