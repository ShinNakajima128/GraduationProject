using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UniRx;

/// <summary>
/// ステージ4のクイズゲームを管理するマネージャー
/// </summary>
public class QuizGameManager : StageGame<QuizGameManager>
{
    #region serialize
    [Header("Variable")]
    [Tooltip("問題の数")]
    [SerializeField]
    int _quizNum = 5;

    [Tooltip("お題の数を数える時間")]
    [SerializeField]
    float _viewingTime = 10.0f;

    [Header("Camera")]
    [Tooltip("カメラTransform")]
    [SerializeField]
    Transform _cameraTrans = default;

    [Tooltip("カメラの初期位置")]
    [SerializeField]
    Transform _startCameraPos = default;

    [Tooltip("カメラの到達位置")]
    [SerializeField]
    Transform _endCameraTrans = default;

    [Tooltip("カメラのアニメーションの種類")]
    [SerializeField]
    Ease _cameraEase = default;

    [Header("Objects")]
    [Tooltip("クイズコントローラー")]
    [SerializeField]
    QuizController _quizCtrl = default;

    [Tooltip("Objectを管理するマネージャー")]
    [SerializeField]
    ObjectManager _objectMng = default;

    [Tooltip("最初の演出用のトランプ兵")]
    [SerializeField]
    Transform _directionTrumpSoldier = default;

    [Header("UI")]
    [SerializeField]
    Text _informationText = default;
    #endregion
    #region private
    #endregion
    #region public
    public override event Action GameSetUp;
    public override event Action GameStart;
    public override event Action GamePause;
    public override event Action GameEnd;
    #endregion
    #region property
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
        _informationText.text = "スタート!";

        yield return new WaitForSeconds(1.5f);

        //yield return StartCoroutine(); QuizManagerのお題表示処理
        
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

        for (int i = 0; i < _quizNum; i++)
        {
            isAnswerPhase = false;
            OnGameSetUp();

            if (i > 0)
            {
                TransitionManager.FadeIn(FadeType.Normal, () =>
                {
                    _cameraTrans.DOMoveX(_startCameraPos.position.x, 0f);
                    TransitionManager.FadeOut(FadeType.Normal, () =>
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
                //ゲームの配置のセットアップ処理をここに記述
                Viewing(() =>
                {
                    isAnswerPhase = true;
                });
            }
            yield return new WaitUntil(() => isAnswerPhase); //選択肢が表示されるのを待機

            yield return StartCoroutine(_quizCtrl.OnChoicePhaseCoroutine(_objectMng, (QuizType)i));
        }
    }
    void Viewing(Action action = null)
    {
        //ゲームの配置のセットアップ処理をここに記述
        _cameraTrans.DOMoveX(_endCameraTrans.position.x, _viewingTime)
                    .SetEase(_cameraEase)
                    .OnComplete(() =>
                    {
                        action?.Invoke();
                        Debug.Log("クイズ表示");
                    });
    }

    public override void OnGameSetUp()
    {
        GameSetUp?.Invoke();
    }
}
