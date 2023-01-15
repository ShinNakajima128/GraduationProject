using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UniRx;

/// <summary>
/// HP���Ǘ�����}�l�[�W���[�N���X
/// </summary>
public class HPManager : MonoBehaviour
{
    #region serialize
    [Header("Variables")]
    [Tooltip("HP�Q�[�W�̌��݃X�e�[�^�X(�m�F�p)")]
    [SerializeField]
    HPGaugeState _currentGaugeState = HPGaugeState.Three;

    [Header("UI")]
    [Tooltip("HP�I�u�W�F�N�g�̐e�I�u�W�F�N�g")]
    [SerializeField]
    Transform _hpGaugeUITrans = default;

    [Header("Components")]
    [Tooltip("�A���X��HP")]
    [SerializeField]
    AliceHP[] _aliceHp = default;
    #endregion

    #region private
    ReactiveProperty<int> _currentHP = new ReactiveProperty<int>();
    bool _init = false;
    bool _isLosted = false;
    #endregion

    #region public
    public event Action DamageAction;
    public event Action LostHpAction;
    public bool IsMaxHP => _currentHP.Value == 3;
    #endregion

    #region property
    public static HPManager Instance { get; private set; }
    /// <summary> ���݂�HP </summary>
    public ReactiveProperty<int> CurrentHP => _currentHP;
    public bool IsLosted => _isLosted;
    #endregion

    private void Awake()
    {
        Instance = this;

        //HP�̒l�ɕύX���������ꍇ�͓o�^�������������s
        _currentHP.Subscribe(value =>
        {
            if (_init)
            {
                //HP��UI��ύX
                SetHP((HPGaugeState)value);

                //HP��0�ɂȂ����ꍇ�̓Q�[���I�[�o�[���o�����s
                if (value <= 0 && !_isLosted)
                {
                    LostHpAction?.Invoke();
                    _isLosted = true;
                }
            }
        });
        _init = true;
    }

    /// <summary>
    /// HP�̒l��ύX����
    /// </summary>
    /// <param name="value"> HP�ɗ^����� </param>
    /// <param name="isHeal"> �񕜂̏������ǂ��� </param>
    public void ChangeHPValue(int value, bool isHeal = false)
    {
        //�񕜂ł͂Ȃ��ꍇ��HP�Q�[�W��h�炷���o
        if (!isHeal)
        {
            _currentHP.Value -= value;
            _hpGaugeUITrans.DOShakePosition(0.25f, 20, 30, fadeOut: false);

            if (_currentHP.Value > 0)
            {
                DamageAction?.Invoke();
            }
        }
        else
        {
            if (_currentHP.Value >= 3)
            {
                return;
            }
            _currentHP.Value += value;
            Debug.Log("HP��");
        }
    }

    /// <summary>
    /// �̗͂�S��������
    /// </summary>
    public void RecoveryHP()
    {
        _currentHP.Value = 3;
    }

    /// <summary>
    /// HP���Z�b�g����
    /// </summary>
    /// <param name="state"> HP�Q�[�W�̏�� </param>
    void SetHP(HPGaugeState state)
    {
        _currentGaugeState = state;

        switch (_currentGaugeState)
        {
            case HPGaugeState.Zero:
                _aliceHp[0].ChangeState(HPState.OFF);
                _aliceHp[1].ChangeState(HPState.OFF);
                _aliceHp[2].ChangeState(HPState.OFF);
                break;
            case HPGaugeState.One:
                _aliceHp[0].ChangeState(HPState.ON);
                _aliceHp[1].ChangeState(HPState.OFF);
                _aliceHp[2].ChangeState(HPState.OFF);
                break;
            case HPGaugeState.Two:
                _aliceHp[0].ChangeState(HPState.ON);
                _aliceHp[1].ChangeState(HPState.ON);
                _aliceHp[2].ChangeState(HPState.OFF);
                break;
            case HPGaugeState.Three:
                _aliceHp[0].ChangeState(HPState.ON);
                _aliceHp[1].ChangeState(HPState.ON);
                _aliceHp[2].ChangeState(HPState.ON);
                break;
            default:
                break;
        }
    }
}

/// <summary>
/// HP�Q�[�W�̃X�e�[�^�X
/// </summary>
public enum HPGaugeState
{
    Zero,
    One,
    Two,
    Three
}
