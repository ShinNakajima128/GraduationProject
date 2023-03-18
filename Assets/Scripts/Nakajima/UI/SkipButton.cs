using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;
using DG.Tweening;

public class SkipButton : MonoBehaviour
{
    #region serialize
    [Header("Variables")]
    [Tooltip("長押し時間")]
    [SerializeField]
    float _longPressTime = 3.0f;

    [SerializeField]
    float _buttonActiveTime = 2.0f;

    [Header("UIObjects")]
    [SerializeField]
    Image _fillAreaImage = default;
    #endregion

    #region private
    CanvasGroup _skipButtonGroup;
    Subject<Unit> _onSkip;
    Coroutine _skipCoroutine;
    Coroutine _fadeCoroutine;
    Tween _animTween;
    bool _isSkipCompleted = false;
    #endregion

    #region public
    /// <summary> スキップボタンが反応するかどうか </summary>
    public event Func<bool> Isrespond;
    #endregion

    #region property
    public static SkipButton Instance { get; private set; }
    public IObservable<Unit> OnSkip => _onSkip;
    #endregion

    private void Awake()
    {
        Instance = this;
        _onSkip = new Subject<Unit>();
        TryGetComponent(out _skipButtonGroup);
    }

    private void Start()
    {
        Setup();
    }

    public void InactiveButton()
    {
        OnFadeButton(0f, 0f);
    }

    public void ResetSubscribe()
    {
        _skipButtonGroup.alpha = 0;
        _isSkipCompleted = false;
        _fillAreaImage.fillAmount = 0;

        if (_onSkip != null)
        {
            _onSkip.Dispose();
            _onSkip = null;
        }
        _onSkip = new Subject<Unit>();
    }

    /// <summary>
    /// スキップボタンのセットアップ
    /// </summary>
    void Setup()
    {
        //スキップボタンを押した時
        this.UpdateAsObservable()
            .Where(_ => UIInput.SkipDown && !_isSkipCompleted)
            .Where(_ => Isrespond())
            .ThrottleFirst(TimeSpan.FromMilliseconds(100))
            .Subscribe(_ =>
            {
                if (_skipButtonGroup.alpha < 1)
                {
                    OnFadeButton(1, 0.15f);
                }

                if (_fadeCoroutine != null)
                {
                    StopCoroutine(_fadeCoroutine);
                    _fadeCoroutine = null;
                }

                if (Time.timeScale != 1)
                {
                    Time.timeScale = 1;
                }

                _skipCoroutine = StartCoroutine(StartSkipCoroutine());
            })
            .AddTo(this);

        //スキップボタンを離した時
        this.UpdateAsObservable()
            .Where(_ => UIInput.SkipUp && !_isSkipCompleted)
            .Where(_ => Isrespond())
            .ThrottleFirst(TimeSpan.FromMilliseconds(100))
            .Subscribe(_ =>
            {
                if (_fadeCoroutine != null)
                {
                    StopCoroutine(_fadeCoroutine);
                    _fadeCoroutine = null;
                }
                _fadeCoroutine = StartCoroutine(InactiveButtonCoroutine());

                if (_skipCoroutine != null)
                {
                    StopCoroutine(_skipCoroutine);
                    _skipCoroutine = null;
                }

                if (_animTween != null)
                {
                    _animTween.Kill();
                    _animTween = null;

                    _fillAreaImage.DOFillAmount(0f, 0.2f);
                }
            })
            .AddTo(this);
    }
    void OnFadeButton(float value, float fadeTime)
    {
        DOTween.To(() => _skipButtonGroup.alpha,
                x => _skipButtonGroup.alpha = x,
                value,
                fadeTime);
    }
    IEnumerator StartSkipCoroutine()
    {
        _animTween = _fillAreaImage.DOFillAmount(1f, _longPressTime)
                                   .SetEase(Ease.Linear);
                                   
        yield return _animTween.WaitForCompletion();
        _isSkipCompleted = true;
        _onSkip.OnNext(Unit.Default);
    }

    /// <summary>
    /// 一定時間後に非アクティブにするコルーチン
    /// </summary>
    IEnumerator InactiveButtonCoroutine()
    {
        yield return new WaitForSeconds(_buttonActiveTime);

        OnFadeButton(0, 0.5f);
    }
}
