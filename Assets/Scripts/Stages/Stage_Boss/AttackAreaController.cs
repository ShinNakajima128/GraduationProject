using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAreaController : MonoBehaviour
{
    #region serialize
    [Tooltip("”ÍˆÍ‚ÌˆÊ’u")]
    [SerializeField]
    DirectionType _directionType = default;

    [Tooltip("UŒ‚”ÍˆÍ")]
    [SerializeField]
    BoxCollider _attackArea = default;

    [Header("Debug")]
    [SerializeField]
    TrumpSolderManager _trumpMng = default;
    #endregion

    #region private
    #endregion

    #region public
    #endregion
    
    #region property
    public BoxCollider AttackAreaCollider => _attackArea;
    public bool IsAttacked { get; set; } = false;
    #endregion

    private void OnTriggerEnter(Collider other)
    {
        //Šù‚ÉUŒ‚’†‚Ìê‡‚Íˆ—‚ðs‚í‚È‚¢
        if (IsAttacked)
        {
            return;
        }

        if (other.CompareTag("Player"))
        {
            IsAttacked = true;
            _trumpMng.OnTrumpSoldersAttack(_directionType, () => IsAttacked = false);
        }
    }
}
