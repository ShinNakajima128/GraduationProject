using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleAreaTrumpSolder : TrumpSolder
{
    #region serialize
    [Tooltip("�z�u���ꂽ�ʒu")]
    [SerializeField]
    DirectionType _dirType = default;
    #endregion
    #region private
    private Animator _anim;
    #endregion
    #region public
    #endregion
    #region property
    public DirectionType DirType { get => _dirType; set => _dirType = value; }
    #endregion

    protected override void Awake()
    {
        base.Awake();
        TryGetComponent(out _anim);
    }

    protected override void Start()
    {
        base.Start();
    }

    /// <summary>
    /// �U��������
    /// </summary>
    /// <param name="waitTime"> �\���Ă���ҋ@���鎞�� </param>
    /// <param name="action"> ����˂��o�����u�Ԃ̃A�N�V���� </param>
    public void OnAttack(float waitTime, Action action = null)
    {
        StartCoroutine(AttackCoroutine(waitTime, action));
    }

    /// <summary>
    /// �ҋ@��Ԃɖ߂�
    /// </summary>
    public void OnReturnToStandby()
    {
        _anim.CrossFadeInFixedTime("Attack_Return", 0.1f);
    }

    /// <summary>
    /// �ł郂�[�V�������Đ�
    /// </summary>
    public void OnAnimation(string name)
    {
        _anim.CrossFadeInFixedTime(name, 0.1f);
    }

    /// <summary>
    /// �g�����v���̍U�������̃R���[�`��
    /// </summary>
    /// <param name="waitTime"> �\���Ă���ҋ@���鎞�� </param>
    /// <param name="action"> ����˂��o�����u�Ԃ̃A�N�V���� </param>
    /// <returns></returns>
    IEnumerator AttackCoroutine(float waitTime, Action action)
    {
        _anim.CrossFadeInFixedTime("Attack_Setup", 0.1f);

        yield return new WaitForSeconds(waitTime);

        _anim.CrossFadeInFixedTime("Attack_Start", 0.1f);

        yield return new WaitForSeconds(0.3f); //�O�ɓ˂������܂ł̃��[�V�������Ԃ�ҋ@

        action?.Invoke();
    }
}
