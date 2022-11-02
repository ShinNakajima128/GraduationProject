using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �e�[�u�������юU�鎆�̃G�t�F�N�g�̃R���|�[�l���g
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
