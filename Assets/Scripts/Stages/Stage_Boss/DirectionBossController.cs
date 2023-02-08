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
    QueenFaceController _queenFc;
    #endregion

    #region public
    #endregion

    #region property
    #endregion

    private void Awake()
    {
        TryGetComponent(out _anim);
        TryGetComponent(out _queenFc);
    }
    private void Start()
    {
        if (!_isEndDirection)
        {
            //演出中:ボスの後ろからカメラが映るEvent
            EventManager.ListenEvents(Events.BossStage_BehindBoss, () => 
            {
                ChangeAnimation(DirectionBossAnimationType.Point);
                _queenFc.ChangeFaceType(QueenFaceType.Talking);
            });
            //演出中:ボス正面のカメラ
            EventManager.ListenEvents(Events.BossStage_FrontBoss, () =>
            {
                _queenFc.ChangeFaceType(QueenFaceType.Talking);
            });
            //演出中:ボスがわなわなと震える
            EventManager.ListenEvents(Events.BossStage_QueenAnger, () => 
            {
                ChangeAnimation(DirectionBossAnimationType.Anger);
            });
            //演出中:ボス顔アップ
            EventManager.ListenEvents(Events.BossStage_ZoomBossFace, () =>
            {
                ChangeAnimation(DirectionBossAnimationType.Order);
                AudioManager.PlaySE(SEType.BossStage_QueenQuiet);
                _queenFc.ChangeFaceType(QueenFaceType.Angry);
                _queenFc.ChangeFaceType(QueenFaceType.Talking);
            });
            //演出中:そっぽをむく
            EventManager.ListenEvents(Events.BossStage_LookOther, () =>
            {
                ChangeAnimation(DirectionBossAnimationType.Point);
                AudioManager.PlaySE(SEType.BossStage_QueenQuiet);
                _queenFc.ChangeFaceType(QueenFaceType.Talking);
            });
        }
        else
        {
            EventManager.ListenEvents(Events.BossStage_End_OnsideQueen, () => 
            {
                ChangeAnimation(DirectionBossAnimationType.Injury);
                _queenFc.ChangeFaceType(QueenFaceType.Talking);
            });
            EventManager.ListenEvents(Events.BossStage_End_GoAroundFrontQueen,() => 
            {
                ChangeAnimation(DirectionBossAnimationType.Anger);
                _queenFc.ChangeFaceType(QueenFaceType.Talking);
            });
            EventManager.ListenEvents(Events.BossStage_End_OnFace, () => 
            {
                ChangeAnimation(DirectionBossAnimationType.Point);
                _queenFc.ChangeFaceType(QueenFaceType.Angry);
                _queenFc.ChangeFaceType(QueenFaceType.Talking);
            });
            EventManager.ListenEvents(Events.BossStage_End_FrontQueen, () =>
            {
                ChangeAnimation(DirectionBossAnimationType.Look);
                _queenFc.ChangeFaceType(QueenFaceType.Talking);
            });
            
            EventManager.ListenEvents(Events.BossStage_End_ShakeHead, () =>
            {
                ChangeAnimation(DirectionBossAnimationType.No);
                _queenFc.ChangeFaceType(QueenFaceType.Talking);
            });

            EventManager.ListenEvents(Events.BossStage_End_CheshireFront, () =>
            {
                _queenFc.ChangeFaceType(QueenFaceType.Default);
            });

            EventManager.ListenEvents(Events.BossStage_End_CheshireSmile, () => ChangeAnimation(DirectionBossAnimationType.End_UP_Idle));
        }
        EventManager.ListenEvents(Events.FinishTalking, StopTalking);
        _queenFc.ChangeFaceType(QueenFaceType.Blink);
    }

    void ChangeAnimation(DirectionBossAnimationType type, float crossFadeTime = 0.2f)
    {
        _anim.CrossFadeInFixedTime(type.ToString(), crossFadeTime);
    }

    void StopTalking()
    {
        if (!_queenFc.IsTalking)
        {
            return;
        }

        _queenFc.FinishTalk();
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
