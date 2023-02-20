using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// �񕜃A�C�e���̋@�\�����R���|�[�l���g
/// </summary>
public class DrinkMe : MonoBehaviour
{
    #region serialize
    [Tooltip("�񕜗�")]
    [SerializeField]
    int _healValue = 1;

    [Tooltip("���]�ɂ����鎞��")]
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

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (_target == null)
            {
                _target = other.GetComponent<IHealable>();
            }

            //�v���C���[�̗̑͂������Ă���ꍇ�̂ݏ������s��
            if (!_target.IsMaxHP)
            {
                _target.Heal(_healValue);
            }
         
            //�A�C�e���l�����o���s���A���g���A�N�e�B�u�ɂ���
            AudioManager.PlaySE(SEType.Player_Heal);
            EffectManager.PlayEffect(EffectType.Player_Heal, other.gameObject.transform);
            gameObject.SetActive(false);
        }
    }
}
