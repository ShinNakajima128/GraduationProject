using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class BossBattlePlayerController : PlayerBase, IDamagable
{
    #region serialize
    [Header("Variables")]
    [SerializeField]
    int _maxHP = 5;

    [Tooltip("ダメージ時の点滅回数")]
    [SerializeField]
    int _blinksNum = 5;

    [Tooltip("ダメージ時の点滅間隔")]
    [SerializeField]
    float _blinksInterVal = 0.2f;

    [Header("Objects")]
    [Tooltip("プレイヤーのモデルオブジェクト")]
    [SerializeField]
    GameObject _playerModel = default;
    #endregion

    #region private
    ReactiveProperty<int> _currentHP = new ReactiveProperty<int>();
    /// <summary> 無敵状態かどうか </summary>
    bool _isInvincibled = false;
    public bool IsInvincibled => _isInvincibled;

    public void Damage(int value)
    {
        StartCoroutine(DamageCoroutine(value));
    }
    #endregion
    #region public
    #endregion
    #region property
    #endregion

    protected override void Awake()
    {
        base.Awake();
        _currentHP.Value = _maxHP;
    }
    void Start()
    {
        _fc.ChangeFaceType(FaceType.Blink);
    }

    IEnumerator DamageCoroutine(int damageValue)
    {
        _isInvincibled = true;
        _fc.ChangeFaceType(FaceType.Damage);
        _currentHP.Value -= damageValue;

        var wait = new WaitForSeconds(_blinksInterVal);

        for (int i = 0; i < _blinksNum; i++)
        {
            _playerModel.SetActive(!_playerModel.activeSelf);
            yield return wait;
        }

        _isInvincibled = false;
        _playerModel.SetActive(true);
        _fc.ChangeFaceType(FaceType.Blink);
    }
}
