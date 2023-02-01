using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using AliceProject;
using UniRx;
using DG.Tweening;

/// <summary>
/// ボスステージの機能を管理するマネージャー
/// </summary>
public class BossStageManager : StageGame<BossStageManager>
{
    #region serialize
    [Header("Variables")]
    [Tooltip("戦闘回数")]
    [SerializeField]
    int _battleNum = 3;

    [SerializeField]
    BossBattleParameter[] _battleParameters = default;

    [Tooltip("別のカメラへ遷移する際にかける時間")]
    [SerializeField]
    float _cameraBlendTime = 2.0f;

    [SerializeField]
    float _closeupTime = 0.25f;

    [Header("Cameras")]
    [Tooltip("Cinemachineを管理するBrain")]
    [SerializeField]
    CinemachineBrain _cameraBrain = default;

    [Tooltip("女王に注目するカメラ")]
    [SerializeField]
    CinemachineVirtualCamera _directionCamera = default;

    [Tooltip("女王に更にズームインするカメラ")]
    [SerializeField]
    CinemachineVirtualCamera _closeupCamera = default;

    [Tooltip("戦闘中の見下ろしカメラ")]
    [SerializeField]
    CinemachineVirtualCamera _battleCamera = default;

    [Tooltip("ボスのジャンプ中に使用するカメラ")]
    [SerializeField]
    CinemachineVirtualCamera _jumpAttackCamera = default;

    [Tooltip("トランプ兵を処刑する演出時のカメラ")]
    [SerializeField]
    CinemachineVirtualCamera _excuteCamera = default;

    [Tooltip("ボスを倒した時のカメラ")]
    [SerializeField]
    CinemachineVirtualCamera _finishCamera = default;

    [Tooltip("演出有りの時の最初のカメラ")]
    [SerializeField]
    CinemachineVirtualCamera _directionStartCamera = default;

    [SerializeField]
    DirectionCameraManager _directionCameraMng = default;

    [Header("Objects")]
    [Tooltip("女王の演出位置")]
    [SerializeField]
    Transform _bossDirectionTrans = default;

    [Tooltip("プレイヤーの初期位置")]
    [SerializeField]
    Transform _playerStartTrans = default;

    [Tooltip("戦闘中の移動可能範囲のエフェクト")]
    [SerializeField]
    GameObject _areaEffect = default;

    [Header("UI")]
    [SerializeField]
    CanvasGroup _hpPanel = default;

    [Tooltip("戦闘終了時の画面")]
    [SerializeField]
    CanvasGroup _finishBattleCanvas = default;

    [SerializeField]
    CanvasGroup _queenNameGroup = default;

    [SerializeField]
    Image[] _infoImages = default;

    [Header("Component")]
    [SerializeField]
    MessagePlayer _messagePlayer = default;

    [SerializeField]
    GameObject _directionBossObject = default;

    [SerializeField]
    BossController _bossCtrl = default;

    [SerializeField]
    CheshireCat _cheshireCat = default;

    [Tooltip("トランプ兵の処刑の演出を行うComponent")]
    [SerializeField]
    ExcuteDirection _excuteDirection = default;

    [SerializeField]
    TrumpSolderManager _trumpSolderMng = default;

    [Tooltip("がれきのGenerator")]
    [SerializeField]
    DebrisGenerator _debrisGenerator = default;

    [Header("Debug")]
    [SerializeField]
    bool _debugMode = false;
    #endregion

    #region public
    public event Action<bool> OnInGame;
    #endregion

    #region private
    Transform _playerTrans;
    /// <summary> 演出中かどうか </summary>
    bool _isDirecting = false;
    /// <summary> 戦闘中かどうか </summary>
    ReactiveProperty<bool> _isInBattle = new ReactiveProperty<bool>();
    CinemachineImpulseSource _impulseSource;
    Coroutine _currentGameCoroutine;
    #endregion

    #region public
    public override event Action GameSetUp;
    public override event Action GameStart;
    public override event Action GamePause;
    public override event Action GameEnd;
    public event Action DirectionSetUp;
    public event Action GameOver;
    #endregion
    #region property
    public static new BossStageManager Instance { get; private set; }
    public static bool IsFirstVisit { get; set; } = true;
    #endregion

