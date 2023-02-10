using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class CreditManager : MonoBehaviour
{
    #region serialize
    [Header("Variables")]
    [Tooltip("加速時のスピード")]
    [SerializeField]
    float _speedUpScale = 2.5f;

    [SerializeField]
    CanvasGroup _thankForPlayingPanelGroup = default;

    [Header("Components")]
    [SerializeField]
    StaffRollContoller _staffRollCtrl = default;
    #endregion

    #region private
    #endregion

    #region public
    #endregion

    #region property
    #endregion

    IEnumerator Start()
    {
        TransitionManager.FadeOut(FadeType.Black_default);
        OperationSetup();

        _staffRollCtrl.OnComplitedAction += () =>
        {
            StartCoroutine(ReturnTitleCoroutine());
        };

        yield return new WaitForSeconds(0.2f);

        AudioManager.PlayBGM(BGMType.Credit);
    }

    void OperationSetup()
    {
        this.UpdateAsObservable()
            .Where(_ => UIInput.High && !_staffRollCtrl.IsScrollComplited)
            .ThrottleFirst(TimeSpan.FromMilliseconds(1000))
            .Subscribe(_ =>
            {
                Time.timeScale = _speedUpScale;
            })
            .AddTo(this);

        this.UpdateAsObservable()
            .Where(_ => UIInput.Low && !_staffRollCtrl.IsScrollComplited)
            .ThrottleFirst(TimeSpan.FromMilliseconds(1000))
            .Subscribe(_ =>
            {
                Time.timeScale = 1f;
            })
            .AddTo(this);

        _thankForPlayingPanelGroup.alpha = 0;

        SkipButton.Instance.Isrespond += () => !_staffRollCtrl.IsScrollComplited;
        SkipButton.Instance.OnSkip.Subscribe(_ =>
        {
            TransitionManager.FadeIn(FadeType.Black_Transparent, 0f);
            TransitionManager.SceneTransition(SceneType.Title);
        });
    }

    IEnumerator ReturnTitleCoroutine()
    {
        Time.timeScale = 1;

        yield return new WaitForSeconds(2.0f);

        TransitionManager.FadeIn(FadeType.White_Transparent, 0f);
        TransitionManager.FadeIn(FadeType.Normal);

        yield return new WaitForSeconds(1.5f);

        _thankForPlayingPanelGroup.alpha = 1;

        yield return new WaitForSeconds(0.5f);
        
        TransitionManager.FadeOut(FadeType.White_default);

        yield return new WaitForSeconds(2.5f);

        yield return new WaitUntil(() => UIInput.B);

        TransitionManager.FadeIn(FadeType.Black_Transparent, 0f);
        TransitionManager.SceneTransition(SceneType.Title);
    }
}
