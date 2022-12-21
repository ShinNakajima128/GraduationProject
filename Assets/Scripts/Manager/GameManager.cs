using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AliceProject;

public enum Stages
{
    Stage1 = 0,
    Stage2 = 1,
    Stage3 = 2,
    Stage4 = 3,
    Stage_Boss = 6,
    StageNum = 7
}

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    #region serialize
    [SerializeField]
    Stages _currentStage = default;

    [SerializeField]
    ClockState _currentClockState = ClockState.Zero;

    [Header("Debug:Lobby")]
    [SerializeField]
    bool _lobbyDebugMode = false;

    [SerializeField]
    ClockState _debugClockState = default;
    #endregion

    #region private
    Dictionary<Stages, bool> _stageStatusDic = new Dictionary<Stages, bool>();
    bool _isClearStaged = false;
    #endregion
    #region property
    public Stages CurrentStage => _currentStage;
    public ClockState CurrentClockState => _currentClockState;
    public static bool IsClearStaged => Instance._isClearStaged;
    #endregion

    private void Awake()
    {
        if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        for (int i = 0; i < (int)Stages.StageNum; i++)
        {
            _stageStatusDic.Add((Stages)i, false);
        }
    }
    /// <summary>
    /// ステージのクリア状況を更新する
    /// </summary>
    /// <param name="stage"> 更新するステージ </param>
    public static void UpdateStageStatus(Stages stage)
    {
        Instance._stageStatusDic[stage] = true;
    }
    /// <summary>
    /// 現在のロビーの時計の状態を更新する
    /// </summary>
    /// <param name="state"> 更新する時間 </param>
    public static void UpdateCurrentClock(ClockState state)
    {
        Instance._currentClockState = state;
    }
    /// <summary>
    /// 保持するステージ情報を更新する
    /// </summary>
    /// <param name="stage"> 更新するステージ </param>
    public static void UpdateCurrentStage(Stages stage)
    {
        Instance._currentStage = stage;
        Instance._isClearStaged = false;
    }
    /// <summary>
    /// GameManagerが保持しているステージのクリア状況を確認する
    /// </summary>
    /// <returns> クリアフラグ </returns>
    public static bool CheckStageStatus()
    {
        //現在のステージの情報を取得
        var stage = Instance._stageStatusDic.FirstOrDefault(s => s.Key == Instance._currentStage);

        //クリア済みかどうかを返す
        return stage.Value;
    }

    /// <summary>
    /// 各ゲームの結果をセーブする
    /// </summary>
    /// <param name="result"> ミニゲームの結果 </param>
    public static void SaveStageResult(bool result)
    {
        Instance._isClearStaged = result;
    }

    /// <summary>
    /// 任意のステージのクリア状況を確認する
    /// </summary>
    /// <param name="stage"> 確認するステージ </param>
    /// <returns> クリアフラグ </returns>
    public static bool CheckStageStatus(Stages stage)
    {
        var s = Instance._stageStatusDic.First(s => s.Key == stage);

        return s.Value;
    }
    /// <summary>
    /// 現在の時計の状況を確認する
    /// </summary>
    /// <returns> 時計のステータス </returns>
    public static ClockState CheckGameStatus()
    {
        if (Instance._lobbyDebugMode)
        {
            return Instance._debugClockState;
        }

        var count = Instance._stageStatusDic.Count(s => s.Value == true);
        Debug.Log(count);
        ClockState state = ClockState.Zero;

        switch (count)
        {
            case 0:
                state = ClockState.Three;
                break;
            case 1:
                state = ClockState.Six;
                break;
            case 2:
                state = ClockState.Nine;
                break;
            case 3:
                state = ClockState.Twelve;
                break;
            default:
                state = ClockState.Zero;
                break;
        }
        return state;
    }

    /// <summary>
    /// ゲームの状態をリセットする
    /// </summary>
    public static void GameReset()
    {
        Instance._currentClockState = ClockState.Zero;
        LobbyManager.Reset();

        for (int i = 0; i < Instance._stageStatusDic.Count; i++)
        {
            Instance._stageStatusDic[(Stages)i] = false;
        }

        Debug.Log("データをリセットしました");
    }

    /// <summary>
    /// ステージクリア時のスチル獲得の演出を再生する
    /// </summary>
    /// <param name="stage"> クリアしたステージ </param>
    /// <param name="type"> 再生するメッセージ </param>
    /// <returns></returns>
    public static IEnumerator GetStillDirectionCoroutine(Stages stage, MessageType type)
    {
        TransitionManager.FadeIn(FadeType.White_Transparent, 0f);
        TransitionManager.FadeIn(FadeType.Normal, action: () =>
        {
            GetStillController.ActiveGettingStillPanel(stage);

            TransitionManager.FadeOut(FadeType.Normal);
        });

        yield return new WaitForSeconds(3.0f);

        yield return MessagePlayer.Instance.PlayMessageCorountine(type);
    }
}
