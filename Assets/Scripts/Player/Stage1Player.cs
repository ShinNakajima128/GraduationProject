using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

public class Stage1Player : PlayerBase, IEffectable, IDamagable
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
    IEnumerator DamageCoroutine()
    {
        _isInvincibled = true;
        _fc.ChangeFaceType(FaceType.Damage);

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
