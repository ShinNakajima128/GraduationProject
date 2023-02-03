﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// イベントの種類
/// </summary>
public enum Events
{
    None,
    CameraShake,
    ConcentratedLine,
    Closeup,
    Reset,
    Lobby_MeetingCheshire,
    Cheshire_StartGrinning,
    Cheshire_EndGrinning,
    Cheshire_Talk,
    FinishTalking,
    Boss_GroundShake,
    Alice_Yes,
    Alice_No,
    Alice_Overlook,
    Alice_Surprised,
    Alice_Retry,
    Alice_Rise,
    Alice_Death,
    Lobby_Introduction,
    BossStage_FrontAlice,
    BossStage_HeadingBossFeet,
    BossStage_SlowlyRise,
    BossStage_OnBossFace,
    BossStage_BehindBoss,
    BossStage_BehindAlice,
    BossStage_FrontBoss,
    BossStage_ZoomBossFace,
    BossStage_BehindBoss_RE2,
    BossStage_LookOther,
    BossStage_QueenAnger,
    BossStage_BehindAlice_RE2,
    BossStage_FrontCheshire,
    BossStage_DisolveCheshire
}

/// <summary>
/// イベントを管理するクラス
/// </summary>
public class EventManager : MonoBehaviour
{
    /// <summary> 各イベントを管理するDictionary </summary>
    Dictionary<Events, Action> m_eventDic = new Dictionary<Events, Action>();

    public static EventManager Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// イベントを登録する
    /// </summary>
    /// <param name="events"> イベントの種類 </param>
    /// <param name="action"> 追加する処理 </param>
    public static void ListenEvents(Events events, Action action)
    {
        if (Instance == null) return;
        Action thisEvent;
        if (Instance.m_eventDic.TryGetValue(events, out thisEvent))
        {
            thisEvent += action;

            Instance.m_eventDic[events] = thisEvent;
        }
        else
        {
            thisEvent += action;
            Instance.m_eventDic.Add(events, thisEvent);
        }
    }

    /// <summary>
    /// 登録したイベントを抹消する
    /// </summary>
    /// <param name="events"> イベントの種類 </param>
    /// <param name="action"> 抹消する処理 </param>
    public static void RemoveEvents(Events events, Action action)
    {
        if (Instance == null) return;
        Action thisEvent;
        if (Instance.m_eventDic.TryGetValue(events, out thisEvent))
        {
            thisEvent -= action;

            Instance.m_eventDic[events] = thisEvent;
        }
    }

    /// <summary>
    /// イベントを実行する
    /// </summary>
    /// <param name="events"> 実行するイベント </param>
    public static void OnEvent(Events events)
    {
        Action thisEvent;
        if (Instance.m_eventDic.TryGetValue(events, out thisEvent))
        {
            thisEvent?.Invoke();
        }
        else
        {
            return;
        }
    }
}
