using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using DG.Tweening;

/// <summary>
/// ボスのHPを管理するマネージャークラス
/// </summary>
public class BossHPManager : MonoBehaviour
{
    #region serialize
    [Header("Variables")]
    [Tooltip("HPゲージの現在ステータス(確認用)")]
    [SerializeField]
    HPGaugeState _currentGaugeState = HPGaugeState.Three;

    [Header("UI")]
    [SerializeField]
    Transform _hpGaugeUITrans = default;
    
    [SerializeField]
    Image[] _hpImages = default;

    [SerializeField]
    Sprite[] _hpSprites = default;
    #endregion

    #region private
    ReactiveProperty<int> _currentHP = new ReactiveProperty<int>();
    #endregion

    #region public
    #endregion

    #region property
    public static BossHPManager Instance { get; private set; }
    #endregion

    private void Awake()
    {
        Instance = this;

        _currentHP.Subscribe(value =>
        {
            _currentGaugeState = (HPGaugeState)value;

            switch (_currentGaugeState)
            {
                case HPGaugeState.Zero:
                    for (int i = 0; i < _hpImages.Length; i++)
                    {
                        _hpImages[i].sprite = _hpSprites[0];
                    }
                    break;
                case HPGaugeState.One:
                    _hpImages[0].sprite = _hpSprites[1];
                    _hpImages[1].sprite = _hpSprites[0];
                    _hpImages[2].sprite = _hpSprites[0];
                    break;
                case HPGaugeState.Two:
                    _hpImages[0].sprite = _hpSprites[1];
                    _hpImages[1].sprite = _hpSprites[2];
                    _hpImages[2].sprite = _hpSprites[0];
                    break;
                case HPGaugeState.Three:
                    _hpImages[0].sprite = _hpSprites[1];
                    _hpImages[1].sprite = _hpSprites[2];
                    _hpImages[2].sprite = _hpSprites[3];
                    break;
                default:
                    break;
            }
        })
        .AddTo(this);
    }

    private void Start()
    {
        _currentHP.Value = 3;
    }

    public void Damage()
    {
        _currentHP.Value--;

        _hpGaugeUITrans.DOShakePosition(0.5f, 20, 30);
    }
}
