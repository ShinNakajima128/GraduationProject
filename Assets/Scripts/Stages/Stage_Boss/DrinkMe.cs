using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �񕜃A�C�e���̋@�\�����R���|�[�l���g
/// </summary>
public class DrinkMe : MonoBehaviour
{
    #region serialize
    [Tooltip("�񕜗�")]
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
