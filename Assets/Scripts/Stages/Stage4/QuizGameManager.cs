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
    Text _questionText = default;

    [SerializeField]
    GameObject[] _targetIcons = default;

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
        _brain.m_DefaultBlend.m_Time = 0f;
        base.Start();
        OnGameStart();
    }

    public override void OnGameStart()
    {
        _informationText.text = "";
        LetterboxController.ActivateLetterbox(true);
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
                                      StartCoroutine(InGameCoroutine());
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
        _informationText.text = "�X�^�[�g!";

        yield return new WaitForSeconds(1.5f);

        action?.Invoke();
        _informationText.text = "";
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

            if (i > 0)
            {
                bool isCompleted = false;

                TransitionManager.FadeIn(FadeType.Normal, action: () =>
                {
                    _quizCamera.Priority = 0;

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

            if (!_debugMode)
            {
                yield return StartCoroutine(_quizCtrl.OnChoicePhaseCoroutine(_objectMng, currentQuizType, x => 
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
                    }
                }));
            }
        }

        GameEnd?.Invoke();

        Debug.Log($"����������/��萔�F{_corectNum}/{_quizNum}");

        if (_corectNum >= _requiredCorrectNum)
        {
            _informationText.text = "�X�e�[�W�N���A�I";
            GameManager.SaveStageResult(true);

            yield return new WaitForSeconds(2.0f);
            _informationText.text = "";

            yield return GameManager.GetStillDirectionCoroutine(Stages.Stage4, AliceProject.MessageType.GetStill_Stage4);
        }
        else
        {
            _informationText.text = "�X�e�[�W���s�c";
            GameManager.SaveStageResult(false);
            yield return new WaitForSeconds(1.0f);
        }
        yield return new WaitForSeconds(1.0f);

        TransitionManager.FadeIn(FadeType.Black_TransParent, 0f);
        TransitionManager.SceneTransition(SceneType.Lobby);

    }

    IEnumerator QuestionCoroutine(QuizType type)
    {
        _questionText.text = "";
        _questionPanelGroup.alpha = 1;

        yield return new WaitForSeconds(1.5f);

        switch (type)
        {
            case QuizType.RedRose:
                _questionText.text = "�ԃo��";
                _targetIcons[0].SetActive(true);
                break;
            case QuizType.WhiteRose:
                _questionText.text = "���o��";
                _targetIcons[1].SetActive(true);
                break;
            case QuizType.RedAndWhiteRose:
                _questionText.text = "�ԃo�������o��";
                _targetIcons[0].SetActive(true);
                _targetIcons[1].SetActive(true);
                break;
            case QuizType.TrumpSolder:
                _questionText.text = "�g�����v��";
                _targetIcons[2].SetActive(true);
                break;
            case QuizType.All:
                _questionText.text = "�S��";
                _targetIcons[0].SetActive(true);
                _targetIcons[1].SetActive(true);
                _targetIcons[2].SetActive(true);
                break;
            default:
                break;
        }

        yield return new WaitForSeconds(2.5f);

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
                        //_playerTrans.DOLocalRotate(new Vector3(0f, 180f, 0f), 0.25f);
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