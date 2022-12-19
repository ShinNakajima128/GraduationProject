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
    TrumpColorType _currentTargetTrumpColor;
    int _currentTargetStrikeNum;
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
        TransitionManager.FadeOut(FadeType.Normal);

        yield return new WaitForSeconds(1.0f);

        _cameraMng.ChangeCamera(CroquetCameraType.Order, 2.0f);

        yield return new WaitForSeconds(2.5f);

        StartCoroutine(InGameCoroutine());
    }

    protected override IEnumerator GameEndCoroutine(Action action = null)
    {
        throw new NotImplementedException();
    }
    
    /// <summary>
    /// ゲーム中のコルーチン
    /// </summary>
    IEnumerator InGameCoroutine()
    {
        OrderData data = default;

        for (int i = 0; i < _playCount; i++)
        {
            data = _order.Data[i];

            _currentTargetTrumpColor = data.TargetTrumpColor;
            _currentTargetStrikeNum = data.TargetNum;
            yield return _order.LetterAnimationCoroutine(i + 1, data.ToString());

            _cameraMng.ChangeCamera(CroquetCameraType.View, 3.0f);

            yield return new WaitForSeconds(3.0f);

            TransitionManager.FadeIn(FadeType.Normal, action: () =>
            {
                _cameraMng.ChangeCamera(CroquetCameraType.InGame, 0f);
                TransitionManager.FadeOut(FadeType.Normal);
            });

            yield return new WaitForSeconds(3.0f);


            //メモ：ここにハリネズミがゴールするまで処理を待機する処理を記述


            yield return new WaitUntil(() => UIInput.Submit);

            if (i != _playCount)
            {
                TransitionManager.FadeIn(FadeType.Normal, action: () =>
                {
                    _cameraMng.ChangeCamera(CroquetCameraType.Order, 0f);
                    TransitionManager.FadeOut(FadeType.Normal);
                });
                
                yield return new WaitForSeconds(3.5f);
            }
            else
            {

            }
        }
    }

    /// <summary>
    /// ハリネズミがゴール地点に着いた時のコルーチン
    /// </summary>
    /// <param name="result"> 打った結果 </param>
    /// <returns></returns>
    IEnumerator GoalDirectionCoroutine(bool result)
    {
        yield return null;
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
