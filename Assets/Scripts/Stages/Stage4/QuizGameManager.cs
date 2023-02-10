using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Cinemachine;
using Unity.Collections;
using UniRx;
using UniRx.Triggers;

/// <summary>
/// �X�e�[�W4�̃N�C�Y�Q�[�����Ǘ�����}�l�[�W���[
/// </summary>
public class QuizGameManager : StageGame<QuizGameManager>
{
    #region serialize
    [Header("Variable")]
    [Tooltip("���̐�")]
    [SerializeField]
    int _quizNum = 5;

    [Tooltip("�N���A�ɕK�v�Ȑ����̐�")]
    [SerializeField]
    int _requiredCorrectNum = 0;

    [Tooltip("�e��Փx���̃N�C�Y�Q�[���̐��l")]
    [SerializeField]
    QuizGameParameter[] _quizGameParams = default;

    [Tooltip("����̐��𐔂��鎞��")]
    [SerializeField]
    float _viewingTime = 10.0f;

    [Header("Directions")]
    [Tooltip("�J�����̏����ʒu")]
    [SerializeField]
    Transform _startPlayerPos = default;

    [Tooltip("�J�����̓��B�ʒu")]
    [SerializeField]
    Transform _endPlayerTrans = default;

    [Tooltip("�J�����̃A�j���[�V�����̎��")]
    [SerializeField]
    Ease _playerMoveEase = default;

    [Header("Objects")]
    [Tooltip("�v���C���[��Transform")]
    [SerializeField]
    Transform _playerTrans = default;

    [Tooltip("�N�C�Y�R���g���[���[")]
    [SerializeField]
    QuizController _quizCtrl = default;

    [Tooltip("Object���Ǘ�����}�l�[�W���[")]
    [SerializeField]
    ObjectManager _objectMng = default;

    [Tooltip("�ŏ��̉��o�p�̃g�����v��")]
    [SerializeField]
    Transform _directionTrumpSoldier = default;

    [SerializeField]
    CheshireCat[] _cheshireCats = default;

    [Header("UI")]
    [SerializeField]
    Text _informationText = default;

    [SerializeField]
    CanvasGroup _questionPanelGroup = default;

    [SerializeField]
    CanvasGroup _hpGroup = default;

    [SerializeField]
    Text _questionText = default;

    [SerializeField]
    GameObject[] _targetIcons = default;

    [SerializeField]
    GameObject _gameStartIcon = default;

    [SerializeField]
    Image[] _infoImages = default;

    [Header("Camera")]
    [SerializeField]
    CinemachineVirtualCamera _quizCamera = default;

    [SerializeField]
    CinemachineBrain _brain = default;

    [Header("Components")]
    [SerializeField]
    AliceFaceController _faceController = default;

    [SerializeField]
    TrumpSolderGenerator _trumpGenerator = default;

    [Tooltip("������AudioSource")]
    [SerializeField]
    AudioSource _ambientSource = default;

    [Header("Debug")]
    [SerializeField]
    bool _debugMode = false;

    [Header("�e�X�g����N�C�Y�̎��")]
    [SerializeField]
    QuizType _debugQuizType = default;
    #endregion
    #region private
    /// <summary> ���������� </summary>
    int _corectNum = 0;
    Animator _playerAnim;
    Coroutine _gameCoroutine;
    bool _isFailured = false;
    #endregion

    #region public
    public override event Action GameSetUp;
    public override event Action GameStart;
    public override event Action GamePause;
    public override event Action GameEnd;
    #endregion
    #region property
    #endregion

    protected override void Awake()
    {
        base.Awake();
        _playerTrans.TryGetComponent(out _playerAnim);
    }
    protected override void Start()
    {
        AudioManager.PlayBGM(BGMType.Stage4);
        HPManager.Instance.LostHpAction += OnGameover;
        _brain.m_DefaultBlend.m_Time = 0f;
        base.Start();
        OnGameStart();

        //��肪�N�������p�Ƀ~�j�Q�[�����X�L�b�v����@�\��ǉ�
        this.UpdateAsObservable()
            .Where(_ => UIInput.Next)
            .Take(1)
            .Subscribe(_ =>
            {
                GameManager.SaveStageResult(true);
                TransitionManager.SceneTransition(SceneType.Lobby);
                AudioManager.StopBGM(1.5f);
            })
            .AddTo(this);
    }

