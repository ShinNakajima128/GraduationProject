using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// 回復アイテムの機能を持つコンポーネント
/// </summary>
public class DrinkMe : MonoBehaviour
{
    #region serialize
    [Tooltip("回復量")]
    [SerializeField]
    int _healValue = 1;

    [Tooltip("一回転にかける時間")]
    [SerializeField]
    float _rotateTime = 2.0f;
    #endregion

    #region private
    IHealable _target;
    #endregion

    #region protected
    protected bool _init = false;
    #endregion

    #region public
    #endregion

    #region property
    #endregion

    protected virtual void OnEnable()
    {
        if (_init)
        {
            OnRotate();
        }
    }

    protected virtual void OnDisable()
    {
        transform.DOLocalRotate(new Vector3(15, 0, 0), 0f);
    }

    protected virtual void Start()
    {
        if (!_init)
        {
            OnRotate();
            _init = true;
        }
    }
    protected void OnRotate()
    {
        transform.DOLocalRotate(new Vector3(15, -360, 0), _rotateTime, RotateMode.FastBeyond360)
                 .SetEase(Ease.Linear)
                 .SetLoops(-1)
                 .SetLink(gameObject, LinkBehaviour.KillOnDisable);
    }

    protected void OnTriggerEnter(Collider other)
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
            AudioManager.PlaySE(SEType.Player_Heal);
            EffectManager.PlayEffect(EffectType.Player_Heal, other.gameObject.transform);
            _target.Heal(_healValue);
            gameObject.SetActive(false);
        }
    }
}
