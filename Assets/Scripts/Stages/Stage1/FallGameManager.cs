using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// �����Q�[���̊Ǘ����s���}�l�[�W���[�N���X
/// </summary>
public class FallGameManager : MonoBehaviour
{
    #region selialize
    [SerializeField]
    Transform _PlayerTrans = default;

    [SerializeField]
    Transform _startTrans = default;
    #endregion

    #region private
    Vector3 _originPos;
    bool _inGame = false;
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
        _originPos = _PlayerTrans.position;
        OnGameStart();
    }

    
    public void OnGameStart()
    {
        Init();
        _PlayerTrans.DOMove(_startTrans.position, 2.0f)
                    .OnComplete(() =>
                    {
                        GameStart?.Invoke();
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
    }

    void Init()
    {
        _PlayerTrans.position = _originPos;
    }
}
