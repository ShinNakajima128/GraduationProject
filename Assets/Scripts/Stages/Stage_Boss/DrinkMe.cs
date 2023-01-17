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
    bool _init = false;
    #endregion

    #region public
    #endregion

    #region property
    #endregion

    private void OnEnable()
    {
        if (_init)
        {
            OnRotate();
        }
    }

    private void OnDisable()
    {
        transform.DOLocalRotate(new Vector3(15, 0, 0), 0f);
    }

    private void Start()
    {
        if (!_init)
        {
            OnRotate();
            _init = true;
        }
    }
    void OnRotate()
    {
        transform.DOLocalRotate(new Vector3(15, -360, 0), _rotateTime, RotateMode.FastBeyond360)
                 .SetEase(Ease.Linear)
                 .SetLoops(-1)
                 .SetLink(gameObject, LinkBehaviour.KillOnDisable);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (_target == null)
            {
                _target = other.GetComponent<IHealable>();
            }

            //�v���C���[�̗̑͂�MAX�̎��͏������s��Ȃ�
            if (_target.IsMaxHP)
            {
                return;
            }

            //�v���C���[���񕜂��A���g���A�N�e�B�u�ɂ���
            _target.Heal(_healValue);
            gameObject.SetActive(false);
        }
    }
}
