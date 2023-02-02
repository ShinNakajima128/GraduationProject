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

    [Tooltip("�_���[�W���̓_�ŉ�")]
    [SerializeField]
    int _blinksNum = 5;

    [Tooltip("�_���[�W���̓_�ŊԊu")]
    [SerializeField]
    float _blinksInterVal = 0.2f;

    [Header("Objects")]
    [Tooltip("�v���C���[�̃��f���I�u�W�F�N�g")]
    [SerializeField]
    GameObject _playerModel = default;
    #endregion

    #region private
    /// <summary> ���G��Ԃ��ǂ��� </summary>
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
    /// �_���[�W���󂯂�
    /// </summary>
    /// <param name="value"> �_���[�W�� </param>
    public void Damage(int value)
    {
        if (_isInvincibled || !BossStageManager.Instance.IsInBattle)
        {
            return;
        }

        _damageCoroutine = StartCoroutine(DamageCoroutine(value));
    }

    /// <summary>
    /// �̗͂��񕜂���
    /// </summary>
    /// <param name="value"> �񕜗� </param>
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
    /// �_���[�W���o�̃R���[�`��
    /// </summary>
    /// <param name="damageValue"> �_���[�W�̒l </param>
    /// <returns></returns>
    IEnumerator DamageCoroutine(int damageValue)
    {
        //����HP���������Ă���ꍇ�͏������s��Ȃ�
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
