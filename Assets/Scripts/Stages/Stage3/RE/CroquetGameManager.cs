using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    int _currentTargetStrikeNum;
    bool _isGoaled = false;
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
        base.Start();
        Init();
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
                _gameUI.SetTrumpCount(_trumpMng.CurrentRedTrumpCount, _trumpMng.CurrentBlackTrumpCount);
                _gameUI.SetResultText("");

                _cameraMng.ChangeCamera(CroquetCameraType.View, 3.0f);

                yield return new WaitForSeconds(3.0f);

                yield return _order.LetterAnimationCoroutine(i + 1, data.ToString());

                TransitionManager.FadeIn(FadeType.Normal, action: () =>
                {
                    _cameraMng.ChangeCamera(CroquetCameraType.InGame, 0f);
                    _gameUI.ChangeUIGroup(CroquetGameState.InGame);
                    TransitionManager.FadeOut(FadeType.Normal);
                });

                yield return new WaitForSeconds(3.0f);

                _player.BeginControl();

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
                        TransitionManager.FadeOut(FadeType.Normal);
                    });

                    yield return new WaitForSeconds(3.5f);
                }
                else
                {
                    if (_requiredSuccessCount >= _successCount)
                    {
                        _gameUI.ChangeUIGroup(CroquetGameState.Finish);

                        yield return new WaitForSeconds(2.0f);

                        GameManager.SaveStageResult(true);
                    }
                    else
                    {
                        GameManager.SaveStageResult(false);
                    }
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
        _cameraMng.ChangeCamera(CroquetCameraType.Goal);
        _gameUI.ChangeUIGroup(CroquetGameState.GoalDirection);

        yield return new WaitForSeconds(2.0f);

        if (result)
        {
            _gameUI.SetResultText("����B���I");
        }
        else
        {
            _gameUI.SetResultText("���莸�s�c");
        }
        yield return new WaitForSeconds(3.0f);
    }

    protected override void Init()
    {
        _player.GoalAction(() =>
        {
            _isGoaled = true;
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

        _gameUI.SetTrumpCount(_trumpMng.CurrentRedTrumpCount, _trumpMng.CurrentBlackTrumpCount);
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
