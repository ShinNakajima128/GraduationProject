using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 汎用的な攻撃判定を持つColliderのコンポーネント
/// </summary>
public class AttackCollider : MonoBehaviour
{
    #region serialize
    #endregion

    #region private
    int _attackValue = 1;
    IDamagable _target;
    #endregion

    #region public
    #endregion

    #region property
    #endregion

    public void SetDamage(int value)
    {
        _attackValue = value;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (_target == null)
            {
                other.TryGetComponent(out _target);
            }

            _target.Damage(_attackValue);
        }
    }
}
