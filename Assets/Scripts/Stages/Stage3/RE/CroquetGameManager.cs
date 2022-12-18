using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// クロッケーゲーム全体を管理するマネージャークラス
/// </summary>
public class CroquetGameManager : StageGame<CroquetGameManager>
{
    #region serialize
    [Header("Variables")]
    [Tooltip("プレイする回数")]
    [SerializeField]
    int _playCount = 3;

    [Header("Components")]
    [SerializeField]
    QueenOrder _order = default;

    [SerializeField]
    CroquetGameUI _gameUI = default;

    [SerializeField]
    CroquetCameraManager _cameraMng = default;
    #endregion

    #region private
    #endregion
    
    #region public
    public override event Action GameSetUp;
    public override event Action GameStart;
    public override event Action GamePause;
    public override event Action GameEnd;
    #endregion
    
    #region property
    public static new CroquetGameManager Instance { get; private set; }
    #endregion

    protected override void Awake()
    {
        Instance = this;
    }

    protected override void Start()
    {
        base.Start();
        OnGameStart();
    }

    public override void OnGameSetUp()
    {
        GameSetUp?.Invoke();
    }

    public override void OnGameStart()
    {
        GameStart?.Invoke();
        StartCoroutine(GameStartCoroutine());
    }

    public override void OnGameEnd()
    {
        GameEnd?.Invoke();
    }

    protected override IEnumerator GameStartCoroutine(Action action = null)
    {
        yield return null;
    }

    protected override IEnumerator GameEndCoroutine(Action action = null)
    {
        throw new NotImplementedException();
    }

    protected override void Init()
    {
        throw new NotImplementedException();
    }
}
/// <summary>
/// クロッケーゲームのステータス
/// </summary>
public enum CroquetGameState
{
    Order,
    InGame,
    GoalDirection
}
