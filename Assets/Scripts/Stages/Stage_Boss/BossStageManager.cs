using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    [Header("Component")]
    [SerializeField]
    MessagePlayer _messagePlayer = default;

    [SerializeField]
    BossController _bossCtrl = default;
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
    #endregion
    #region public
    public override event Action GameSetUp;
    public override event Action GameStart;
    public override event Action GamePause;
    public override event Action GameEnd;
    public event Action DirectionSetUp;
    #endregion
    #region property
    public static new BossStageManager Instance { get; private set; }
    #endregion

    protected override void Awake()
    {
        Instance = this;
        TryGetComponent(out _impulseSource);
        _playerTrans = GameObject.FindGameObjectWithTag("Player").transform; //プレイヤーのTransformを取得

        //バトル中かどうかの値が変わった時に行う処理を登録
        _isInBattle.Subscribe(_ => OnInGame?.Invoke(_isInBattle.Value));
    }

    protected override void Start()
    {
        base.Start();
        Init();
        OnGameStart();
    }

    public override void OnGameSetUp()
    {
        GameSetUp?.Invoke();
        _areaEffect.SetActive(true);
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

    protected override IEnumerator GameStartCoroutine(Action action = null)
    {
        yield return new WaitForSeconds(1.5f);

        //カメラをボスに寄せる
        CameraBlend(CameraType.Direction, _cameraBlendTime);

        yield return new WaitForSeconds(_cameraBlendTime);

        //ボス戦開始時の演出を再生
        StartCoroutine(_messagePlayer.PlayMessageCorountine(MessageType.Stage_Boss_Start, () =>
        {
            _isDirecting = true;
        }));

        yield return new WaitUntil(() => _isDirecting);

        _isDirecting = false;

        CameraBlend(CameraType.Battle, _cameraBlendTime);

        yield return new WaitForSeconds(_cameraBlendTime);

        StartCoroutine(InGameCoroutine());
    }

    protected override IEnumerator GameEndCoroutine(Action action = null)
    {
        throw new NotImplementedException();
    }

    protected override void Init()
    {
        _messagePlayer.Closeup += OnCloseup;
        _messagePlayer.Reset += OnReset;
        _isInBattle.Value = false;
    }

    IEnumerator InGameCoroutine()
    {
        _isInBattle.Value = true;

        for (int i = 0; i < _battleNum; i++)
        {
            CharacterMovable?.Invoke(true);
            OnGameSetUp();

            yield return _bossCtrl.BattlePhaseCoroutine((BossBattlePhase)i);

            Debug.Log("ボスが被弾。バトルフェイズを終了し、演出を開始");

            yield return DirectionCoroutine((BossBattlePhase)i);
        }

        //ボスを倒したあとの処理をここで実行し、エンディングSceneへ遷移する予定
        TransitionManager.SceneTransition(SceneType.Lobby);
    }

    IEnumerator DirectionCoroutine(BossBattlePhase phase)
    {
        CharacterMovable?.Invoke(false);
        OnDirectionSetUp();

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

        CameraBlend(CameraType.Battle, _cameraBlendTime);

        yield return new WaitForSeconds(_cameraBlendTime);
    }

    /// <summary>
    /// 各オブジェクトの再配置。仕切り直しを行う
    /// </summary>
    /// <returns></returns>
    IEnumerator PartitioningBattleCoroutine()
    {
        TransitionManager.FadeIn(FadeType.Normal, () => 
        {
            //ボスの位置と向きを初期化
            _bossCtrl.gameObject.transform.DOMove(_bossDirectionTrans.position, 0f);
            _bossCtrl.gameObject.transform.DOLocalRotate(new Vector3(0f, 180f, 0f), 0f);
            _bossCtrl.PlayBossAnimation(BossAnimationType.Idle, 0.1f);

            //プレイヤーの位置と向きを初期化
            _playerTrans.DOLocalMove(_playerStartTrans.position, 0f);
            _playerTrans.DOLocalRotate(Vector3.zero, 0f);

            TransitionManager.FadeOut(FadeType.Normal);
        });

        yield return new WaitForSeconds(3.5f); //画面のフェード演出終了まで待機

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
                break;
            case CameraType.Direction:
                _directionCamera.Priority = 15;
                _closeupCamera.Priority = 9;
                _battleCamera.Priority = 9;
                _jumpAttackCamera.Priority = 9;
                break;
            case CameraType.Battle:
                _directionCamera.Priority = 9;
                _closeupCamera.Priority = 9;
                _battleCamera.Priority = 15;
                _jumpAttackCamera.Priority = 9;
                break;
            case CameraType.Direction_Closeup:
                _directionCamera.Priority = 9;
                _closeupCamera.Priority = 15;
                _battleCamera.Priority = 9;
                _jumpAttackCamera.Priority = 9;
                break;
            case CameraType.JumpAttack:
                _directionCamera.Priority = 9;
                _closeupCamera.Priority = 9;
                _battleCamera.Priority = 9;
                _jumpAttackCamera.Priority = 15;
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
}
public enum CameraType
{
    Default,
    Direction,
    Battle,
    Direction_Closeup,
    JumpAttack
}
