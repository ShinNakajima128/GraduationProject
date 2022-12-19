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

    [Header("Components")]
    [SerializeField]
    QueenOrder _order = default;

    [SerializeField]
    CroquetGameUI _gameUI = default;

    [SerializeField]
    CroquetCameraManager _cameraMng = default;
    #endregion

    #region private
    TrumpColorType _currentTargetTrumpColor;
    int _currentTargetStrikeNum;
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
        base.Start();
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
        OrderData data = default;

        for (int i = 0; i < _playCount; i++)
        {
            data = _order.Data[i];

            _currentTargetTrumpColor = data.TargetTrumpColor;
            _currentTargetStrikeNum = data.TargetNum;
            yield return _order.LetterAnimationCoroutine(i + 1, data.ToString());

            _cameraMng.ChangeCamera(CroquetCameraType.View, 3.0f);

            yield return new WaitForSeconds(3.0f);

            TransitionManager.FadeIn(FadeType.Normal, action: () =>
            {
                _cameraMng.ChangeCamera(CroquetCameraType.InGame, 0f);
                TransitionManager.FadeOut(FadeType.Normal);
            });

            yield return new WaitForSeconds(3.0f);


            //�����F�����Ƀn���l�Y�~���S�[������܂ŏ�����ҋ@���鏈�����L�q


            yield return new WaitUntil(() => UIInput.Submit);

            if (i != _playCount)
            {
                TransitionManager.FadeIn(FadeType.Normal, action: () =>
                {
                    _cameraMng.ChangeCamera(CroquetCameraType.Order, 0f);
                    TransitionManager.FadeOut(FadeType.Normal);
                });
                
                yield return new WaitForSeconds(3.5f);
            }
            else
            {

            }
        }
    }

    /// <summary>
    /// �n���l�Y�~���S�[���n�_�ɒ��������̃R���[�`��
    /// </summary>
    /// <param name="result"> �ł������� </param>
    /// <returns></returns>
    IEnumerator GoalDirectionCoroutine(bool result)
    {
        yield return null;
    }

    protected override void Init()
    {
        throw new NotImplementedException();
    }
}
/// <summary>
/// �N���b�P�[�Q�[���̃X�e�[�^�X
/// </summary>
public enum CroquetGameState
{
    Order,
    InGame,
    GoalDirection
}
