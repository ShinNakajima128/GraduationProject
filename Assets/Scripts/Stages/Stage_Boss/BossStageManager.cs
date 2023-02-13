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
/// �{�X�X�e�[�W�̋@�\���Ǘ�����}�l�[�W���[
/// </summary>
public class BossStageManager : StageGame<BossStageManager>
{
    #region serialize
    [Header("Variables")]
    [Tooltip("�퓬��")]
    [SerializeField]
    int _battleNum = 3;

    [SerializeField]
    BossBattleParameter[] _battleParameters = default;

    [Tooltip("�ʂ̃J�����֑J�ڂ���ۂɂ����鎞��")]
    [SerializeField]
    float _cameraBlendTime = 2.0f;

    [SerializeField]
    float _closeupTime = 0.25f;

    [Header("Cameras")]
    [Tooltip("Cinemachine���Ǘ�����Brain")]
    [SerializeField]
    CinemachineBrain _cameraBrain = default;

    [Tooltip("�����ɒ��ڂ���J����")]
    [SerializeField]
    CinemachineVirtualCamera _directionCamera = default;

    [Tooltip("�����ɍX�ɃY�[���C������J����")]
    [SerializeField]
    CinemachineVirtualCamera _closeupCamera = default;

    [Tooltip("�퓬���̌����낵�J����")]
    [SerializeField]
    CinemachineVirtualCamera _battleCamera = default;

    [Tooltip("�{�X�̃W�����v���Ɏg�p����J����")]
    [SerializeField]
    CinemachineVirtualCamera _jumpAttackCamera = default;

    [Tooltip("�g�����v�������Y���鉉�o���̃J����")]
    [SerializeField]
    CinemachineVirtualCamera _excuteCamera = default;

    [Tooltip("�{�X��|�������̃J����")]
    [SerializeField]
    CinemachineVirtualCamera _finishCamera = default;

    [Tooltip("���o�L��̎��̍ŏ��̃J����")]
    [SerializeField]
    CinemachineVirtualCamera _directionStartCamera = default;

    [SerializeField]
    DirectionCameraManager _directionCameraMng = default;

    [Header("Objects")]
    [Tooltip("�����̉��o�ʒu")]
    [SerializeField]
    Transform _bossDirectionTrans = default;

    [Tooltip("�v���C���[�̏����ʒu")]
    [SerializeField]
    Transform _playerStartTrans = default;

    [Tooltip("�퓬���̈ړ��\�͈͂̃G�t�F�N�g")]
    [SerializeField]
    GameObject _areaEffect = default;

    [Tooltip("�J�n���A�C���Q�[�����̃I�u�W�F�N�g�̐e�I�u�W�F�N�g")]
    [SerializeField]
    GameObject _inGameObjectsParent = default;

    [Tooltip("�퓬�I����ɉ��o���s���I�u�W�F�N�g�̐e�I�u�W�F�N�g")]
    [SerializeField]
    GameObject _endDirectionObjectsParent = default;

    [Header("UI")]
    [SerializeField]
    CanvasGroup _hpPanel = default;

    [Tooltip("�퓬�I�����̉��")]
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

    [Tooltip("�g�����v���̏��Y�̉��o���s��Component")]
    [SerializeField]
    ExcuteDirection _excuteDirection = default;

    [SerializeField]
    TrumpSolderManager _trumpSolderMng = default;

    [Tooltip("���ꂫ��Generator")]
    [SerializeField]
    DebrisGenerator _debrisGenerator = default;

    [Tooltip("����Generator")]
    [SerializeField]
    FallPoleGenerator _fallPoleGenerator = default;

    [Header("Debug")]
    [Tooltip("�ŏ��̉��o���X�L�b�v�A3�t�F�C�Y�ڂ���X�^�[�g")]
    [SerializeField]
    bool _debugMode = false;
    
    [Tooltip("�퓬�I����V�[���m�F�p")]
    [SerializeField]
    bool _playOnEnding = false;
    #endregion

    #region public
    public event Action<bool> OnInGame;
    #endregion

