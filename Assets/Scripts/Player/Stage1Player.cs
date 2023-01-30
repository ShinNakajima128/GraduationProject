using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

public class Stage1Player : PlayerBase, IEffectable, IDamagable, IHealable
{
    #region serialize
    [Tooltip("エフェクトを再生する位置")]
    [SerializeField]
    Transform _effectPos = default;

    [Tooltip("プレイヤーのモデルオブジェクト")]
    [SerializeField]
    GameObject _playerModel = default;

    [Tooltip("ダメージ時の点滅回数")]
    [SerializeField]
    int _blinksNum = 5;

    [Tooltip("ダメージ時の点滅間隔")]
    [SerializeField]
    float _blinksInterVal = 0.2f;
    #endregion

    #region private
    /// <summary> 無敵状態かどうか </summary>
    bool _isInvincibled = false;
    #endregion

    #region property
    public Transform EffectPos => _effectPos;

    public bool IsInvincibled => _isInvincibled;
    public bool IsMaxHP { get; private set; } = true;

    void Start()
    {
        _fc.ChangeFaceType(FaceType.Blink);
    }
    #endregion
    public void Damage(int value)
    {
        StartCoroutine(DamageCoroutine());
        FallGameManager.Instance.OnDamage(value);
    }

    /// <summary>
    /// 体力を回復する
    /// </summary>
    /// <param name="value"> 回復量 </param>
    public void Heal(int value)
    {
        HPManager.Instance.ChangeHPValue(value, true);

        if (HPManager.Instance.IsMaxHP)
        {
            IsMaxHP = true;
        }
    }

    IEnumerator DamageCoroutine()
    {
        _isInvincibled = true;
        _fc.ChangeFaceType(FaceType.Damage);
        IsMaxHP = false;

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

    #region animation event method
    public void OnSmileFace()
    {
        _fc.ChangeFaceType(FaceType.Smile);
    }
    public void OnRotationFace()
    {
        _fc.ChangeFaceType(FaceType.Rotation);
    }
    #endregion
}