    public override void OnGameStart()
    {
        _informationText.text = "";
        _infoImages[0].enabled = false;
        _infoImages[1].enabled = false;
        LetterboxController.ActivateLetterbox(true);
        HPManager.Instance.RecoveryHP();

        OnGameSetUp();

        _playerTrans.DOMove(_playerTrans.position, 1.4f)
                    .OnComplete(() =>
                    {
                        //��l���L�������X�^�[�g�ʒu�܂Ői��
                        _playerAnim.CrossFadeInFixedTime("Move", 0.2f);
                        _cheshireCats[0].ChangeState(CheshireCatState.FastWalk);
                    });
        
        _playerTrans.DOMoveX(_startPlayerPos.position.x, 3.0f)
                    .SetEase(Ease.Linear)
                    .OnComplete(() => 
                    {
                        _cheshireCats[0].ChangeState(CheshireCatState.Idle_Standing);
                        _playerAnim.CrossFadeInFixedTime("Idle", 0.2f);
                    })
                    .SetDelay(1.5f);

        _directionTrumpSoldier.DOMoveX(0f, 4.5f)
                              .SetEase(Ease.Linear)
                              .OnComplete(() =>
                              {
                                  _directionTrumpSoldier.gameObject.SetActive(false);
                                  LetterboxController.ActivateLetterbox(false);
                                  StartCoroutine(GameStartCoroutine(() =>
                                  {
                                      GameStart?.Invoke();
                                      _gameCoroutine = StartCoroutine(InGameCoroutine());
                                  }));
                              })
                              .SetDelay(2.5f);
    }

    public override void OnGameEnd()
    {
    }

    protected override void Init()
    {
    }

    protected override IEnumerator GameStartCoroutine(Action action = null)
    {
        //_informationText.text = "�X�^�[�g!";
        _infoImages[0].enabled = true;

        yield return new WaitForSeconds(1.5f);

        action?.Invoke();
        _infoImages[0].enabled = false;
        //_informationText.text = "";
    }

    protected override IEnumerator GameEndCoroutine(Action action = null)
    {
        yield return null;
    }

