using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using AliceProject;

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

    [Tooltip("難易度毎のゲームの数値")]
    [SerializeField]
    CroquetGameParameter[] _gameParameters = default;

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

    [SerializeField]
    Transform _testModel = default;

    [Header("Debug")]
    [SerializeField]
    bool _debugMode = default;
    #endregion

    #region private
    TrumpColorType _currentTargetTrumpColor;
    /// <summary> 倒す目標数 </summary>
    int _currentTargetStrikeNum;
    int _currentRedStrileNum = 0;
    int _currentBlackStrikeNum = 0;
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
        _currentRedStrileNum = 0;
        _currentBlackStrikeNum = 0;

        var param = _gameParameters.FirstOrDefault(p => p.DifficultyType == GameManager.Instance.CurrentGameDifficultyType);

        _order.SetOrderData(param.OrderDatas);
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
                _gameUI.SetTrumpCount(_currentRedStrileNum, _currentBlackStrikeNum);
                _gameUI.SetResultText("");

                _cameraMng.ChangeCamera(CroquetCameraType.View, 3.0f);

                yield return new WaitForSeconds(3.0f);

                //お題のアニメーションが終了するまで待機
                yield return _order.LetterAnimationCoroutine(i + 1, data.ToString());

                _cameraMng.ChangeCamera(CroquetCameraType.InGame);
                LetterboxController.ActivateLetterbox(false, 1.5f);

                yield return new WaitForSeconds(1.5f);

                _gameUI.ChangeUIGroup(CroquetGameState.InGame);

                //初めてステージ3をプレイしている場合
                if (GameManager.Instance.IsFirstVisitCurrentStage)
                {
                    //フェイズ毎の会話パートを再生
                    switch (i)
                    {
                        case 0:
                            yield return MessagePlayer.Instance.PlayMessageCorountine(MessageType.FirstVisit_Stage3_Phase1);
                            break;
                        case 1:
                            yield return MessagePlayer.Instance.PlayMessageCorountine(MessageType.FirstVisit_Stage3_Phase2);
                            break;
                        case 2:
                            yield return MessagePlayer.Instance.PlayMessageCorountine(MessageType.FirstVisit_Stage3_Phase3);
                            break;
                        default:
                            break;
                    }
                }

                _player.BeginControl(); //入力受付開始

                yield return new WaitUntil(() => _player.IsThrowed);

                _cameraMng.ChangeCamera(CroquetCameraType.Strike, 1.0f);

                //メモ：ここにハリネズミがゴールするまで処理を待機する処理を記述
                yield return new WaitUntil(() => _isGoaled);

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

                    yield return new WaitForSeconds(2.0f);
                }
                else
                {
                    if (_successCount >= _requiredSuccessCount)
                    {
                        //_gameUI.ChangeUIGroup(CroquetGameState.Finish);
                        _gameUI.SetResultText("ステージクリア！");
                        GameManager.SaveStageResult(true);
                        yield return new WaitForSeconds(2.0f);
                        _gameUI.SetResultText("");
                        yield return GameManager.GetStillDirectionCoroutine(Stages.Stage3, MessageType.GetStill_Stage3);
                    }
                    else
                    {
                        _gameUI.SetResultText("ステージ失敗…");
                        yield return new WaitForSeconds(2.0f);
                        GameManager.SaveStageResult(false);
                    }
                    TransitionManager.FadeIn(FadeType.Black_TransParent, 0f);
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
        yield return new WaitForSeconds(2.0f);
        _gameUI.ChangeUIGroup(CroquetGameState.GoalDirection);
     
        if (result)
        {
            _successCount++;
            OnGoalEffect();
        }

        _gameUI.OnResultUI(result);
        yield return new WaitUntil(() => UIInput.Submit);

        _gameUI.OffResultUI();
    }

    protected override void Init()
    {
        _player.GoalAction(() =>
        {
            _isGoaled = true;
            _testModel.DOShakeScale(0.5f,strength: 1f, vibrato:10);
        });

        _player.CheckPointAction(() =>
        {
            _cameraMng.ChangeCamera(CroquetCameraType.Goal, 1.5f);
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
        }

        switch (type)
        {
            case TrumpColorType.Red:
                _currentRedStrileNum++;
                break;
            case TrumpColorType.Black:
                _currentBlackStrikeNum++;
                break;
            default:
                break;
        }

        _gameUI.SetTrumpCount(_currentRedStrileNum, _currentBlackStrikeNum);
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

/// <summary>
/// クロッケーゲームの難易度毎のパラメーター
/// </summary>
[Serializable]
public struct CroquetGameParameter
{
    public string ParamName;
    public DifficultyType DifficultyType;
    public OrderData[] OrderDatas;
}
