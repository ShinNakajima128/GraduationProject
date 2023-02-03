using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class BossBattlePlayerController : PlayerBase, IDamagable, IHealable
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
    /// <summary> 無敵状態かどうか </summary>
    bool _isInvincibled = false;
    Coroutine _damageCoroutine;
    BossBattlePlayerMove _playerMove;
    #endregion
    #region public
    #endregion
    #region property
    public bool IsInvincibled => _isInvincibled;
    public bool IsMaxHP { get; private set; } = true;
    #endregion

    protected override void Awake()
    {
        base.Awake();
        TryGetComponent(out _playerMove);
        //_currentHP.Value = _maxHP;
    }
    void Start()
    {
        _fc.ChangeFaceType(FaceType.Blink);
        HPManager.Instance.ChangeHPValue(_maxHP, true);
        BossStageManager.Instance.GameOver += StopAction;
        EventManager.ListenEvents(Events.BossStage_FrontAlice, () => _fc.ChangeFaceType(FaceType.Fancy));
        EventManager.ListenEvents(Events.BossStage_HeadingBossFeet, () => _fc.ChangeFaceType(FaceType.Blink));
    }

    /// <summary>
    /// ダメージを受ける
    /// </summary>
    /// <param name="value"> ダメージ量 </param>
    public void Damage(int value)
    {
        if (_isInvincibled || !BossStageManager.Instance.IsInBattle)
        {
            return;
        }

        _damageCoroutine = StartCoroutine(DamageCoroutine(value));
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

    void StopAction()
    {
        if (_damageCoroutine != null)
        {
            StopCoroutine(_damageCoroutine);
            _damageCoroutine = null;
            _playerModel.SetActive(true);
        }

        _isInvincibled = true;
        _fc.ChangeFaceType(FaceType.Damage);
    }

    /// <summary>
    /// ダメージ演出のコルーチン
    /// </summary>
    /// <param name="damageValue"> ダメージの値 </param>
    /// <returns></returns>
    IEnumerator DamageCoroutine(int damageValue)
    {
        //既にHPが消失している場合は処理を行わない
        if (HPManager.Instance.IsLosted)
        {
            yield break;
        }

        _isInvincibled = true;
        _fc.ChangeFaceType(FaceType.Damage);
        IsMaxHP = false;
        HPManager.Instance.ChangeHPValue(damageValue);
        AudioManager.PlaySE(SEType.Player_Damage);

        var wait = new WaitForSeconds(_blinksInterVal);

        for (int i = 0; i < _blinksNum; i++)
        {
            _playerModel.SetActive(!_playerModel.activeSelf);
            yield return wait;
        }

        _isInvincibled = false;
        _playerModel.SetActive(true);
        _fc.ChangeFaceType(FaceType.Blink);
        _damageCoroutine = null;
    }
}
