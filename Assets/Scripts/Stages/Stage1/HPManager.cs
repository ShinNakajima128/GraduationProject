using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UniRx;

/// <summary>
/// HPを管理するマネージャークラス
/// </summary>
public class HPManager : MonoBehaviour
{
    #region serialize
    [Header("Variables")]
    [Tooltip("HPゲージの現在ステータス(確認用)")]
    [SerializeField]
    HPGaugeState _currentGaugeState = HPGaugeState.Three;

    [Header("UI")]
    [Tooltip("HPオブジェクトの親オブジェクト")]
    [SerializeField]
    Transform _hpGaugeUITrans = default;

    [Header("Components")]
    [Tooltip("アリスのHP")]
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
    /// <summary> 現在のHP </summary>
    public ReactiveProperty<int> CurrentHP => _currentHP;
    public bool IsLosted => _isLosted;
    #endregion

    private void Awake()
    {
        Instance = this;

        //HPの値に変更があった場合は登録した処理を実行
        _currentHP.Subscribe(value =>
        {
            if (_init)
            {
                //HPのUIを変更
                SetHP((HPGaugeState)value);

                //HPが0になった場合はゲームオーバー演出を実行
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
    /// HPの値を変更する
    /// </summary>
    /// <param name="value"> HPに与える量 </param>
    /// <param name="isHeal"> 回復の処理かどうか </param>
    public void ChangeHPValue(int value, bool isHeal = false)
    {
        //回復ではない場合はHPゲージを揺らす演出
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
            Debug.Log("HP回復");
        }
    }

    /// <summary>
    /// 体力を全快させる
    /// </summary>
    public void RecoveryHP()
    {
        _currentHP.Value = 3;
    }

    /// <summary>
    /// HPをセットする
    /// </summary>
    /// <param name="state"> HPゲージの状態 </param>
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
/// HPゲージのステータス
/// </summary>
public enum HPGaugeState
{
    Zero,
    One,
    Two,
    Three
}