    #region private
    Transform _playerTrans;
    /// <summary> ���o�����ǂ��� </summary>
    bool _isDirecting = false;
    /// <summary> �퓬�����ǂ��� </summary>
    ReactiveProperty<bool> _isInBattle = new ReactiveProperty<bool>();
    CinemachineImpulseSource _impulseSource;
    Coroutine _startDirectionCoroutine;
    Coroutine _currentGameCoroutine;
    Coroutine _endDirectionCoroutine;
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
    public bool IsInBattle => _isInBattle.Value;
    public bool IsDirecting => _isDirecting;
    #endregion

    protected override void Awake()
    {
        Instance = this;
        TryGetComponent(out _impulseSource);
        _playerTrans = GameObject.FindGameObjectWithTag("Player").transform; //�v���C���[��Transform���擾

        //�o�g�������ǂ����̒l���ς�������ɍs��������o�^
        _isInBattle.Subscribe(value => OnInGame?.Invoke(value))
                   .AddTo(this);
    }

    protected override void Start()
    {
        base.Start();
        Init();
        SubscribeStartEvents();
        OnGameStart();
    }

    public override void OnGameSetUp()
    {
        GameSetUp?.Invoke();

        //���݂̓�Փx�̃p�����[�^�[���擾
        var param = _battleParameters.FirstOrDefault(p => p.DifficultyType == GameManager.Instance.CurrentGameDifficultyType);

        //�p�����[�^�[�𔽉f
        _bossCtrl.SetParameter(param.PhaseParameters);
    }

