using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using AliceProject;

/// <summary>
/// �N���b�P�[�Q�[���S�̂��Ǘ�����}�l�[�W���[�N���X
/// </summary>
public class CroquetGameManager : StageGame<CroquetGameManager>
{
    #region serialize
    [Header("Variables")]
    [Tooltip("�v���C�����")]
    [SerializeField]
    int _playCount = 3;

    [Tooltip("�X�e�[�W�ɃN���A�ɕK�v�ȉ�")]
    [SerializeField]
    int _requiredSuccessCount = 3;

    [Tooltip("��Փx���̃Q�[���̐��l")]
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
    /// <summary> �|���ڕW�� </summary>
    int _currentTargetStrikeNum;
    int _currentRedStrileNum = 0;
    int _currentBlackStrikeNum = 0;
    bool _isGoaled = false;
    /// <summary> �|������ </summary>
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
    /// �Q�[�����̃R���[�`��
    /// </summary>
    IEnumerator InGameCoroutine()
    {
        if (!_debugMode)
        {
            OrderData data = default;

            for (int i = 0; i < _playCount; i++)
            {
                //�Q�[���̃Z�b�g�A�b�v
                OnGameSetUp();

                if (i < 3)
                {
                    _trumpMng.SetTrumpSolder((AlignmentType)i);
                }
                else
                {
                    //�g�����v���̕��ѕ��������_���ɃZ�b�g
                    var random = (AlignmentType)UnityEngine.Random.Range(0, 3);
                    _trumpMng.SetTrumpSolder(random);
                }

                //����̃f�[�^���擾
                data = _order.Data[i];

                _currentTargetTrumpColor = data.TargetTrumpColor;
                _currentTargetStrikeNum = data.TargetNum;

                _gameUI.SetOrderText(i + 1, data.ToString());
                _gameUI.SetTrumpCount(_currentRedStrileNum, _currentBlackStrikeNum);
                _gameUI.SetResultText("");

                _cameraMng.ChangeCamera(CroquetCameraType.View, 3.0f);

                yield return new WaitForSeconds(3.0f);

                //����̃A�j���[�V�������I������܂őҋ@
                yield return _order.LetterAnimationCoroutine(i + 1, data.ToString());

                _cameraMng.ChangeCamera(CroquetCameraType.InGame);
                LetterboxController.ActivateLetterbox(false, 1.5f);

                yield return new WaitForSeconds(1.5f);

                _gameUI.ChangeUIGroup(CroquetGameState.InGame);

                //���߂ăX�e�[�W3���v���C���Ă���ꍇ
                if (GameManager.Instance.IsFirstVisitCurrentStage)
                {
                    //�t�F�C�Y���̉�b�p�[�g���Đ�
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

                _player.BeginControl(); //���͎�t�J�n

                yield return new WaitUntil(() => _player.IsThrowed);

                _cameraMng.ChangeCamera(CroquetCameraType.Strike, 1.0f);

                //�����F�����Ƀn���l�Y�~���S�[������܂ŏ�����ҋ@���鏈�����L�q
                yield return new WaitUntil(() => _isGoaled);

                _isGoaled = false;

                //�S�[���������ɃV���[�g�̌��ʂɉ����Č��ʂ����o��ύX
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
                        _gameUI.SetResultText("�X�e�[�W�N���A�I");
                        GameManager.SaveStageResult(true);
                        yield return new WaitForSeconds(2.0f);
                        _gameUI.SetResultText("");
                        yield return GameManager.GetStillDirectionCoroutine(Stages.Stage3, MessageType.GetStill_Stage3);
                    }
                    else
                    {
                        _gameUI.SetResultText("�X�e�[�W���s�c");
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
    /// �n���l�Y�~���S�[���n�_�ɒ��������̃R���[�`��
    /// </summary>
    /// <param name="result"> �ł������� </param>
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
    /// �^�[�Q�b�g�̓|�����������Z����
    /// </summary>
    /// <param name="type"> �|�����g�����v�̎�� </param>
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
    /// �S�[���G�t�F�N�g���Đ�
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
/// �N���b�P�[�Q�[���̃X�e�[�^�X
/// </summary>
public enum CroquetGameState
{
    Order,
    InGame,
    GoalDirection,
    Finish
}

/// <summary>
/// �N���b�P�[�Q�[���̓�Փx���̃p�����[�^�[
/// </summary>
[Serializable]
public struct CroquetGameParameter
{
    public string ParamName;
    public DifficultyType DifficultyType;
    public OrderData[] OrderDatas;
}