    IEnumerator InGameCoroutine()
    {
        bool isAnswerPhase;
        QuizType currentQuizType;

        for (int i = 0; i < _quizNum; i++)
        {
            isAnswerPhase = false;
            currentQuizType = (QuizType)i;

            if (i > 0 || _isFailured)
            {
                bool isCompleted = false;

                TransitionManager.FadeIn(FadeType.Normal, action: () =>
                {
                    _quizCamera.Priority = 0;
                    _hpGroup.alpha = 0;

                    _playerAnim.CrossFadeInFixedTime("Idle", 0.1f);
                    _faceController.ChangeFaceType(FaceType.Default);

                    _cheshireCats[0].gameObject.SetActive(true);
                    _cheshireCats[1].gameObject.SetActive(false);

                    _cheshireCats[0].ChangeState(CheshireCatState.Idle_Standing);

                    _playerTrans.DOMoveX(_startPlayerPos.position.x, 0f);
                    _playerTrans.DOLocalRotate(new Vector3(0f, 90f, 0f), 0f);

                    if (!_debugMode)
                    {
                        OnQuizSetUp(currentQuizType);
                    }
                    else
                    {
                        OnQuizSetUp(_debugQuizType);
                    }

                    TransitionManager.FadeOut(FadeType.Normal, action: () =>
                    {
                        isCompleted = true;
                    });
                });

                yield return new WaitUntil(() => isCompleted);

                //������^�[�Q�b�g��\��
                if (!_debugMode)
                {
                    
                    yield return QuestionCoroutine(currentQuizType);
                }
                else
                {
                    yield return QuestionCoroutine(_debugQuizType);
                }

                Viewing(() =>
                {
                    isAnswerPhase = true;
                });
            }
            else
            {
                if (!_debugMode)
                {
                    OnQuizSetUp(currentQuizType);
                    yield return QuestionCoroutine(currentQuizType);
                }
                else
                {
                    OnQuizSetUp(_debugQuizType);
                    yield return QuestionCoroutine(_debugQuizType);
                }

                Viewing(() =>
                {
                    isAnswerPhase = true;
                });
            }
            yield return new WaitUntil(() => isAnswerPhase); //�I�������\�������̂�ҋ@

            bool isCamerachanged = false;

            TransitionManager.FadeIn(FadeType.Normal, 0.5f, action: () =>
            {
                _quizCamera.Priority = 30;
                _playerAnim.CrossFadeInFixedTime("SitIdle", 0f);
                _playerTrans.DOLocalRotate(new Vector3(0f, 225f, 0f), 0f);
                _cheshireCats[0].gameObject.SetActive(false);
                _cheshireCats[1].gameObject.SetActive(true);

                _cheshireCats[1].ChangeState(CheshireCatState.LyingDown, 0f);
            });

            yield return new WaitForSeconds(0.8f);

            TransitionManager.FadeOut(FadeType.Normal, 0.5f, () =>
            {
                isCamerachanged = true;
            });

            yield return new WaitUntil(() => isCamerachanged);

            _hpGroup.alpha = 1;

            //�I������\��
            if (!_debugMode)
            {
                yield return StartCoroutine(_quizCtrl.OnChoicePhaseCoroutine(_objectMng, currentQuizType, 
                x => 
                {
                    _corectNum += x;

                    if (x == 1)
                    {
                        _playerAnim.CrossFadeInFixedTime("SitHappy", 0.2f);
                        _faceController.ChangeFaceType(FaceType.Smile);
                        _isFailured = false;
                    }
                    else
                    {
                        _playerAnim.CrossFadeInFixedTime("SitSad", 0.2f);
                        _faceController.ChangeFaceType(FaceType.Cry);
                        HPManager.Instance.ChangeHPValue(1);
                        _isFailured = true;
                        i--;
                    }
                }));
            }
            else
            {
                yield return StartCoroutine(_quizCtrl.OnChoicePhaseCoroutine(_objectMng, _debugQuizType, x =>
                {
                    _corectNum += x;

                    if (x == 1)
                    {
                        _playerAnim.CrossFadeInFixedTime("SitHappy", 0.2f);
                        _faceController.ChangeFaceType(FaceType.Smile);
                    }
                    else
                    {
                        _playerAnim.CrossFadeInFixedTime("SitSad", 0.2f);
                        _faceController.ChangeFaceType(FaceType.Cry);
                        HPManager.Instance.ChangeHPValue(1);
                    }
                }));
            }
        }

        GameEnd?.Invoke();

        Debug.Log($"����������/��萔�F{_corectNum}/{_quizNum}");

        if (_corectNum >= _requiredCorrectNum)
        {
            //_informationText.text = "�X�e�[�W�N���A�I";
            _ambientSource.Stop();
            _infoImages[1].enabled = true;
            GameManager.SaveStageResult(true);
            AudioManager.PlayBGM(BGMType.ClearJingle, false);
            _hpGroup.alpha = 0;

            yield return new WaitForSeconds(3f);

            _infoImages[1].enabled = false;
            _informationText.text = "";

            if (!GameManager.CheckStageStatus())
            {
                yield return GameManager.GetStillDirectionCoroutine(Stages.Stage4, AliceProject.MessageType.GetStill_Stage4);
            }
            else
            {
                GameManager.ChangeLobbyState(LobbyState.Default);
            }
        }
        else
        {
            _informationText.text = "�X�e�[�W���s�c";
            GameManager.SaveStageResult(false);
            yield return new WaitForSeconds(1.0f);
        }
        yield return new WaitForSeconds(1.0f);

        TransitionManager.FadeIn(FadeType.Black_Transparent, 0f);
        TransitionManager.SceneTransition(SceneType.Lobby);
    }