    public override void OnGameStart()
    {
        GameStart?.Invoke();
        _startDirectionCoroutine = StartCoroutine(GameStartCoroutine());
        SkipButton.Instance.Isrespond += () => IsDirecting;
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
    /// �J������؂�ւ���
    /// </summary>
    /// <param name="type"> �؂�ւ���̃J�����̎�� </param>
    /// <param name="blendTime"> �؂�ւ��ɂ����鎞�� </param>
    public static void CameraChange(CameraType type, float blendTime)
    {
        Instance.CameraBlend(type, blendTime);
    }

    /// <summary>
    /// �J������h�炷
    /// </summary>
    public static void CameraShake()
    {
        Instance._impulseSource.GenerateImpulse();
    }

    /// <summary>
    /// �Q�[���J�n���̉��o�̃R���[�`��
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    protected override IEnumerator GameStartCoroutine(Action action = null)
    {
        yield return null;
        yield return null;

        if (_playOnEnding)
        {
            //�����F�J�������o�g���I�����o�ɐ؂�ւ��鏈���������֋L�q
            _isDirecting = true;

            _trumpSolderMng.OnAllTrumpActivate(true);
            _trumpSolderMng.OnAllTrumpAnimation("Idle");
            SubscribeEndEvents();
            _inGameObjectsParent.SetActive(false);
            _endDirectionObjectsParent.SetActive(true);
            AudioManager.PlayBGM(BGMType.EndBossBattle);

            yield return new WaitForSeconds(0.5f);

            _endDirectionCoroutine = StartCoroutine(_messagePlayer.PlayMessageCorountine(MessageType.Stage_Boss_End1));
            yield return _endDirectionCoroutine;

            yield return GameManager.GetStillDirectionCoroutine(Stages.Stage_Boss, MessageType.GetStill_Stage_Boss, 3.0f);

            //�G���f�B���OScene�֑J�ڂ���
            TransitionManager.FadeIn(FadeType.Black_Transparent, 0f);
            TransitionManager.SceneTransition(SceneType.Ending);
            _isDirecting = false;
            yield break;
        }

        AudioManager.PlayBGM(BGMType.Boss_Before);

        if (!_debugMode && IsFirstVisit)
        {
            _isDirecting = true;

            SkipButton.Instance.OnSkip.Subscribe(_ =>
            {
                if (_startDirectionCoroutine != null)
                {
                    StopCoroutine(_startDirectionCoroutine);
                    _startDirectionCoroutine = null;
                }
                    StartCoroutine(SkipCorourine());
            });

            _directionBossObject.SetActive(true);
            _bossCtrl.gameObject.SetActive(false);

            _directionStartCamera.Priority = 30;

            yield return _messagePlayer.PlayMessageCorountine(MessageType.Stage_Boss_Start1);

            EventManager.OnEvent(Events.BossStage_HeadingBossFeet);
            yield return _directionCameraMng.StartDirectionCoroutine(CameraDirectionType.BossStage_HeadingBossFeet);

            _directionStartCamera.Priority = 0;

            yield return _messagePlayer.PlayMessageCorountine(MessageType.Stage_Boss_Start2);

            yield return _directionCameraMng.StartDirectionCoroutine(CameraDirectionType.BossStage_SlowlyRise);

            yield return _directionCameraMng.StartDirectionCoroutine(CameraDirectionType.BossStage_OnBossFace);

            AudioManager.PlaySE(SEType.BossStage_QueenAppearance);
            OnFadeDescription(1, 0.25f);

            yield return new WaitForSeconds(2.5f);
            
            OnFadeDescription(0, 0.25f);

            yield return new WaitForSeconds(1.0f);

            yield return _directionCameraMng.StartDirectionCoroutine(CameraDirectionType.BossStage_BehindBoss);
            yield return _messagePlayer.PlayMessageCorountine(MessageType.Stage_Boss_Start3);

            EventManager.OnEvent(Events.BossStage_QueenAnger);
            yield return new WaitForSeconds(2f);

            yield return _messagePlayer.PlayMessageCorountine(MessageType.Stage_Boss_Start4);
        }

        EventManager.OnEvent(Events.BossStage_DisolveCheshire);
        _directionBossObject.SetActive(false);
        _bossCtrl.gameObject.SetActive(true);

        yield return new WaitForSeconds(2.0f);

        TransitionManager.FadeIn(FadeType.Black_default,
                         action: () =>
                         {
                             _directionCameraMng.ResetCamera();
                         });

        yield return new WaitForSeconds(1.5f);

        yield return MessagePlayer.Instance.PlayMessageCorountine(MessageType.Stage_Boss_JustBeforeBattle);

        yield return new WaitForSeconds(1.0f);

        AudioManager.PlaySE(SEType.Trump_Alignment);
        VibrationController.OnVibration(Strength.Low, 0.2f);

        yield return new WaitForSeconds(0.3f);

        AudioManager.PlaySE(SEType.Trump_Alignment);
        VibrationController.OnVibration(Strength.Low, 0.2f);

        yield return new WaitForSeconds(0.3f);

        AudioManager.PlaySE(SEType.Trump_Alignment);
        VibrationController.OnVibration(Strength.Low, 0.2f);

        yield return new WaitForSeconds(0.3f);

        yield return new WaitForSeconds(2.0f);
        
        TransitionManager.FadeOut(FadeType.Black_default);
        InGameSetup();

        yield return new WaitForSeconds(_cameraBlendTime);

        action?.Invoke();
        _isDirecting = false;
        //�C���Q�[���̏������J�n
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
        _isInBattle.Value = false;
        _hpPanel.alpha = 0;
        _infoImages[0].enabled = false;
        _infoImages[1].enabled = false;
    }

    void SubscribeStartEvents()
    {
        EventManager.ListenEvents(Events.BossStage_FrontAlice, () => StartCoroutine(_directionCameraMng.StartDirectionCoroutine(CameraDirectionType.BossStage_FrontAlice)));
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

    void SubscribeEndEvents()
    {
        EventManager.ListenEvents(Events.BossStage_End_Start, () => StartCoroutine(_directionCameraMng.StartDirectionCoroutine(CameraDirectionType.BossStage_End_Start)));
        EventManager.ListenEvents(Events.BossStage_End_OnsideQueen, () => StartCoroutine(_directionCameraMng.StartDirectionCoroutine(CameraDirectionType.BossStage_End_OnSideQueen)));
        EventManager.ListenEvents(Events.BossStage_End_GoAroundFrontQueen, () => StartCoroutine(_directionCameraMng.StartDirectionCoroutine(CameraDirectionType.BossStage_End_GoAroundFrontQueen)));
        EventManager.ListenEvents(Events.BossStage_End_OnFace, () => StartCoroutine(_directionCameraMng.StartDirectionCoroutine(CameraDirectionType.Bossstage_End_OnFace)));
        EventManager.ListenEvents(Events.BossStage_End_AliceFront, () => StartCoroutine(_directionCameraMng.StartDirectionCoroutine(CameraDirectionType.BossStage_End_AliceFront)));
        EventManager.ListenEvents(Events.BossStage_End_ZoomAlice, () => StartCoroutine(_directionCameraMng.StartDirectionCoroutine(CameraDirectionType.BossStage_End_ZoomAlice)));
        EventManager.ListenEvents(Events.BossStage_End_FrontQueen, () => StartCoroutine(_directionCameraMng.StartDirectionCoroutine(CameraDirectionType.BossStage_End_FrontQueen)));
        EventManager.ListenEvents(Events.BossStage_End_AliceOblique, () => StartCoroutine(_directionCameraMng.StartDirectionCoroutine(CameraDirectionType.BossStage_End_AliceOblique)));
        EventManager.ListenEvents(Events.BossStage_End_LeftToRightTrump, () => StartCoroutine(_directionCameraMng.StartDirectionCoroutine(CameraDirectionType.BossStage_End_LeftToRightTrump)));
        EventManager.ListenEvents(Events.BossStage_End_RightToLeftTrump, () => StartCoroutine(_directionCameraMng.StartDirectionCoroutine(CameraDirectionType.BossStage_End_RightToLeftTrump)));
        EventManager.ListenEvents(Events.BossStage_End_AliceDiagonallyBack, () => StartCoroutine(_directionCameraMng.StartDirectionCoroutine(CameraDirectionType.BossStage_End_AliceDiagonallyBack)));
        EventManager.ListenEvents(Events.BossStage_End_QueenDepend, () => StartCoroutine(_directionCameraMng.StartDirectionCoroutine(CameraDirectionType.BossStage_End_QueenDepend)));
        EventManager.ListenEvents(Events.BossStage_End_CheshireFront, () => StartCoroutine(_directionCameraMng.StartDirectionCoroutine(CameraDirectionType.BossStage_End_CheshireFront)));
        EventManager.ListenEvents(Events.BossStage_End_AliceFloat1, () => StartCoroutine(_directionCameraMng.StartDirectionCoroutine(CameraDirectionType.BossStage_End_AliceFloat1)));
        EventManager.ListenEvents(Events.BossStage_End_AliceFloat2, () => StartCoroutine(_directionCameraMng.StartDirectionCoroutine(CameraDirectionType.BossStage_End_AliceFloat2)));
        EventManager.ListenEvents(Events.BossStage_End_CheshireOverhead, () => StartCoroutine(_directionCameraMng.StartDirectionCoroutine(CameraDirectionType.BossStage_End_CheshireOverhead)));
        EventManager.ListenEvents(Events.BossStage_End_AliceLookDown, () => StartCoroutine(_directionCameraMng.StartDirectionCoroutine(CameraDirectionType.BossStage_End_AliceLookDown)));
        EventManager.ListenEvents(Events.BossStage_End_CheshireLookUp, () => StartCoroutine(_directionCameraMng.StartDirectionCoroutine(CameraDirectionType.BossStage_End_CheshireLookUp)));
        EventManager.ListenEvents(Events.BossStage_End_AliceZoomUp, () => StartCoroutine(_directionCameraMng.StartDirectionCoroutine(CameraDirectionType.BossStage_End_AliceZoomUp)));

        if (!SkipButton.Instance.gameObject.activeSelf) 
        {
            SkipButton.Instance.gameObject.SetActive(true);
        }
        SkipButton.Instance.OnSkip.Subscribe(_ => 
        {
            TransitionManager.SetCanvasPriority(1010);
            MessagePlayer.Instance.StopMessage();
            TransitionManager.FadeIn(FadeType.White_Transparent, 0f);
            TransitionManager.SceneTransition(SceneType.Ending, FadeType.White_default);
        });
    }

    void Gameover()
    {
        StopCoroutine(_currentGameCoroutine);
        _currentGameCoroutine = null;
    }

    IEnumerator InGameCoroutine()
    {
        AudioManager.PlayBGM(BGMType.Boss_InGame);

        for (int i = 0; i < _battleNum; i++)
        {
            OnGameSetUp();
            _isInBattle.Value = true;
            _bossCtrl.ChangeFace(QueenFaceType.Default);

            if (i == 0)
            {
                _infoImages[0].enabled = true;
                yield return new WaitForSeconds(2.0f);
                _infoImages[0].enabled = false;
            }

            if (_debugMode)
            {
                i = 2;
            }

            yield return new WaitForSeconds(0.5f);

            //�o�g���t�F�C�Y���I������܂őҋ@
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

                                      if (i == 2)
                                      {
                                          _fallPoleGenerator.Generate(3);
                                      }
                                  });

            Debug.Log("�{�X����e�B�o�g���t�F�C�Y���I�����A���o���J�n");

            _isInBattle.Value = false;
            _areaEffect.transform.DOLocalMoveY(-3.5f, 1.0f);
            _bossCtrl.ChangeFace(QueenFaceType.Damage);
            if (_debrisGenerator.IsGenerating)
            {
                _debrisGenerator.StopGenerate();
                _debrisGenerator.Return();
            }

            //���݂̃t�F�C�Y�ɍ��킹�����o�̏������J�n
            yield return DirectionCoroutine((BossBattlePhase)i);
        }
        //�{�X��̃X�`���l�����o
        yield return GameManager.GetStillDirectionCoroutine(Stages.Stage_Boss, MessageType.GetStill_Stage_Boss, 2.0f);

        TransitionManager.FadeIn(FadeType.Black_default, action: () =>
        {
            GetStillController.InactiveGetStillPanel();
            _trumpSolderMng.OnAllTrumpAnimation("Idle");
            SubscribeEndEvents();
            _inGameObjectsParent.SetActive(false);
            _endDirectionObjectsParent.SetActive(true);
        });

        yield return new WaitForSeconds(3.5f);
        TransitionManager.FadeOut(FadeType.Black_default);
        AudioManager.PlayBGM(BGMType.EndBossBattle);

        _isDirecting = true;

        _endDirectionCoroutine = StartCoroutine(_messagePlayer.PlayMessageCorountine(MessageType.Stage_Boss_End1));
        yield return _endDirectionCoroutine;

        _isDirecting = false;

        //�{�X��|�������Ƃ̏����������Ŏ��s���A�G���f�B���OScene�֑J�ڂ���\��
        TransitionManager.FadeIn(FadeType.White_Transparent, 0f);
        TransitionManager.SceneTransition(SceneType.Ending);
    }

