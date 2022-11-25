using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Stages
{
    Stage1 = 0,
    Stage2 = 1,
    Stage3 = 2,
    Stage4 = 3,
    Stage5 = 4,
    Stage6 = 5,
    StageNum = 6
}

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    #region serialize
    [SerializeField]
    Stages _currentStage = default;

    [SerializeField]
    ClockState _currentClockState = ClockState.Zero;
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
    public static ClockState CheckGameStatus()
    {
        var count = Instance._stageStatusDic.Count(s => s.Value == true);
        Debug.Log(count);
        ClockState state = ClockState.Zero;

        switch (count)
        {
            case 0:
                state = ClockState.Two;
                break;
            case 1:
                state = ClockState.Four;
                break;
            case 2:
                state = ClockState.Six;
                break;
            case 3:
                state = ClockState.Eight;
                break;
            case 4:
                state = ClockState.Ten;
                break;
            case 5:
                state = ClockState.Twelve;
                break;
            default:
                break;
        }
        return state;
    }
}
