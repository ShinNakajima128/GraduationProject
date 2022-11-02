using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// テーブルから飛び散る紙のエフェクトのコンポーネント
/// </summary>
public class Scatter : MonoBehaviour
{
    #region serialize
    #endregion
    #region private
    IDamagable _target;
    #endregion
    #region property
    #endregion

    private void OnParticleCollision(GameObject other)
    {
        if (other.tag == "Player")
        {
            if (_target == null)
            {
                _target = other.GetComponent<IDamagable>();
            }

            if (!_target.IsInvincibled)
            {
                _target.Damage(1);
                AudioManager.PlaySE(SEType.Player_Damage);
            }
        }
    }
}
