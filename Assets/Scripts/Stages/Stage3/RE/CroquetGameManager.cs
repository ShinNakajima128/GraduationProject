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

    [Tooltip("ステージにクリアに必要な回数")]
    [SerializeField]
    int _requiredSuccessCount = 3;

    [SerializeField]
    Transform[] _golaEffectTrans = default;

    [Header("Components")]
    [SerializeField]
    QueenOrder _order = default;

    [SerializeField]
    CroquetGameUI _gameUI = default;

    [SerializeField]
    CroquetCameraManager _cameraMng = default;

    [SerializeField]
    Stage3PlayerController _player = default;

    [SerializeField]
    CroquetTrumpManager _trumpMng = default;

    [Header("Debug")]
    [SerializeField]
    bool _debugMode = default;
    #endregion

    #region private
    TrumpColorType _currentTargetTrumpColor;
    /// <summary> 倒す目標数 </summary>
    int _currentTargetStrikeNum;
    bool _isGoaled = false;
    /// <summary> 倒した数 </summary>
    int _currentStrikeNum = 0;
    int _successCount = 0;
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
        AudioManager.PlayBGM(BGMType.Stage3);
        LetterboxController.ActivateLetterbox(true);
        base.Start();
        Init();
        OnGameStart();
    }

    public override void OnGameSetUp()
    {
        GameSetUp?.Invoke();
        _currentStrikeNum = 0;
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
        if (!_debugMode)
        {
            OrderData data = default;

            for (int i = 0; i < _playCount; i++)
            {
                //ゲームのセットアップ
                OnGameSetUp();

                if (i < 3)
                {
                    _trumpMng.SetTrumpSolder((AlignmentType)i);
                }
                else
                {
                    //トランプ兵の並び方をランダムにセット
                    var random = (AlignmentType)UnityEngine.Random.Range(0, 3);
                    _trumpMng.SetTrumpSolder(random);
                }

                //お題のデータを取得
                data = _order.Data[i];

                _currentTargetTrumpColor = data.TargetTrumpColor;
                _currentTargetStrikeNum = data.TargetNum;

                _gameUI.SetOrderText(i + 1, data.ToString());
                _gameUI.SetTrumpCount(_trumpMng.CurrentRedTrumpCount, _trumpMng.CurrentBlackTrumpCount);
                _gameUI.SetResultText("");

                _cameraMng.ChangeCamera(CroquetCameraType.View, 3.0f);

                yield return new WaitForSeconds(3.0f);

                //お題のアニメーションが終了するまで待機
                yield return _order.LetterAnimationCoroutine(i + 1, data.ToString());

                TransitionManager.FadeIn(FadeType.Normal, action: () =>
                {
                    _cameraMng.ChangeCamera(CroquetCameraType.InGame, 0f);
                    _gameUI.ChangeUIGroup(CroquetGameState.InGame);

                    LetterboxController.ActivateLetterbox(false, 0f);
                    TransitionManager.FadeOut(FadeType.Normal);
                });

                yield return new WaitForSeconds(3.0f);

                _player.BeginControl();

                yield return new WaitUntil(() => _player.IsThrowed);

                _cameraMng.ChangeCamera(CroquetCameraType.Strike, 1.0f);

                //メモ：ここにハリネズミがゴールするまで処理を待機する処理を記述
                yield return new WaitUntil(() => _isGoaled);

                OnGoalEffect();
                _isGoaled = false;

                //ゴールした時にシュートの結果に応じて結果を演出を変更
                yield return GoalDirectionCoroutine(_currentStrikeNum >= _currentTargetStrikeNum);

                if (i != _playCount - 1)
                {
                    TransitionManager.FadeIn(FadeType.Normal, action: () =>
                    {
                        _cameraMng.ChangeCamera(CroquetCameraType.Order, 0f);
                        _gameUI.ChangeUIGroup(CroquetGameState.Order);

                        LetterboxController.ActivateLetterbox(true, 0f);
                        TransitionManager.FadeOut(FadeType.Normal);
                    });

                    yield return new WaitForSeconds(3.5f);
                }
                else
                {
                    if (_successCount >= _requiredSuccessCount)
                    {
                        //_gameUI.ChangeUIGroup(CroquetGameState.Finish);
                        _gameUI.SetResultText("ステージクリア！");
                        GameManager.SaveStageResult(true);
                    }
                    else
                    {
                        _gameUI.SetResultText("ステージ失敗…");
                        GameManager.SaveStageResult(false);
                    }

                    yield return new WaitForSeconds(2.0f);
                    TransitionManager.SceneTransition(SceneType.Lobby);
                }
            }
        }
        else
        {
            _cameraMng.ChangeCamera(CroquetCameraType.InGame, 2.0f);
            _player.BeginControl();
        }
    }

    /// <summary>
    /// ハリネズミがゴール地点に着いた時のコルーチン
    /// </summary>
    /// <param name="result"> 打った結果 </param>
    /// <returns></returns>
    IEnumerator GoalDirectionCoroutine(bool result)
    {
        _cameraMng.ChangeCamera(CroquetCameraType.Goal);
        _gameUI.ChangeUIGroup(CroquetGameState.GoalDirection);

        yield return new WaitForSeconds(2.0f);

        if (result)
        {
            _gameUI.SetResultText("お題達成！");
        }
        else
        {
            _gameUI.SetResultText("お題失敗…");
        }
        yield return new WaitForSeconds(2.0f);
    }

    protected override void Init()
    {
        _player.GoalAction(() =>
        {
            _isGoaled = true;
        });
    }

    /// <summary>
    /// ターゲットの倒した数を加算する
    /// </summary>
    /// <param name="type"> 倒したトランプの種類 </param>
    public void AddScore(TrumpColorType type)
    {
        if (type == _currentTargetTrumpColor)
        {
            _currentStrikeNum++;
            Debug.Log("当たり");
        }

        _gameUI.SetTrumpCount(_trumpMng.CurrentRedTrumpCount, _trumpMng.CurrentBlackTrumpCount);
    }

    /// <summary>
    /// ゴールエフェクトを再生
    /// </summary>
    void OnGoalEffect()
    {
        for (int i = 0; i < _golaEffectTrans.Length; i++)
        {
            EffectManager.PlayEffect(EffectType.Stage3_Goal, _golaEffectTrans[i]);
        }
    }
}
/// <summary>
/// クロッケーゲームのステータス
/// </summary>
public enum CroquetGameState
{
    Order,
    InGame,
    GoalDirection,
    Finish
}
