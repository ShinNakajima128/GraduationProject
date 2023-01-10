using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// ���ꂫ�̋@�\�����R���|�[�l���g
/// </summary>
public class Debris : MonoBehaviour
{
    #region serialize
    [Tooltip("�����ɂ����鎞��")]
    [SerializeField]
    float _fallTime = 5.0f;

    [Tooltip("�A�N�e�B�u���̍���")]
    [SerializeField]
    float _startHeight = 5.0f;

    [SerializeField]
    Ease _debrisEase = Ease.InCubic;
    #endregion

    #region private
    IDamagable _target;
    GameObject _shadowObject;
    Action _shadowActivateAction;
    bool _init;
    #endregion

    #region public
    #endregion

    #region property
    #endregion


    private void OnDisable()
    {
        if (!_init)
        {
            _init = true;
        }
        else
        {
            EffectManager.PlayEffect(EffectType.Debris_Landing, transform.position);
        }
        transform.localPosition = new Vector3(0, transform.localPosition.y + _startHeight, 0);
    }
    
    /// <summary>
    /// �A�j���[�V�������Đ�����
    /// </summary>
    public void OnAnimation(Action action = null)
    {
        _shadowActivateAction = action;

        transform.DOLocalMoveY(0.5f, _fallTime)
                 .SetEase(_debrisEase)
                 .OnComplete(() => 
                 {
                     //EffectManager.PlayEffect(EffectType.Debris_Landing, transform.position);
                     gameObject.SetActive(false);
                     _shadowActivateAction?.Invoke();
                 })
                 .SetLink(gameObject, LinkBehaviour.KillOnDisable);
    }

    /// <summary>
    /// �e���Z�b�g����
    /// </summary>
    /// <param name="shadow"> �Z�b�g����e�I�u�W�F�N�g </param>
    public void SetShadow(GameObject shadow)
    {
        _shadowObject = shadow;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (_target == null)
            {
                _target = other.GetComponent<IDamagable>();
            }

            if (!_target.IsInvincibled)
            {
                _target.Damage(1);
                gameObject.SetActive(false);
                _shadowObject.SetActive(false);
                _shadowActivateAction?.Invoke();
                //AudioManager.PlaySE(SEType.Player_Damage);
            }
        }
    }
}