    IEnumerator DirectionCoroutine(BossBattlePhase phase)
    {

        if (_debugMode)
        {
            phase = BossBattlePhase.Third;
        }

        if (phase != BossBattlePhase.Third)
        {
            CameraBlend(CameraType.Default, 1.5f);
        }

        yield return new WaitForSeconds(1.0f);

        _trumpSolderMng.OnAllTrumpAnimation("Shaking_Start");
        CharacterMovable?.Invoke(false);
        OnDirectionSetUp();

        //�Ō�̃t�F�C�Y�̏ꍇ�͐퓬�I���̉��o���s��
        if (phase == BossBattlePhase.Third)
        {
            yield return FinishBattleCoroutine();

            yield break;
        }

        yield return new WaitForSeconds(2.0f);

        _hpPanel.alpha = 0;
        HPManager.Instance.RecoveryHP();
        _bossCtrl.ChangeFace(QueenFaceType.Angry);
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
        AudioManager.PlaySE(SEType.BossStage_QueenQuiet);

        MessageType message = default;

        //���݂̃t�F�C�Y�ɍ��킹���e�L�X�g�f�[�^�̎�ނ�ݒ�
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

        yield return PartitioningBattleCoroutine(phase);
    }

    /// <summary>
    /// �e�I�u�W�F�N�g�̍Ĕz�u�B�d�؂蒼�����s��
    /// </summary>
    /// <returns></returns>
    IEnumerator PartitioningBattleCoroutine(BossBattlePhase phase)
    {
        if (phase == BossBattlePhase.First)
        {
            TransitionManager.FadeIn(FadeType.Normal, 0.2f, action: () =>
              {
                  CameraBlend(CameraType.ExcuteTrump, 0.01f);
                  TransitionManager.FadeOut(FadeType.Normal, 0.2f);
              });
        }

        //�v���C���[�̈ʒu�ƌ�����������
        _playerTrans.DOLocalMove(_playerStartTrans.position, 0f);
        _playerTrans.DOLocalRotate(Vector3.zero, 0f);
        _trumpSolderMng.OnAllTrumpAnimation("Idle");

        //�g�����v���̎���΂����o�̏���
        yield return _excuteDirection.ExcuteDirectionCoroutine(phase);

        if (phase == BossBattlePhase.Second)
        {
            CameraBlend(CameraType.Direction, 0.02f);
            yield return _messagePlayer.PlayMessageCorountine(MessageType.Stage_Boss_Down2_Addition);
            _bossCtrl.PlayBossAnimation(BossAnimationType.Idle);
            yield return new WaitForSeconds(1.0f);

            CameraBlend(CameraType.Default, 2.0f);
            yield return new WaitForSeconds(2.5f);
        }
        else
        {
            CameraBlend(CameraType.Default, 0.02f);
            _bossCtrl.PlayBossAnimation(BossAnimationType.Idle);
            yield return new WaitForSeconds(1.0f);
        }
    }