    IEnumerator QuestionCoroutine(QuizType type)
    {
        _questionText.text = $"{(int)type + 1}";
        _questionPanelGroup.alpha = 1;
        AudioManager.PlaySE(SEType.Stage4_Question1);

        yield return new WaitForSeconds(1.5f);

        AudioManager.PlaySE(SEType.Stage4_Question2);

        switch (type)
        {
            case QuizType.RedRose:
                _targetIcons[0].SetActive(true);
                break;
            case QuizType.WhiteRose:
                _targetIcons[1].SetActive(true);
                break;
            case QuizType.RedAndWhiteRose:
                _targetIcons[2].SetActive(true);
                break;
            case QuizType.TrumpSolder:
                _targetIcons[3].SetActive(true);
                break;
            case QuizType.All:
                _targetIcons[0].SetActive(true);
                _targetIcons[1].SetActive(true);
                _targetIcons[2].SetActive(true);
                break;
            default:
                break;
        }

        yield return new WaitForSeconds(1.5f);

        _gameStartIcon.SetActive(true);
        yield return new WaitUntil(() => UIInput.Submit);

        _gameStartIcon.SetActive(false);
        _questionText.text = "";
        _targetIcons[0].SetActive(false);
        _targetIcons[1].SetActive(false);
        _targetIcons[2].SetActive(false);
        _questionPanelGroup.alpha = 0;
    }

    void Viewing(Action action = null)
    {
        //�Q�[���̔z�u�̃Z�b�g�A�b�v�����������ɋL�q
        _playerAnim.CrossFadeInFixedTime("Move", 0.1f);
        _cheshireCats[0].ChangeState(CheshireCatState.FastWalk);

        
        _playerTrans.DOMoveX(_endPlayerTrans.position.x, _viewingTime)
                    .SetEase(_playerMoveEase)
                    .OnComplete(() =>
                    {
                        action?.Invoke();
                        _playerAnim.CrossFadeInFixedTime("Idle", 0.2f);
                        _cheshireCats[0].ChangeState(CheshireCatState.Idle_Standing);
                        Debug.Log("�N�C�Y�\��");
                    });
    }

    /// <summary>
    /// �N�C�Y�Q�[���̃Z�b�g�A�b�v
    /// </summary>
    public override void OnGameSetUp()
    {
        var param = _quizGameParams.FirstOrDefault(p => p.DifficultyType == GameManager.Instance.CurrentGameDifficultyType);
        _viewingTime = param.ViewingTime;
        _trumpGenerator.SetGenerateCount(param);
        _hpGroup.alpha = 0;

        GameSetUp?.Invoke();
    }

    /// <summary>
    /// �N�C�Y�̃Z�b�g�A�b�v
    /// </summary>
    /// <param name="type"></param>
    void OnQuizSetUp(QuizType type)
    {
        QuizSetUp?.Invoke(type);
    }

    void OnGameover()
    {
        StartCoroutine(GameoverDirectionCoroutine());
    }

    IEnumerator GameoverDirectionCoroutine()
    {
        StopCoroutine(_gameCoroutine);
        _gameCoroutine = null;

        yield return new WaitForSeconds(2.5f);

        GameoverDirection.Instance.OnGameoverDirection();
    }
}

/// <summary>
/// �N�C�Y�Q�[���̊e�p�����[�^�[
/// </summary>
[Serializable]
public struct QuizGameParameter
{
    public string ParamName;
    public DifficultyType DifficultyType;
    public int ViewingTime;
    public int StandingGenerateMaxCount;
    public int WalkGenerateMaxCount;
    public int PaintGenerateMaxCount;
    public int LoafGenerateMaxCount;
    public int DipGenerateMaxCount;
    public int HideTreeGenerateMaxCount;
    public int HideBucketGenerateMaxCount;
}