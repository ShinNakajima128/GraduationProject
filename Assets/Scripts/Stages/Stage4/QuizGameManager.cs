using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
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

    [Tooltip("����̐��𐔂��鎞��")]
    [SerializeField]
    float _viewingTime = 10.0f;

    [Header("Camera")]
    [Tooltip("�J����Transform")]
    [SerializeField]
    Transform _cameraTrans = default;

    [Tooltip("�J�����̏����ʒu")]
    [SerializeField]
    Transform _startCameraPos = default;

    [Tooltip("�J�����̓��B�ʒu")]
    [SerializeField]
    Transform _endCameraTrans = default;

    [Tooltip("�J�����̃A�j���[�V�����̎��")]
    [SerializeField]
    Ease _cameraEase = default;

    [Header("Objects")]
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
    #endregion
    #region private
    #endregion
    #region property
    public override Action GameStart { get; set; }
    public override Action GamePause { get; set; }
    public override Action GameEnd { get; set; }
    #endregion

    protected override void Start()
    {
        base.Start();
        OnGameStart();
    }

    public override void OnGameStart()
    {
        _informationText.text = "";
        _cameraTrans.DOMoveX(_startCameraPos.position.x, 3.0f)
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
        StartCoroutine(InGameCoroutine());
    }

    protected override IEnumerator GameEndCoroutine(Action action = null)
    {
        yield return null;
    }

    IEnumerator InGameCoroutine()
    {
        bool isAnswerSelect;

        for (int i = 0; i < _quizNum; i++)
        {
            isAnswerSelect = false;

            if (i > 0)
            {
                TransitionManager.FadeIn(FadeType.Normal, () =>
                {
                    _cameraTrans.DOMoveX(_startCameraPos.position.x, 0f);
                    TransitionManager.FadeOut(FadeType.Normal, () =>
                    {
                        Viewing(() =>
                        {
                            isAnswerSelect = true;
                        });
                    });
                });
            }
            else
            {
                //�Q�[���̔z�u�̃Z�b�g�A�b�v�����������ɋL�q
                Viewing(() =>
                {
                    isAnswerSelect = true;
                });
            }
            yield return new WaitUntil(() => isAnswerSelect); //�v���C���[�̑I����҂�
        }
    }
    void Viewing(Action action = null)
    {
        //�Q�[���̔z�u�̃Z�b�g�A�b�v�����������ɋL�q
        _cameraTrans.DOMoveX(_endCameraTrans.position.x, _viewingTime)
                    .SetEase(_cameraEase)
                    .OnComplete(() =>
                    {
                        action?.Invoke();
                        Debug.Log("�N�C�Y�\��");
                    });
    }
}
