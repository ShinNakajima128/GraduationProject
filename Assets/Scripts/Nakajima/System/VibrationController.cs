﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

/// <summary>
/// 振動の種類
/// </summary>
public enum Strength
{
    Low,
    Middle,
    High
}

/// <summary>
/// ゲームパッドの振動を管理するコントローラー
/// </summary>
public class VibrationController : MonoBehaviour
{
    [SerializeField]
    float _lowPowerValue = 0.5f;

    [SerializeField]
    float _middlePowerValue = 1.5f;

    [SerializeField]
    float _highPowerValue = 3;

    [SerializeField]
    float _duration = 0.5f;

    Coroutine _vibCoroutine;


    public static VibrationController Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// ゲームパッドを振動させる
    /// </summary>
    /// <param name="duration"> 振動させる時間 </param>
    public static void OnVibration(Strength type, float duration = 0.5f)
    {
        Instance._vibCoroutine = Instance.StartCoroutine(Instance.Vibration(type, duration));
    }
    IEnumerator Vibration(Strength type, float duration)
    {
        switch (type)
        {
            case Strength.Low:
                GamePad.SetVibration(0, Instance._lowPowerValue, Instance._lowPowerValue);
                break;
            case Strength.Middle:
                GamePad.SetVibration(0, Instance._middlePowerValue, Instance._middlePowerValue);
                break;
            case Strength.High:
                GamePad.SetVibration(0, Instance._highPowerValue, Instance._highPowerValue);
                break;
            default:
                break;
        }
        

        yield return new WaitForSecondsRealtime(duration);

        GamePad.SetVibration(0, 0, 0);
    }

    public static void OffVibration()
    {
        if (Instance._vibCoroutine != null)
        {
            Instance.StopCoroutine(Instance._vibCoroutine);
            Instance._vibCoroutine = null;
        }
        GamePad.SetVibration(0, 0, 0);
    }
}