    protected override void Awake()
    {
        Instance = this;
        TryGetComponent(out _impulseSource);
        _playerTrans = GameObject.FindGameObjectWithTag("Player").transform; //プレイヤーのTransformを取得

        //バトル中かどうかの値が変わった時に行う処理を登録
        _isInBattle.Subscribe(_ => OnInGame?.Invoke(_isInBattle.Value)).AddTo(this);
    }

    protected override void Start()
    {
        base.Start();
        Init();
        SubscribeEvents();
        OnGameStart();
    }

    public override void OnGameSetUp()
    {
        GameSetUp?.Invoke();

        //現在の難易度のパラメーターを取得
        var param = _battleParameters.FirstOrDefault(p => p.DifficultyType == GameManager.Instance.CurrentGameDifficultyType);

        //パラメーターを反映
        _bossCtrl.SetParameter(param.PhaseParameters);
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

    public void OnDirectionSetUp()
    {
        DirectionSetUp?.Invoke();
    }

    /// <summary>
    /// カメラを切り替える
    /// </summary>
    /// <param name="type"> 切り替え先のカメラの種類 </param>
    /// <param name="blendTime"> 切り替えにかける時間 </param>
    public static void CameraChange(CameraType type, float blendTime)
    {
        Instance.CameraBlend(type, blendTime);
    }

    /// <summary>
    /// カメラを揺らす
    /// </summary>
    public static void CameraShake()
    {
        Instance._impulseSource.GenerateImpulse();
    }

    /// <summary>
    /// ゲーム開始時の演出のコルーチン
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    protected override IEnumerator GameStartCoroutine(Action action = null)
    {
        yield return null;
        yield return null;

        AudioManager.PlayBGM(BGMType.Boss_Before);

        if (!_debugMode && IsFirstVisit)
        {
            _directionBossObject.SetActive(true);
            _bossCtrl.gameObject.SetActive(false);

            _directionStartCamera.Priority = 30;

            yield return _messagePlayer.PlayMessageCorountine(MessageType.Stage_Boss_Start1);

            yield return _directionCameraMng.StartDirectionCoroutine(CameraDirectionType.BossStage_HeadingBossFeet);

            _directionStartCamera.Priority = 0;

            yield return _messagePlayer.PlayMessageCorountine(MessageType.Stage_Boss_Start2);

            yield return _directionCameraMng.StartDirectionCoroutine(CameraDirectionType.BossStage_SlowlyRise);

            yield return _directionCameraMng.StartDirectionCoroutine(CameraDirectionType.BossStage_OnBossFace);

            AudioManager.PlaySE(SEType.BossStage_QueenAppearance);
            OnFadeDescription(1, 0.25f);
            yield return new WaitForSeconds(2.5f);
            
            OnFadeDescription(0, 0.25f);

            yield return new WaitForSeconds(2.0f);

            yield return _directionCameraMng.StartDirectionCoroutine(CameraDirectionType.BossStage_BehindBoss);
            yield return _messagePlayer.PlayMessageCorountine(MessageType.Stage_Boss_Start3);

            EventManager.OnEvent(Events.BossStage_QueenAnger);
            yield return new WaitForSeconds(2f);

            yield return _messagePlayer.PlayMessageCorountine(MessageType.Stage_Boss_Start4, () =>
            {
                _isDirecting = true;
            });
            
            yield return new WaitUntil(() => _isDirecting);

            _isDirecting = false;
        }

        EventManager.OnEvent(Events.BossStage_DisolveCheshire);
        _directionBossObject.SetActive(false);
        _bossCtrl.gameObject.SetActive(true);

        yield return new WaitForSeconds(2.0f);

        TransitionManager.FadeIn(FadeType.Black_default,
                         action: () =>
                         {
                             _directionCameraMng.ResetCamera();
                             TransitionManager.FadeOut(FadeType.Black_default);
                         });

        yield return new WaitForSeconds(1.5f);

        //セリフの再生終了時にボスのモーションをリセット、カメラを戦闘用に変更
        _bossCtrl.PlayBossAnimation(BossAnimationType.Idle, 0.3f);
        LetterboxController.ActivateLetterbox(true);
        CameraBlend(CameraType.Default, 0);
        AudioManager.StopBGM(_cameraBlendTime / 2);

        yield return new WaitForSeconds(_cameraBlendTime);

        action?.Invoke();
        //インゲームの処理を開始
        _currentGameCoroutine = StartCoroutine(InGameCoroutine());
    }

    protected override IEnumerator GameEndCoroutine(Action action = null)
    {
        throw new NotImplementedException();
    }

    protected override void Init()
    {
        _messagePlayer.Closeup += OnCloseup;
        HPManager.Instance.LostHpAction += OnBossStageGameOver;
        //_messagePlayer.Reset += OnReset;
        _isInBattle.Value = false;
        _hpPanel.alpha = 0;
        _infoImages[0].enabled = false;
        _infoImages[1].enabled = false;
    }

    void SubscribeEvents()
    {
        EventManager.ListenEvents(Events.BossStage_FrontAlice, () => StartCoroutine(_directionCameraMng.StartDirectionCoroutine(CameraDirectionType.BossStage_FrontAlice)));
        EventManager.ListenEvents(Events.BossStage_HeadingBossFeet, () => StartCoroutine(_directionCameraMng.StartDirectionCoroutine(CameraDirectionType.BossStage_HeadingBossFeet)));
        EventManager.ListenEvents(Events.BossStage_SlowlyRise, () => StartCoroutine(_directionCameraMng.StartDirectionCoroutine(CameraDirectionType.BossStage_SlowlyRise)));
        EventManager.ListenEvents(Events.BossStage_OnBossFace, () => 
        { 
            StartCoroutine(_directionCameraMng.StartDirectionCoroutine(CameraDirectionType.BossStage_OnBossFace));
        });
        EventManager.ListenEvents(Events.BossStage_BehindBoss, () => StartCoroutine(_directionCameraMng.StartDirectionCoroutine(CameraDirectionType.BossStage_BehindBoss, 31)));
        EventManager.ListenEvents(Events.BossStage_BehindBoss_RE2, () => StartCoroutine(_directionCameraMng.StartDirectionCoroutine(CameraDirectionType.BossStage_BehindBoss, 33)));
        EventManager.ListenEvents(Events.BossStage_BehindAlice, () => StartCoroutine(_directionCameraMng.StartDirectionCoroutine(CameraDirectionType.BossStage_BehindAlice, 32)));
        EventManager.ListenEvents(Events.BossStage_FrontBoss, () => StartCoroutine(_directionCameraMng.StartDirectionCoroutine(CameraDirectionType.BossStage_FrontBoss, 34)));
        EventManager.ListenEvents(Events.BossStage_ZoomBossFace, () => StartCoroutine(_directionCameraMng.StartDirectionCoroutine(CameraDirectionType.BossStage_ZoomBossFace, 35)));
        EventManager.ListenEvents(Events.BossStage_FrontCheshire, () => StartCoroutine(_directionCameraMng.StartDirectionCoroutine(CameraDirectionType.BossStage_FrontCheshireCat, 36)));
        EventManager.ListenEvents(Events.BossStage_DisolveCheshire, () => _cheshireCat.ActivateDissolve(true));
    }

    void Gameover()
    {
        StopCoroutine(_currentGameCoroutine);
        _currentGameCoroutine = null;
    }

    IEnumerator InGameCoroutine()
    {
        _isInBattle.Value = true;
        AudioManager.PlayBGM(BGMType.Boss_InGame);

        for (int i = 0; i < _battleNum; i++)
        {
            OnGameSetUp();

            if (i == 0)
            {
                _infoImages[0].enabled = true;
                yield return new WaitForSeconds(2.0f);
                _infoImages[0].enabled = false;
            }

            //バトルフェイズを終了するまで待機
            yield return _bossCtrl.BattlePhaseCoroutine((BossBattlePhase)i,
                                  firstAction: () =>
                                  {
                                      CameraBlend(CameraType.Battle, _cameraBlendTime);
                                      _areaEffect.transform.DOLocalMoveY(0f, 1.0f);
                                  },
                                  phaseStartAction: () =>
                                  {
                                      CharacterMovable?.Invoke(true);
                                      _hpPanel.alpha = 1;

                                      if (i > 0)
                                      {
                                          _debrisGenerator.StartGenerate();
                                      }
                                  });

            Debug.Log("ボスが被弾。バトルフェイズを終了し、演出を開始");

            _areaEffect.transform.DOLocalMoveY(-3.5f, 1.0f);
            _trumpSolderMng.OnAllTrumpAnimation("Shaking_Start");

            if (_debrisGenerator.IsGenerating)
            {
                _debrisGenerator.StopGenerate();
            }

            //現在のフェイズに合わせた演出の処理を開始
            yield return DirectionCoroutine((BossBattlePhase)i);
        }

        yield return GameManager.GetStillDirectionCoroutine(Stages.Stage_Boss, MessageType.GetStill_Stage_Boss);

        //ボスを倒したあとの処理をここで実行し、エンディングSceneへ遷移する予定
        TransitionManager.FadeIn(FadeType.Black_TransParent, 0f);
        TransitionManager.SceneTransition(SceneType.Ending);
    }

    IEnumerator DirectionCoroutine(BossBattlePhase phase)
    {
        yield return new WaitForSeconds(1.0f);

        CharacterMovable?.Invoke(false);
        OnDirectionSetUp();

        if (_debugMode)
        {
            phase = BossBattlePhase.Third;
        }

        //最後のフェイズの場合は戦闘終了の演出を行う
        if (phase == BossBattlePhase.Third)
        {
            yield return FinishBattleCoroutine();

            _hpPanel.alpha = 0;
            yield break;
        }

        yield return new WaitForSeconds(2.5f);

        _hpPanel.alpha = 0;
        HPManager.Instance.RecoveryHP();
        _bossCtrl.PlayBossAnimation(BossAnimationType.Jump);
        _bossCtrl.transform.DOLookAt(new Vector3(_bossDirectionTrans.position.x, 0f, _bossDirectionTrans.position.z), 0.5f);

        yield return new WaitForSeconds(1.3f);

        yield return _bossCtrl.gameObject.transform.DOMove(_bossDirectionTrans.position, 1.5f)
                                                   .SetEase(Ease.OutQuint)
                                                   .OnComplete(() =>
                                                   {
                                                       _bossCtrl.gameObject.transform.DOLocalRotate(new Vector3(0f, 180f, 0f), 0.5f);
                                                   }).WaitForCompletion();

        CameraBlend(CameraType.Direction, 2.0f);

        yield return new WaitForSeconds(2.0f);

        _bossCtrl.PlayBossAnimation(BossAnimationType.Angry, 0.1f);

        MessageType message = default;

        //現在のフェイズに合わせたテキストデータの種類を設定
        switch (phase)
        {
            case BossBattlePhase.First:
                message = MessageType.Stage_Boss_Down1;
                break;
            case BossBattlePhase.Second:
                message = MessageType.Stage_Boss_Down2;
                break;
            case BossBattlePhase.Third:
                message = MessageType.Stage_Boss_Down3;
                break;
        }

        yield return _messagePlayer.PlayMessageCorountine(message);

        yield return PartitioningBattleCoroutine();
    }

    /// <summary>
    /// 各オブジェクトの再配置。仕切り直しを行う
    /// </summary>
    /// <returns></returns>
    IEnumerator PartitioningBattleCoroutine()
    {
        TransitionManager.FadeIn(FadeType.Normal, 0.2f, action: () =>
          {
             //プレイヤーの位置と向きを初期化
             _playerTrans.DOLocalMove(_playerStartTrans.position, 0f);
              _playerTrans.DOLocalRotate(Vector3.zero, 0f);
              _trumpSolderMng.OnAllTrumpAnimation("Idle");

              CameraBlend(CameraType.ExcuteTrump, 0.01f);
              TransitionManager.FadeOut(FadeType.Normal, 0.2f);
          });
        ////プレイヤーの位置と向きを初期化


        //トランプ兵の首を飛ばす演出の処理
        yield return _excuteDirection.ExcuteDirectionCoroutine();

        CameraBlend(CameraType.Default, 0.02f);
        _bossCtrl.PlayBossAnimation(BossAnimationType.Idle);
        yield return new WaitForSeconds(2.0f);
    }

    IEnumerator FinishBattleCoroutine()
    {
        CameraBlend(CameraType.FinishBattle, 2.0f);

        yield return new WaitForSeconds(4.0f);

        _infoImages[1].enabled = true;
        AudioManager.PlayBGM(BGMType.BossStage_Clear, false);
        //_finishBattleCanvas.alpha = 1;

        yield return new WaitForSeconds(12.0f);

        _infoImages[1].enabled = false;
    }

    /// <summary>
    /// 現在のカメラから別のカメラへ遷移する
    /// </summary>
    /// <param name="type"> カメラの種類 </param>
    /// <param name="blendTime"> 遷移にかける時間 </param>
    void CameraBlend(CameraType type, float blendTime)
    {
        _cameraBrain.m_DefaultBlend.m_Time = blendTime;

        switch (type)
        {
            case CameraType.Default:
                _directionCamera.Priority = 9;
                _closeupCamera.Priority = 9;
                _battleCamera.Priority = 9;
                _jumpAttackCamera.Priority = 9;
                _finishCamera.Priority = 9;
                _excuteCamera.Priority = 9;
                break;
            case CameraType.Direction:
                _directionCamera.Priority = 15;
                _closeupCamera.Priority = 9;
                _battleCamera.Priority = 9;
                _jumpAttackCamera.Priority = 9;
                _finishCamera.Priority = 9;
                _excuteCamera.Priority = 9;
                break;
            case CameraType.Battle:
                _directionCamera.Priority = 9;
                _closeupCamera.Priority = 9;
                _battleCamera.Priority = 15;
                _jumpAttackCamera.Priority = 9;
                _finishCamera.Priority = 9;
                _excuteCamera.Priority = 9;
                break;
            case CameraType.Direction_Closeup:
                _directionCamera.Priority = 9;
                _closeupCamera.Priority = 15;
                _battleCamera.Priority = 9;
                _jumpAttackCamera.Priority = 9;
                _finishCamera.Priority = 9;
                _excuteCamera.Priority = 9;
                break;
            case CameraType.JumpAttack:
                _directionCamera.Priority = 9;
                _closeupCamera.Priority = 9;
                _battleCamera.Priority = 9;
                _jumpAttackCamera.Priority = 15;
                _finishCamera.Priority = 9;
                _excuteCamera.Priority = 9;
                break;
            case CameraType.FinishBattle:
                _directionCamera.Priority = 9;
                _closeupCamera.Priority = 9;
                _battleCamera.Priority = 9;
                _jumpAttackCamera.Priority = 9;
                _finishCamera.Priority = 15;
                _excuteCamera.Priority = 9;
                break;
            case CameraType.ExcuteTrump:
                _directionCamera.Priority = 9;
                _closeupCamera.Priority = 9;
                _battleCamera.Priority = 9;
                _jumpAttackCamera.Priority = 9;
                _finishCamera.Priority = 9;
                _excuteCamera.Priority = 15;
                break;
            default:
                break;
        }
    }

    void OnCloseup()
    {
        CameraBlend(CameraType.Direction_Closeup, _closeupTime);
    }

    void OnReset()
    {
        CameraBlend(CameraType.Direction_Closeup, _cameraBlendTime);
    }

    void OnFadeDescription(float value, float fadeTime)
    {
        DOTween.To(() => _queenNameGroup.alpha,
                x => _queenNameGroup.alpha = x,
                value,
                fadeTime);
    }
    /// <summary>
    /// ゲームオーバー演出を再生する
    /// </summary>
    void OnBossStageGameOver()
    {
        StartCoroutine(GameOverCoroutine());
    }

    IEnumerator GameOverCoroutine()
    {
        yield return null;

        GameOver?.Invoke();

        Gameover();

        TransitionManager.FadeIn(FadeType.Black_default, fadeTime: 2.0f, action: () =>
         {
             GameoverDirection.Instance.ActivateGameoverUI(true);
             TransitionManager.FadeOut(FadeType.Black_default, fadeTime: 2.0f);
             IsFirstVisit = false;
         });
    }
}
public enum CameraType
{
    Default,
    Direction,
    Battle,
    Direction_Closeup,
    JumpAttack,
    FinishBattle,
    ExcuteTrump
}

/// <summary>
/// ボス戦のパラメーター
/// </summary>
[Serializable]
public struct BossBattleParameter
{
    public string PramName;
    public DifficultyType DifficultyType;
    public PhaseParameter[] PhaseParameters;
}
