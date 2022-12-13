using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Cinemachine;

/// <summary>
/// �����Q�[���̊Ǘ����s���}�l�[�W���[�N���X
/// </summary>
public class FallGameManager : MonoBehaviour
{
    #region selialize
    [Header("Variable")]
    [SerializeField]
    int _targetCount = 10;

    [Header("Objects")]
    [SerializeField]
    Transform _playerTrans = default;

    [SerializeField]
    Transform _startTrans = default;

    [SerializeField]
    GameObject _inGamePanel = default;

    [SerializeField]
    Text _informationText = default;

    [SerializeField]
    MessagePlayer _player = default;

    [SerializeField]
    CinemachineVirtualCamera _finishCamera = default;
    #endregion

    #region private
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
    public int TargetCount => _targetCount;
    #endregion

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        _originPos = _playerTrans.position;
        GameManager.UpdateCurrentStage(Stages.Stage1);
        OnGameStart();
        TransitionManager.FadeIn(FadeType.White, 0f);
        TransitionManager.FadeOut(FadeType.Normal, action: () =>
        {
            TransitionManager.FadeIn(FadeType.Black, 0f, action:() =>
            {
                TransitionManager.FadeOut(FadeType.Normal, 0f);
            });
        });
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

    public void OnGameEnd()
    {
        GameEnd?.Invoke();
        Debug.Log("�Q�[���I��");
        StartCoroutine(GameEndCoroutine());
    }

    void Init()
    {
        _playerTrans.position = _originPos;
        _informationText.text = "";
    }

    IEnumerator GameStartCoroutine(Action action = null)
    {
        _informationText.text = "�X�^�[�g!";

        yield return new WaitForSeconds(1.5f);

        action?.Invoke();
        _informationText.text = "";
    }

    IEnumerator GameEndCoroutine()
    {
        TransitionManager.FadeIn(FadeType.Normal, action: () =>
         {
             _inGamePanel.SetActive(false);
             TransitionManager.FadeOut(FadeType.Normal);
         });
        _informationText.gameObject.transform.DOLocalMoveY(300, 0f);
        yield return new WaitForSeconds(4.5f);
        _finishCamera.Priority = 12;
        yield return new WaitForSeconds(2f);

        ClearDirection?.Invoke();
        _informationText.text = "�X�e�[�W�N���A!";

        yield return new WaitForSeconds(4.0f);

        GameManager.SaveStageResult(true);
        TransitionManager.SceneTransition(SceneType.Lobby);
    }
}
