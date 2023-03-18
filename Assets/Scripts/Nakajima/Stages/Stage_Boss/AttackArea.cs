using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackArea : MonoBehaviour
{
    #region serialize
    #endregion
    #region private
    BoxCollider _attackCollider;
    #endregion
    #region public
    #endregion
    #region property
    public BoxCollider AttackCollider => _attackCollider;
    #endregion

    private void Awake()
    {
        TryGetComponent(out _attackCollider);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!BossStageManager.Instance.IsInBattle)
        {
            return;
        }

        //プレイヤーかボス
        if (other.CompareTag("Player") || other.CompareTag("Boss"))
        {
            if (other.TryGetComponent<IDamagable>(out var target))
            {
                if (!target.IsInvincibled)
                {
                    target.Damage(1);
                }
            }
        }
    }
}
