using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �����Q�[���̊Ǘ����s���}�l�[�W���[�N���X
/// </summary>
public class FallGameManager : MonoBehaviour
{
    #region selialize
    #endregion

    #region private
    bool _inGame = false;
    #endregion

    #region property
    public static FallGameManager Instance { get; private set; }
    public Action GameStart { get; set; }
    public Action GamePause { get; set; }
    public Action GameEnd { get; set; }
    #endregion

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        StartCoroutine(InGame());
    }

    IEnumerator InGame()
    {
        yield return new WaitForSeconds(2.0f);

        GameStart?.Invoke();
        Debug.Log("�Q�[���J�n");

        _inGame = true;

        while (_inGame)
        {
            yield return null;
        }

        GameEnd?.Invoke();
        Debug.Log("�Q�[���I��");
    }
}
