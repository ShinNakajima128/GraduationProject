using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LobbyClockController : MonoBehaviour
{
    #region serialize
    [Tooltip("åªç›ÇÃéûä‘")]
    [SerializeField]
    ClockState _currentClockState = ClockState.Zero;

    [Header("Variables")]
    [Tooltip("ã∂Ç¡ÇΩéûÇÃéûêjÇÃâÒì]éûä‘")]
    [SerializeField]
    float _hourHandCrazyRotateTime = 10.0f;

    [Tooltip("éûêjÇéûåvâÒÇËÇ…Ç∑ÇÈÇ©Ç«Ç§Ç©")]
    [SerializeField]
    bool _isHourHandClockWise = true;

    [Tooltip("ã∂Ç¡ÇΩéûÇÃï™êjÇÃâÒì]éûä‘")]
    [SerializeField]
    float _minuteHandCrazyRotateTime = 3.0f;

    [Tooltip("ï™êjÇéûåvâÒÇËÇ…Ç∑ÇÈÇ©Ç«Ç§Ç©")]
    [SerializeField]
    bool _isMinuteHandClockWise = true;

    [Tooltip("éûåvÇ™ã∂Ç¡ÇƒÇ¢ÇÈéûä‘")]
    [SerializeField]
    float _CrazyClockTime = 3.0f;

    [Tooltip("éûêj")]
    [SerializeField]
    Transform _hourHand = default;

    [Tooltip("ï™êj")]
    [SerializeField]
    Transform _minuteHand = default;

    [Tooltip("éûêj")]
    [SerializeField]
    Transform _secondHand = default;
    #endregion

    #region private
    bool _isCrazing = false;
    Tween _currentHourTween;
    Tween _currentMinuteTween;
    #endregion
    
    #region property
    #endregion

    private void OnValidate()
    {
        //ChangeClockState(_currentClockState);
    }

    void Start()
    {
        //StartCoroutine(SecondHandMove());
        //OnCrazyClock();
        //StartCoroutine(CrazyClockCoroutine());
    }

    public void ChangeClockState(ClockState state, float animTime = 3f, float derayTime = 4.0f, Action action = null)
    {
        if (_isCrazing)
        {
            _currentHourTween.Kill();
            _currentMinuteTween.Kill();

            _currentHourTween = null;
            _currentMinuteTween = null;
        }

        Vector3 hourRotate = default;
        Vector3 secondRotate = default;
        
        Debug.Log(state);

        switch (state)
        {
            case ClockState.Zero:
                hourRotate = new Vector3(0f, 330f, 0f);
                secondRotate = new Vector3(0f, 0f, 0f);
                break;
            case ClockState.One:
                hourRotate = new Vector3(0f, 332.5f, 0f);
                secondRotate = new Vector3(0f, 30f, 0f);
                break;
            case ClockState.Two:
                hourRotate = new Vector3(0f, 335f, 0f);
                secondRotate = new Vector3(0f, 60f, 0f);
                break;
            case ClockState.Three:
                hourRotate = new Vector3(0f, 337.5f, 0f);
                secondRotate = new Vector3(0f, 90f, 0f);
                break;
            case ClockState.Four:
                hourRotate = new Vector3(0f, 340f, 0f);
                secondRotate = new Vector3(0f, 120f, 0f);
                break;
            case ClockState.Five:
                hourRotate = new Vector3(0f, 342.5f, 0f);
                secondRotate = new Vector3(0f, 150f, 0f);
                break;
            case ClockState.Six:
                hourRotate = new Vector3(0f, 345f, 0f);
                secondRotate = new Vector3(0f, 180f, 0f);
                break;
            case ClockState.Seven:
                hourRotate = new Vector3(0f, 347.5f, 0f);
                secondRotate = new Vector3(0f, 210f, 0f);
                break;
            case ClockState.Eight:
                hourRotate = new Vector3(0f, 350f, 0f);
                secondRotate = new Vector3(0f, 240f, 0f);
                break;
            case ClockState.Nine:
                hourRotate = new Vector3(0f, 352.5f, 0f);
                secondRotate = new Vector3(0f, 270f, 0f);
                break;
            case ClockState.Ten:
                hourRotate = new Vector3(0f, 355f, 0f);
                secondRotate = new Vector3(0f, 300f, 0f);
                break;
            case ClockState.Eleven:
                hourRotate = new Vector3(0f, 357.5f, 0f);
                secondRotate = new Vector3(0f, 330f, 0f);
                break;
            case ClockState.Twelve:
                hourRotate = new Vector3(0f, 360f, 0f);
                secondRotate = new Vector3(0f, 360f, 0f);

                break;
            default:
                break;
        }

        if (_isCrazing)
        {
            _hourHand.DOLocalRotate(hourRotate, _hourHandCrazyRotateTime);

            _minuteHand.DOLocalRotate(secondRotate, _minuteHandCrazyRotateTime);
        }
        else
        {
            _hourHand.DOLocalRotate(hourRotate, animTime)
                 .SetDelay(derayTime);

            _minuteHand.DOLocalRotate(secondRotate, animTime)
                .OnComplete(() =>
                {
                    GameManager.UpdateCurrentClock(state);
                    action?.Invoke();
                })
                .SetDelay(derayTime);
        }
        

        _isCrazing = false;
    }

    IEnumerator SecondHandMove()
    {
        int rotateValue = 0;

        while (true)
        {
            if (rotateValue >= 360)
            {
                rotateValue = 0;
            }

            yield return new WaitForSeconds(1.0f);

            rotateValue += 6;

            _secondHand.DOLocalRotate(new Vector3(0f, rotateValue, 0f), 0f);
        }
    }

    /// <summary>
    /// ã∂Ç¡ÇΩéûåvÇÃââèoÇçƒê∂Ç∑ÇÈ
    /// </summary>
    public void OnCrazyClock()
    {
        _isCrazing = true;
        _currentHourTween = _hourHand.DOLocalRotate(new Vector3(0, _isHourHandClockWise ? 360 : -360, 0), _hourHandCrazyRotateTime, RotateMode.FastBeyond360)
                                     .SetEase(Ease.Linear)
                                     .SetLoops(-1);

        _currentMinuteTween = _minuteHand.DOLocalRotate(new Vector3(0, _isMinuteHandClockWise ? 360 : -360, 0), _minuteHandCrazyRotateTime, RotateMode.FastBeyond360)
                                     .SetEase(Ease.Linear)
                                     .SetLoops(-1);
    }

    public IEnumerator CrazyClockCoroutine()
    {
        OnCrazyClock();

        yield return new WaitForSeconds(_CrazyClockTime);

        ChangeClockState(ClockState.Twelve);
    }
}
public enum ClockState
{
    Zero,
    One,
    Two,
    Three,
    Four,
    Five,
    Six,
    Seven,
    Eight,
    Nine,
    Ten,
    Eleven,
    Twelve
}
