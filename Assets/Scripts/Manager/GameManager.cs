using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AliceProject;


/// <summary>
/// ゲーム全体を管理するマネージャークラス
/// </summary>
public class GameManager : SingletonMonoBehaviour<GameManager>
{
    #region serialize
    [Tooltip("現在のステージ")]
    [SerializeField]
    Stages _currentStage = default;

    [Tooltip("現在の時計の状態")]
    [SerializeField]
    ClockState _currentClockState = ClockState.Zero;

    [Tooltip("現在のロビーの状態")]
    [SerializeField]
    LobbyState _currentLobbyState = default;

    [Tooltip("現在のゲームの難易度")]
    [SerializeField]
    DifficultyType _currentGameDifficultyType = default;

    [Header("Debug:Lobby")]
    [SerializeField]
    bool _lobbyDebugMode = false;

    [SerializeField]
    ClockState _debugClockState = default;
    #endregion

    #region private
    Dictionary<Stages, bool> _stageStatusDic = new Dictionary<Stages, bool>();
    bool _isClearStaged = false;
    bool[] _isFirstVisitStages = new bool[(int)Stages.StageNum];
    #endregion
    #region property
    public Stages CurrentStage => _currentStage;
    public ClockState CurrentClockState => _currentClockState;
    public LobbyState CurrentLobbyState => _currentLobbyState;
    public DifficultyType CurrentGameDifficultyType => _currentGameDifficultyType;
    public static bool IsClearStaged => Instance._isClearStaged;
    public Dictionary<Stages, bool> StageSttatusDic => _stageStatusDic;
    public bool IsFirstVisitCurrentStage => _isFirstVisitStages[(int)_currentStage];
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
            _isFirstVisitStages[i] = true;
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
    /// 訪れたステージをプレイ済みにする
    /// </summary>
    /// <param name="stage"> 訪れたステージ </param>
    public static void UpdateFirstVisit(Stages stage)
    {
        Instance._isFirstVisitStages[(int)stage] = false;
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
    /// 現在のロビーの状態を変更する
    /// </summary>
    /// <param name="state"> 設定するロビーの状態 </param>
    public static void ChangeLobbyState(LobbyState state)
    {
        Instance._currentLobbyState = state;
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
    /// 難易度を変更する
    /// </summary>
    /// <param name="type"> 設定する難易度 </param>
    public static void ChangeGameDifficult(DifficultyType type)
    {
        Instance._currentGameDifficultyType = type;
    }

    /// <summary>
    /// ゲームの状態をリセットする
    /// </summary>
    public static void GameReset()
    {
        Instance._currentClockState = ClockState.Zero;
        Instance._currentGameDifficultyType = DifficultyType.Easy;
        Instance._currentLobbyState = LobbyState.Default;
        Instance._isClearStaged = false;
        FallGameManager.IsSecondTry = false;
        UnderLobbyManager.IsFirstVisit = true;
        BossStageManager.IsFirstVisit = true;
        LobbyManager.Reset();

        for (int i = 0; i < Instance._stageStatusDic.Count; i++)
        {
            Instance._stageStatusDic[(Stages)i] = false;
            Instance._isFirstVisitStages[i] = true;
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
            AudioManager.PlayBGM(BGMType.GetStill, false);
            TransitionManager.FadeOut(FadeType.Normal);
        });

        yield return new WaitForSeconds(1.5f);

        yield return GetStillController.ActiveGettingStillPanel(stage);

        yield return MessagePlayer.Instance.PlayMessageCorountine(type);
        ChangeLobbyState(LobbyState.Default);
    }
}

/// <summary>
/// ステージの種類
/// </summary>
public enum Stages
{
    Stage1 = 0,
    Stage2 = 1,
    Stage3 = 2,
    Stage4 = 3,
    Stage_Boss = 4,
    StageNum = 5
}

/// <summary>
/// 難易度の種類
/// </summary>
public enum DifficultyType
{
    Easy,
    Normal,
    Hard
}

/// <summary>
/// ロビーの状態
/// </summary>
public enum LobbyState
{
    Default,
    Under
}

