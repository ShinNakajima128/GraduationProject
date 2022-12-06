using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using AliceProject;
using UniRx;

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

    [Header("Objects")]
    [Tooltip("�퓬���̈ړ��\�͈͂̃G�t�F�N�g")]
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
    #endregion
    #region property
    public static new BossStageManager Instance { get; private set; }
    #endregion

    protected override void Awake()
    {
        Instance = this;
        TryGetComponent(out _impulseSource);

        //�o�g�������ǂ����̒l���ς�������ɍs��������o�^
        _isInBattle.Subscribe(_ => OnInGame.Invoke(_isInBattle.Value));
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

    protected override IEnumerator GameStartCoroutine(Action action = null)
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
        }
        
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
