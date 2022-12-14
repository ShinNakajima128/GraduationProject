using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
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

    [Header("UI")]
    [SerializeField]
    Text _informationText = default;

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
        base.Start();
        OnGameStart();
    }

    public override void OnGameStart()
    {
        _informationText.text = "";

        //��l���L�������X�^�[�g�ʒu�܂Ői��
        _playerAnim.CrossFadeInFixedTime("Move", 0.1f);
        _playerTrans.DOMoveX(_startPlayerPos.position.x, 3.0f)
                    .OnComplete(() => 
                    {
                        _playerAnim.CrossFadeInFixedTime("Idle", 0.1f);
                    })
                    .SetDelay(1.5f);

        _directionTrumpSoldier.DOMoveX(-10f, 4.5f)
                              .SetEase(Ease.Linear)
                              .OnComplete(() =>
                              {
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
                TransitionManager.FadeIn(FadeType.Normal, action: () =>
                {
                    if (!_debugMode)
                    {
                        OnQuizSetUp(currentQuizType);
                    }
                    else
                    {
                        OnQuizSetUp(_debugQuizType);
                    }
                    _playerTrans.DOMoveX(_startPlayerPos.position.x, 0f);
                    TransitionManager.FadeOut(FadeType.Normal, action: () =>
                    {
                        Viewing(() =>
                        {
                            isAnswerPhase = true;
                        });
                    });
                });
            }
            else
            {
                if (!_debugMode)
                {
                    OnQuizSetUp(currentQuizType);
                }
                else
                {
                    OnQuizSetUp(_debugQuizType);
                }
                Viewing(() =>
                {
                    isAnswerPhase = true;
                });
            }
            yield return new WaitUntil(() => isAnswerPhase); //�I�������\�������̂�ҋ@

            if (!_debugMode)
            {
                yield return StartCoroutine(_quizCtrl.OnChoicePhaseCoroutine(_objectMng, currentQuizType, x => _corectNum += x));
            }
            else
            {
                yield return StartCoroutine(_quizCtrl.OnChoicePhaseCoroutine(_objectMng, _debugQuizType, x => _corectNum += x));
            }
        }

        GameEnd?.Invoke();

        Debug.Log($"����������/��萔�F{_corectNum}/{_quizNum}");

        if (_corectNum >= _requiredCorrectNum)
        {
            _informationText.text = "�X�e�[�W�N���A�I";
            GameManager.SaveStageResult(true);
        }
        else
        {
            _informationText.text = "�X�e�[�W���s�c";
            GameManager.SaveStageResult(false);
        }
        yield return new WaitForSeconds(2.5f);

        TransitionManager.SceneTransition(SceneType.Lobby);

    }
    void Viewing(Action action = null)
    {
        //�Q�[���̔z�u�̃Z�b�g�A�b�v�����������ɋL�q
        _playerAnim.CrossFadeInFixedTime("Move", 0.1f);
        _playerTrans.DOMoveX(_endPlayerTrans.position.x, _viewingTime)
                    .SetEase(_playerMoveEase)
                    .OnComplete(() =>
                    {
                        action?.Invoke();
                        _playerAnim.CrossFadeInFixedTime("Idle", 0.1f);
                        Debug.Log("�N�C�Y�\��");
                    });
    }

    public override void OnGameSetUp()
    {
        GameSetUp?.Invoke();
    }
    void OnQuizSetUp(QuizType type)
    {
        QuizSetUp?.Invoke(type);
    }
}
