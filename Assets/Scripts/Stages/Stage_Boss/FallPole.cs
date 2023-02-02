using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FallPole : MonoBehaviour
{
    #region serialize
    [Header("Variables")]
    [Tooltip("プレイヤーに与えるダメージ")]
    [SerializeField]
    int _attackValue = 1;

    [SerializeField]
    float _fallTime = 3.0f;

    [SerializeField]
    float _startHeight = 5.0f;

    [SerializeField]
    Ease _fallPoleEase = default;

    [Header("Components")]
    [SerializeField]
    AttackCollider _attackCollider = default;
    #endregion

    #region private
    #endregion

    #region public
    #endregion

    #region property
    #endregion

    private void OnEnable()
    {
        if (BossStageManager.Instance.IsInBattle)
        {
            _attackCollider.enabled = true;
        }
        else
        {
            _attackCollider.enabled = false;
        }
    }

    private void OnDisable()
    {
        transform.localPosition = new Vector3(0, transform.localPosition.y + _startHeight, 0);
    }

    private void Start()
    {
        OnAnimation();
    }

    /// <summary>
    /// アニメーションを再生する
    /// </summary>
    public void OnAnimation(Action action = null)
    {
        transform.DOLocalMoveY(1f, _fallTime)
                 .SetEase(_fallPoleEase)
                 .OnComplete(() =>
                 {
                     EffectManager.PlayEffect(EffectType.FallPole, transform);
                     _attackCollider.enabled = false;
                     AudioManager.PlaySE(SEType.BossStage_DebrisLanding);
                 })
                 .SetLink(gameObject, LinkBehaviour.KillOnDisable);
    }
}
