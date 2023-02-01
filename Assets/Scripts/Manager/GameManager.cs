using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AliceProject;


/// <summary>
/// �Q�[���S�̂��Ǘ�����}�l�[�W���[�N���X
/// </summary>
public class GameManager : SingletonMonoBehaviour<GameManager>
{
    #region serialize
    [Tooltip("���݂̃X�e�[�W")]
    [SerializeField]
    Stages _currentStage = default;

    [Tooltip("���݂̎��v�̏��")]
    [SerializeField]
    ClockState _currentClockState = ClockState.Zero;

    [Tooltip("���݂̃��r�[�̏��")]
    [SerializeField]
    LobbyState _currentLobbyState = default;

    [Tooltip("���݂̃Q�[���̓�Փx")]
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
    /// �X�e�[�W�̃N���A�󋵂��X�V����
    /// </summary>
    /// <param name="stage"> �X�V����X�e�[�W </param>
    public static void UpdateStageStatus(Stages stage)
    {
        Instance._stageStatusDic[stage] = true;
    }
    /// <summary>
    /// ���݂̃��r�[�̎��v�̏�Ԃ��X�V����
    /// </summary>
    /// <param name="state"> �X�V���鎞�� </param>
    public static void UpdateCurrentClock(ClockState state)
    {
        Instance._currentClockState = state;
    }
    /// <summary>
    /// �ێ�����X�e�[�W�����X�V����
    /// </summary>
    /// <param name="stage"> �X�V����X�e�[�W </param>
    public static void UpdateCurrentStage(Stages stage)
    {
        Instance._currentStage = stage;
        Instance._isClearStaged = false;
    }

    /// <summary>
    /// �K�ꂽ�X�e�[�W���v���C�ς݂ɂ���
    /// </summary>
    /// <param name="stage"> �K�ꂽ�X�e�[�W </param>
    public static void UpdateFirstVisit(Stages stage)
    {
        Instance._isFirstVisitStages[(int)stage] = false;
    }

    /// <summary>
    /// GameManager���ێ����Ă���X�e�[�W�̃N���A�󋵂��m�F����
    /// </summary>
    /// <returns> �N���A�t���O </returns>
    public static bool CheckStageStatus()
    {
        //���݂̃X�e�[�W�̏����擾
        var stage = Instance._stageStatusDic.FirstOrDefault(s => s.Key == Instance._currentStage);

        //�N���A�ς݂��ǂ�����Ԃ�
        return stage.Value;
    }

    /// <summary>
    /// �e�Q�[���̌��ʂ��Z�[�u����
    /// </summary>
    /// <param name="result"> �~�j�Q�[���̌��� </param>
    public static void SaveStageResult(bool result)
    {
        Instance._isClearStaged = result;
    }

    /// <summary>
    /// ���݂̃��r�[�̏�Ԃ�ύX����
    /// </summary>
    /// <param name="state"> �ݒ肷�郍�r�[�̏�� </param>
    public static void ChangeLobbyState(LobbyState state)
    {
        Instance._currentLobbyState = state;
    }

    /// <summary>
    /// �C�ӂ̃X�e�[�W�̃N���A�󋵂��m�F����
    /// </summary>
    /// <param name="stage"> �m�F����X�e�[�W </param>
    /// <returns> �N���A�t���O </returns>
    public static bool CheckStageStatus(Stages stage)
    {
        var s = Instance._stageStatusDic.First(s => s.Key == stage);

        return s.Value;
    }
    /// <summary>
    /// ���݂̎��v�̏󋵂��m�F����
    /// </summary>
    /// <returns> ���v�̃X�e�[�^�X </returns>
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
    /// ��Փx��ύX����
    /// </summary>
    /// <param name="type"> �ݒ肷���Փx </param>
    public static void ChangeGameDifficult(DifficultyType type)
    {
        Instance._currentGameDifficultyType = type;
    }

    /// <summary>
    /// �Q�[���̏�Ԃ����Z�b�g����
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

        Debug.Log("�f�[�^�����Z�b�g���܂���");
    }

    /// <summary>
    /// �X�e�[�W�N���A���̃X�`���l���̉��o���Đ�����
    /// </summary>
    /// <param name="stage"> �N���A�����X�e�[�W </param>
    /// <param name="type"> �Đ����郁�b�Z�[�W </param>
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
/// �X�e�[�W�̎��
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
/// ��Փx�̎��
/// </summary>
public enum DifficultyType
{
    Easy,
    Normal,
    Hard
}

/// <summary>
/// ���r�[�̏��
/// </summary>
public enum LobbyState
{
    Default,
    Under
}

