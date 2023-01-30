using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Cinemachine;
using AliceProject;
/// <summary>
/// �����Q�[���̊Ǘ����s���}�l�[�W���[�N���X
/// </summary>
public class FallGameManager : MonoBehaviour
{
    #region selialize
    [Header("Variable")]
    [Tooltip("�ő�HP")]
    [SerializeField]
    int _maxHP = 3;

    [Tooltip("�e��Փx�̃p�����[�^�[")]
    [SerializeField]
    FallGameParameter[] _gamePrameters = default;

    [Header("Objects")]
    [SerializeField]
    Transform _playerTrans = default;

    [SerializeField]
    Transform _startTrans = default;

    [SerializeField]
    CanvasGroup _inGamePanel = default;

    [SerializeField]
    Text _informationText = default;

    [SerializeField]
    Image[] _infoImages = default;

    [SerializeField]
    CinemachineVirtualCamera _finishCamera = default;

    [Header("Debug")]
    [SerializeField]
    bool _debugMode = false;
    #endregion

    #region private
    /// <summary> �ڕW�̖��� </summary>
    int _targetCount;
    Vector3 _originPos;
    #endregion

    #region events
    public event Action GameStart;
    public event Action<IEffectable> GetItem;
    public event Action GamePause;
    public event Action GameEnd;
    public event Action ClearDirection;
    #endregion

    #region property
    public static FallGameManager Instance { get; private set; }
    public static bool IsSecondTry { get; set; } = false;
    public int TargetCount => _targetCount;
    public int MaxHP => _maxHP;
    #endregion

    private void Awake()
    {
        Instance = this;

        if (_debugMode)
        {
            IsSecondTry = _debugMode;
        }

    }
    private void Start()
    {
        _originPos = _playerTrans.position;
        _inGamePanel.alpha = 0;

        AudioManager.PlayBGM(BGMType.Stage1);
        GameManager.UpdateCurrentStage(Stages.Stage1);

        HPManager.Instance.ChangeHPValue(_maxHP, true); //HPManager�ɍő�HP�̒l��o�^
        HPManager.Instance.LostHpAction += OnGameOver; //�Q�[���I�[�o�[���̏�����HPManager�ɓo�^

        OnGameStart();

        LetterboxController.ActivateLetterbox(true, 0f);
        TransitionManager.FadeOut(FadeType.Normal);
    }

    public void OnGameStart()
    {
        Init();
        _playerTrans.DOMove(_startTrans.position, 2.0f)
                    .OnComplete(() =>
                    {
                        StartCoroutine(GameStartCoroutine(() => GameStart?.Invoke()));
                        Debug.Log("�Q�[���J�n");
                    });
    }

    public void OnGetItem(IEffectable effect)
    {
        GetItem?.Invoke(effect);
    }

    /// <summary>
    /// �_���[�W���󂯂鏈��
    /// </summary>
    /// <param name="damageValue"> �󂯂��_���[�W�̒l </param>
    public void OnDamage(int damageValue)
    {
        HPManager.Instance.ChangeHPValue(damageValue);
        Debug.Log("�_���[�W���󂯂�");
    }

    /// <summary>
    /// HP���񕜂��鏈��
    /// </summary>
    /// <param name="healValue"> �񕜂���l </param>
    public void OnHeal(int healValue)
    {
        HPManager.Instance.ChangeHPValue(healValue, true);
        Debug.Log("HP����");
    }

    public void OnGameEnd()
    {
        GameEnd?.Invoke();
        Debug.Log("�Q�[���I��");
        AudioManager.StopBGM();
        AudioManager.PlaySE(SEType.Stage1_Fall);
        StartCoroutine(GameEndCoroutine());
    }

    /// <summary>
    /// �Q�[���I�[�o�[�̏��������s
    /// </summary>
    void OnGameOver()
    {
        //���r�[���璧�킵�Ă��Ȃ��ꍇ
        if (!IsSecondTry)
        {
            TransitionManager.SceneTransition(SceneType.Stage1_Fall);
        }
        else
        {
            GameEnd?.Invoke();
            GameoverDirection.Instance.OnGameoverDirection();
        }
    }


    /// <summary>
    /// ����������
    /// </summary>
    void Init()
    {
        _playerTrans.position = _originPos;
        _infoImages[0].enabled = false;
        _infoImages[1].enabled = false;
        _informationText.text = "";

        var diffIndex = (int)GameManager.Instance.CurrentGameDifficultyType;

        _targetCount = _gamePrameters[diffIndex].TargetCount;
        ObstacleGenerator.Instance.SetInterval(_gamePrameters[diffIndex].ObstacleGenerateInterval);
        TableGenerator.Instance.SetInterval(_gamePrameters[diffIndex].TableGenerateInterval);
        WhitePaperGenerator.Instance.SetInterval(_gamePrameters[diffIndex].WhitePaperGenerateInterval);
    }

    IEnumerator GameStartCoroutine(Action action = null)
    {
        LetterboxController.ActivateLetterbox(false, 1.5f);
        yield return new WaitForSeconds(1.5f);

        //���߂ăv���C���鎞�̓��b�Z�[�W��\��
        if (GameManager.Instance.IsFirstVisitCurrentStage)
        {
            yield return MessagePlayer.Instance.PlayMessageCorountine(MessageType.FirstVisit_Stage1);
            yield return new WaitForSeconds(1.5f);
        }
        _infoImages[0].enabled = true;
        //_informationText.text = "�X�^�[�g!";

        yield return new WaitForSeconds(1.5f);

        action?.Invoke();
        _infoImages[0].enabled = false;
        //_informationText.text = "";
        _inGamePanel.alpha = 1;
    }

    IEnumerator GameEndCoroutine()
    {
        TransitionManager.FadeIn(FadeType.Normal, action: () =>
         {
             _inGamePanel.alpha = 0;
             TransitionManager.FadeOut(FadeType.Normal);
         });
        _informationText.gameObject.transform.DOLocalMoveY(300, 0f);

        yield return new WaitForSeconds(4.5f);

        _finishCamera.Priority = 12;

        yield return new WaitForSeconds(2f);

        ClearDirection?.Invoke();
        _infoImages[1].enabled = true;
        AudioManager.PlayBGM(BGMType.ClearJingle, false);
        //_informationText.text = "�X�e�[�W�N���A!";

        yield return new WaitForSeconds(4.0f);

        GameManager.SaveStageResult(true);

        _infoImages[1].enabled = false;
        //_informationText.text = "";

        yield return GameManager.GetStillDirectionCoroutine(Stages.Stage1, MessageType.GetStill_Stage1);

        GameManager.UpdateFirstVisit(Stages.Stage1);
        TransitionManager.FadeIn(FadeType.Black_TransParent, 0f);
        TransitionManager.SceneTransition(SceneType.Lobby);
        IsSecondTry = true;
    }
}

/// <summary>
/// �����Q�[���̊e��Փx�̐��l
/// </summary>
[Serializable]
public struct FallGameParameter
{
    public string ParamName;
    public DifficultyType DifficultyType;
    public int TargetCount;
    public float ObstacleGenerateInterval;
    public float TableGenerateInterval;
    public float WhitePaperGenerateInterval;
}
