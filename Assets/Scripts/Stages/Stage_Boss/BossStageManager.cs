using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    [Header("UI")]
    [Tooltip("�퓬�I�����̉��")]
    [SerializeField]
    CanvasGroup _finishBattleCanvas = default;

    [Header("Component")]
    [SerializeField]
    MessagePlayer _messagePlayer = default;

    [SerializeField]
    BossController _bossCtrl = default;

    [Tooltip("�g�����v���̏��Y�̉��o���s��Component")]
    [SerializeField]
    ExcuteDirection _excuteDirection = default;

    [Header("Debug")]
    [SerializeField]
    bool _debugMode = false;
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
        _playerTrans = GameObject.FindGameObjectWithTag("Player").transform; //�v���C���[��Transform���擾

        //�o�g�������ǂ����̒l���ς�������ɍs��������o�^
        _isInBattle.Subscribe(_ => OnInGame?.Invoke(_isInBattle.Value)).AddTo(this);
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
        AudioManager.PlayBGM(BGMType.Boss_Before);

        if (!_debugMode)
        {
            yield return new WaitForSeconds(1.5f);

            //�J�������{�X�Ɋ񂹂�
            CameraBlend(CameraType.Direction, _cameraBlendTime);

            yield return new WaitForSeconds(_cameraBlendTime);

            //�{�X��J�n���̉��o���Đ�
            StartCoroutine(_messagePlayer.PlayMessageCorountine(MessageType.Stage_Boss_Start, () =>
            {
                _isDirecting = true;
            }));

            yield return new WaitUntil(() => _isDirecting);

            _isDirecting = false;
        }

        //�Z���t�̍Đ��I�����Ƀ{�X�̃��[�V���������Z�b�g�A�J������퓬�p�ɕύX
        _bossCtrl.PlayBossAnimation(BossAnimationType.Idle, 0.3f);
        CameraBlend(CameraType.Default, _cameraBlendTime);
        AudioManager.StopBGM(_cameraBlendTime / 2);

        yield return new WaitForSeconds(_cameraBlendTime);

        action?.Invoke();
        //�C���Q�[���̏������J�n
        StartCoroutine(InGameCoroutine());
    }

    protected override IEnumerator GameEndCoroutine(Action action = null)
    {
        throw new NotImplementedException();
    }

    protected override void Init()
    {
        _messagePlayer.Closeup += OnCloseup;
        //_messagePlayer.Reset += OnReset;
        _isInBattle.Value = false;
    }

    IEnumerator InGameCoroutine()
    {
        _isInBattle.Value = true;
        AudioManager.PlayBGM(BGMType.Boss_InGame);

        for (int i = 0; i < _battleNum; i++)
        {
            OnGameSetUp();

            //�o�g���t�F�C�Y���I������܂őҋ@
            yield return _bossCtrl.BattlePhaseCoroutine((BossBattlePhase)i, () =>
            {
                CharacterMovable?.Invoke(true);
                CameraBlend(CameraType.Battle, _cameraBlendTime);
            });

            Debug.Log("�{�X����e�B�o�g���t�F�C�Y���I�����A���o���J�n");
            _areaEffect.SetActive(false);

            //���݂̃t�F�C�Y�ɍ��킹�����o�̏������J�n
            yield return DirectionCoroutine((BossBattlePhase)i);
        }

        //�{�X��|�������Ƃ̏����������Ŏ��s���A�G���f�B���OScene�֑J�ڂ���\��
        TransitionManager.SceneTransition(SceneType.Ending);
    }

    IEnumerator DirectionCoroutine(BossBattlePhase phase)
    {
        CharacterMovable?.Invoke(false);
        OnDirectionSetUp();

        if (_debugMode)
        {
            phase = BossBattlePhase.Third;
        }

        //�Ō�̃t�F�C�Y�̏ꍇ�͐퓬�I���̉��o���s��
        if (phase == BossBattlePhase.Third)
        {
            yield return FinishBattleCoroutine();
            yield break;
        }

        yield return new WaitForSeconds(1.0f);

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

        yield return PartitioningBattleCoroutine();

        CameraBlend(CameraType.Default, _cameraBlendTime);

        yield return new WaitForSeconds(_cameraBlendTime);
    }

    /// <summary>
    /// �e�I�u�W�F�N�g�̍Ĕz�u�B�d�؂蒼�����s��
    /// </summary>
    /// <returns></returns>
    IEnumerator PartitioningBattleCoroutine()
    {
        TransitionManager.FadeIn(FadeType.Normal,0.2f, action: () =>
         {
            //�v���C���[�̈ʒu�ƌ�����������
            _playerTrans.DOLocalMove(_playerStartTrans.position, 0f);
             _playerTrans.DOLocalRotate(Vector3.zero, 0f);

             CameraBlend(CameraType.ExcuteTrump, 0.01f);
             TransitionManager.FadeOut(FadeType.Normal, 0.2f);
         });
        ////�v���C���[�̈ʒu�ƌ�����������


        //�g�����v���̎���΂����o�̏���
        yield return _excuteDirection.ExcuteDirectionCoroutine();

        CameraBlend(CameraType.Direction, 0.01f);
        _bossCtrl.PlayBossAnimation(BossAnimationType.Idle);
        yield return new WaitForSeconds(2.0f);
    }

    IEnumerator FinishBattleCoroutine()
    {
        CameraBlend(CameraType.FinishBattle, 2.0f);

        yield return new WaitForSeconds(3.0f);

        _finishBattleCanvas.alpha = 1;

        yield return new WaitForSeconds(2.0f);
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
