using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 演出用のボスの機能を持つコンポーネント
/// </summary>
public class DirectionBossController : MonoBehaviour
{
    #region serialize
    #endregion

    #region private
    Animator _anim;
    #endregion

    #region public
    #endregion

    #region property
    #endregion

    private void Awake()
    {
        TryGetComponent(out _anim);
    }
    private void Start()
    {
        EventManager.ListenEvents(Events.BossStage_BehindBoss, () => ChangeAnimation(DirectionBossAnimationType.Point));
        EventManager.ListenEvents(Events.BossStage_QueenAnger, () => ChangeAnimation(DirectionBossAnimationType.Anger));
        EventManager.ListenEvents(Events.BossStage_ZoomBossFace, () =>
        {
            ChangeAnimation(DirectionBossAnimationType.Order);
            AudioManager.PlaySE(SEType.BossStage_QueenQuiet);
        });
        EventManager.ListenEvents(Events.BossStage_LookOther, () => 
        {
            ChangeAnimation(DirectionBossAnimationType.Point);
            AudioManager.PlaySE(SEType.BossStage_QueenQuiet);
        });
    }

    void ChangeAnimation(DirectionBossAnimationType type, float crossFadeTime = 0.2f)
    {
        _anim.CrossFadeInFixedTime(type.ToString(), crossFadeTime);
    }

    public enum DirectionBossAnimationType
    {
        Idle,
        Order,
        Anger,
        Look,
        No,
        Point
    }
}
