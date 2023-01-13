using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 回復アイテムの機能を持つコンポーネント
/// </summary>
public class DrinkMe : MonoBehaviour
{
    #region serialize
    [Tooltip("回復量")]
    [SerializeField]
    int _healValue = 1;
    #endregion

    #region private
    IHealable _target;
    #endregion

    #region public
    #endregion

    #region property
    #endregion

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (_target == null)
            {
                _target = other.GetComponent<IHealable>();
            }

            //プレイヤーの体力がMAXの時は処理を行わない
            if (_target.IsMaxHP)
            {
                return;
            }

            //プレイヤーを回復し、自身を非アクティブにする
            _target.Heal(_healValue);
            gameObject.SetActive(false);
        }
    }
}