    IEnumerator FinishBattleCoroutine()
    {
        CameraBlend(CameraType.FinishBattle, 2.0f);
        _debrisGenerator.Return();
        _fallPoleGenerator.Return();

        yield return new WaitForSeconds(3.0f);

        AudioManager.PlaySE(SEType.BossStage_Down);
        VibrationController.OnVibration(Strength.Middle, 0.3f);

        yield return new WaitForSeconds(1.0f);

        yield return _messagePlayer.PlayMessageCorountine(MessageType.Stage_Boss_Down3);

        yield return new WaitForSeconds(1.0f);

        _infoImages[1].enabled = true;
        _hpPanel.alpha = 0;
        AudioManager.PlayBGM(BGMType.BossStage_Clear, false);
        ItemGenerator.Instance.Return();

        yield return new WaitForSeconds(11.5f);

        _infoImages[1].enabled = false;
    }

    IEnumerator SkipCorourine()
    {
        TransitionManager.FadeIn(FadeType.Black_default, action: () => 
        {
            EventManager.OnEvent(Events.BossStage_DisolveCheshire);
            MessagePlayer.Instance.StopMessage();
            MessagePlayer.Instance.FadeMessageCanvas(0f, 0f);
            _directionBossObject.SetActive(false);
            _bossCtrl.gameObject.SetActive(true);
            InGameSetup();
            _directionCameraMng.ResetCamera();
            SkipButton.Instance.ResetSubscribe();
            SkipButton.Instance.gameObject.SetActive(false);
        });

        yield return new WaitForSeconds(2.5f);

        TransitionManager.FadeOut(FadeType.Black_default);

        yield return new WaitForSeconds(_cameraBlendTime);

        //�C���Q�[���̏������J�n
        _currentGameCoroutine = StartCoroutine(InGameCoroutine());
    }

    /// <summary>
    /// ���݂̃J��������ʂ̃J�����֑J�ڂ���
    /// </summary>
    /// <param name="type"> �J�����̎�� </param>
    /// <param name="blendTime"> �J�ڂɂ����鎞�� </param>
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
    /// �Q�[���I�[�o�[���o���Đ�����
    /// </summary>
    void OnBossStageGameOver()
    {
        StartCoroutine(GameOverCoroutine());
    }

    void InGameSetup()
    {
        //�Z���t�̍Đ��I�����Ƀ{�X�̃��[�V���������Z�b�g�A�J������퓬�p�ɕύX
        _bossCtrl.PlayBossAnimation(BossAnimationType.Idle, 0.3f);
        _trumpSolderMng.OnAllTrumpActivate(true); //�g�����v�����A�N�e�B�u��
        LetterboxController.ActivateLetterbox(true);
        CameraBlend(CameraType.Default, 0);
        AudioManager.StopBGM(_cameraBlendTime / 2);
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
/// �{�X��̃p�����[�^�[
/// </summary>
[Serializable]
public struct BossBattleParameter
{
    public string PramName;
    public DifficultyType DifficultyType;
    public PhaseParameter[] PhaseParameters;
}
