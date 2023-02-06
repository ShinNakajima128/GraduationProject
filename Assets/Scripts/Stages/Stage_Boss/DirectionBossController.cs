using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 演出用のボスの機能を持つコンポーネント
/// </summary>
public class DirectionBossController : MonoBehaviour
{
    #region serialize
    [SerializeField]
    bool _isEndDirection = false;
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
        if (!_isEndDirection)
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
        else
        {
            EventManager.ListenEvents(Events.BossStage_End_OnsideQueen, () => ChangeAnimation(DirectionBossAnimationType.Injury));
            EventManager.ListenEvents(Events.BossStage_End_GoAroundFrontQueen, () => ChangeAnimation(DirectionBossAnimationType.Anger));
            EventManager.ListenEvents(Events.BossStage_End_OnFace, () => ChangeAnimation(DirectionBossAnimationType.Point));
            EventManager.ListenEvents(Events.BossStage_End_FrontQueen, () => ChangeAnimation(DirectionBossAnimationType.Look));
            EventManager.ListenEvents(Events.BossStage_End_ShakeHead, () => ChangeAnimation(DirectionBossAnimationType.No));
            EventManager.ListenEvents(Events.BossStage_End_CheshireSmile, () => ChangeAnimation(DirectionBossAnimationType.End_UP_Idle));
        }
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
        Point,
        Injury,
        End_Idle,
        End_UP_Idle
    }
}
