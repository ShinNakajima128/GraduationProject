using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// アリスの演出を管理するコンポーネント
/// </summary>
public class AliceDirctionControlller : MonoBehaviour
{
    #region serialize
    [Header("Variables")]
    [Tooltip("最初に浮き上がる高さ")]
    [SerializeField]
    float _floatingHeight = 2.0f;

    [Tooltip("アニメーションの時間")]
    [SerializeField]
    float _animTime = 2.0f;

    [Tooltip("アニメーションの動きの種類")]
    [SerializeField]
    Ease _animEase = default;

    [Tooltip("更に上昇する高さ")]
    [SerializeField]
    float _furtherRiseHeight = 10;

    [Tooltip("更に上昇時のアニメーション時間")]
    [SerializeField]
    float _furtherRiseAnimTime = 30.0f;

    [Tooltip("更に上昇時のアニメーションの動きの種類")]
    [SerializeField]
    Ease _furtherRiseAnimEase = default;

    [Tooltip("アリスのRenderer")]
    [SerializeField]
    Renderer _aliceRenderer = default;

    [Header("Effects")]
    [SerializeField]
    GameObject _floatingEffect = default;
    #endregion

    #region private
    AliceFaceController _fc;
    Animator _anim;
    Tween _floatTween;
    #endregion

    #region public
    #endregion

    #region property
    #endregion

    private void Awake()
    {
        TryGetComponent(out _fc);
        TryGetComponent(out _anim);
        _floatingEffect.SetActive(false);
    }

    private void Start()
    {
        EventManager.ListenEvents(Events.BossStage_End_Start, () => _fc.ChangeFaceType(FaceType.Talking));
        EventManager.ListenEvents(Events.BossStage_End_AliceFront, () => _fc.ChangeFaceType(FaceType.Talking));
        EventManager.ListenEvents(Events.Alice_Talking, () => _fc.ChangeFaceType(FaceType.Talking));
        EventManager.ListenEvents(Events.FinishTalking, StopTalking);
        EventManager.ListenEvents(Events.BossStage_End_ZoomAlice, () => 
        {
            ChangeAnimation(AliceDirectionAnimType.OpenArms);
            _fc.ChangeFaceType(FaceType.Talking);
        });
        EventManager.ListenEvents(Events.BossStage_End_AliceOblique, () =>
        {
            ChangeAnimation(AliceDirectionAnimType.No);
            _fc.ChangeFaceType(FaceType.Talking);
        });
        EventManager.ListenEvents(Events.BossStage_End_AliceDiagonallyBack, () =>
        {
            ChangeAnimation(AliceDirectionAnimType.Doubts);
            _fc.ChangeFaceType(FaceType.Talking);
        });
        EventManager.ListenEvents(Events.BossStage_End_CheshireFront, () =>
        {
            _aliceRenderer.enabled = false;
        });
        EventManager.ListenEvents(Events.BossStage_End_AliceFloat1, () =>
        {
            _aliceRenderer.enabled = true;
            ChangeAnimation(AliceDirectionAnimType.Float);
            _fc.ChangeFaceType(FaceType.Fancy);
            _fc.ChangeFaceType(FaceType.Talking);
            OnFloating();
            _floatingEffect.SetActive(true);
        });
        EventManager.ListenEvents(Events.BossStage_End_AliceFloat2, () =>
        {
            _fc.ChangeFaceType(FaceType.Fancy);
            _fc.ChangeFaceType(FaceType.Talking);
        });
        EventManager.ListenEvents(Events.BossStage_End_AliceLookDown, () =>
        {
            OnFurtherRise();
        });
        EventManager.ListenEvents(Events.BossStage_End_CheshireLookUp, () =>
        {
            FixationHeight();
        });
        EventManager.ListenEvents(Events.BossStage_End_AliceZoomUp, () =>
        {
            _fc.transform.DOLocalRotate(Vector3.zero, 0f);
            _fc.ChangeFaceType(FaceType.Cry);
            _fc.ChangeFaceType(FaceType.Talking);
        });
        EventManager.ListenEvents(Events.BossStage_End_AliceCloseEyes, () =>
        {
            //ChangeAnimation(AliceDirectionAnimType.No);
            _fc.ChangeFaceType(FaceType.Tolerance);
            _fc.ChangeFaceType(FaceType.Talking);
        });
        EventManager.ListenEvents(Events.BossStage_End_AliceSmile, () =>
        {
            _fc.ChangeFaceType(FaceType.Smile);
            _fc.ChangeFaceType(FaceType.Talking);
        });
    }

    /// <summary>
    /// 浮き上がる
    /// </summary>
    void OnFloating()
    {
        _fc.transform.DOLocalMoveY(_fc.transform.localPosition.y + _floatingHeight, _animTime)
                     .SetEase(_animEase)
                     .SetDelay(1f);
    }

    /// <summary>
    /// 更に上昇する
    /// </summary>
    void OnFurtherRise()
    {
        _fc.transform.DOLocalRotate(new Vector3(30, 0, 0), 0f);
        _fc.transform.DOMoveY(4, 0f);
        _floatTween = _fc.transform.DOMoveY(_fc.transform.position.y + _furtherRiseHeight, _furtherRiseAnimTime)
                                   .SetEase(_furtherRiseAnimEase);
    }

    //アリスの高度を固定する
    void FixationHeight()
    {
        _floatTween.Kill();
        _floatTween = null;

        _fc.transform.DOMoveY(4f, 0f);
    }

    void ChangeAnimation(AliceDirectionAnimType type, float crossTime = 0.2f)
    {
        _anim.CrossFadeInFixedTime(type.ToString(), crossTime);
    }

    void StopTalking()
    {
        if (!_fc.IsTalking)
        {
            return;
        }

        _fc.FinishTalk();
    }
}
