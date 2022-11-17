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

    #region property
    public static FallGameManager Instance { get; private set; }
    public Action GameStart { get; set; }
    public Action<IEffectable> GetItem { get; set; }
    public Action GamePause { get; set; }
    public Action GameEnd { get; set; }
    #endregion

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        _originPos = _playerTrans.position;
        OnGameStart();
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
        TransitionManager.FadeIn(FadeType.Normal, () => 
        {
            _inGamePanel.SetActive(false);
            TransitionManager.FadeOut(FadeType.Normal);
        });
        yield return new WaitForSeconds(4.0f);
        _finishCamera.Priority = 12;
        yield return new WaitForSeconds(2.5f);

        _informationText.text = "�X�e�[�W�N���A!";

        yield return new WaitForSeconds(4.0f);

        //_informationText.text = "";

        TransitionManager.SceneTransition(SceneType.Lobby);
    }
}
