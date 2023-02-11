using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LobbyClockController : MonoBehaviour
{
    #region serialize
    [Tooltip("Œ»İ‚ÌŠÔ")]
    [SerializeField]
    ClockState _currentClockState = ClockState.Zero;

    [Header("Variables")]
    [Tooltip("‹¶‚Á‚½‚Ìj‚Ì‰ñ“]ŠÔ")]
    [SerializeField]
    float _hourHandCrazyRotateTime = 10.0f;

    [Tooltip("j‚ğŒv‰ñ‚è‚É‚·‚é‚©‚Ç‚¤‚©")]
    [SerializeField]
    bool _isHourHandClockWise = true;

    [Tooltip("‹¶‚Á‚½‚Ì•ªj‚Ì‰ñ“]ŠÔ")]
    [SerializeField]
    float _minuteHandCrazyRotateTime = 3.0f;

    [Tooltip("•ªj‚ğŒv‰ñ‚è‚É‚·‚é‚©‚Ç‚¤‚©")]
    [SerializeField]
    bool _isMinuteHandClockWise = true;

    [Tooltip("Œv‚ª‹¶‚Á‚Ä‚¢‚éŠÔ")]
    [SerializeField]
    float _crazyClockTime = 3.0f;

    [Tooltip("j")]
    [SerializeField]
    Transform _hourHand = default;

    [Tooltip("•ªj")]
    [SerializeField]
    Transform _minuteHand = default;

    [Tooltip("j")]
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

    public void ChangeClockState(ClockState state, float animTime = 3f, float derayTime = 4.0f, Action action = null, bool isPlaySE = false)
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
            if (animTime <= 0)
            {
                _hourHand.DOLocalRotate(hourRotate, _hourHandCrazyRotateTime, _isHourHandClockWise ? RotateMode.FastBeyond360 : RotateMode.Fast);

                _minuteHand.DOLocalRotate(secondRotate, _minuteHandCrazyRotateTime, _isMinuteHandClockWise ? RotateMode.FastBeyond360 : RotateMode.Fast);
            }
            else
            {
                _hourHand.DOLocalRotate(hourRotate, animTime, _isHourHandClockWise ? RotateMode.FastBeyond360 : RotateMode.Fast);

                _minuteHand.DOLocalRotate(secondRotate, animTime, _isMinuteHandClockWise ? RotateMode.FastBeyond360 : RotateMode.Fast);
            }
        }
        else
        {
            _hourHand.DOLocalRotate(hourRotate, animTime)
                     .SetDelay(derayTime);

            _minuteHand.DOLocalRotate(secondRotate, animTime)
                       .OnComplete(() =>
                       {
                           if (isPlaySE)
                           {
                               AudioManager.StopSE();
                               AudioManager.PlaySE(SEType.Lobby_StopClock);
                               VibrationController.OnVibration(Strength.Middle, 0.3f);
                           }
                           GameManager.UpdateCurrentClock(state);
                           action?.Invoke();
                       })
                       .SetDelay(derayTime)
                       .OnStart(() =>
                       {
                           if (isPlaySE)
                           {
                               AudioManager.PlaySE(SEType.Lobby_ClockMove);
                               VibrationController.OnVibration(Strength.Low, animTime - 0.2f);
                           }
                       });
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
    /// ‹¶‚Á‚½Œv‚Ì‰‰o‚ğÄ¶‚·‚é
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

    public IEnumerator CrazyClockCoroutine(float crazyTime = 0f, float returnTime = 0f)
    {
        OnCrazyClock();

        if (crazyTime <= 0)
        {
            yield return new WaitForSeconds(_crazyClockTime);
        }
        else
        {
            yield return new WaitForSeconds(crazyTime);
        }

        ChangeClockState(ClockState.Twelve, animTime: returnTime, isPlaySE: true);
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
