using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagingArea : MonoBehaviour
{
    #region serialize
    #endregion

    #region private
    #endregion

    #region public
    #endregion

    #region property
    #endregion

    private void OnParticleCollision(GameObject other)
    {
        Debug.Log(other.ToString());

        if (other.CompareTag("Player"))
        {
            if (other.TryGetComponent<IDamagable>(out var target))
            {
                target.Damage(1);
            }
            Debug.Log("プレイヤーは衝撃波にヒット");
        }
    }
}
