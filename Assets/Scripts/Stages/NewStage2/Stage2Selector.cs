using System;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// オブジェクトの選択をする機能
/// </summary>
public class Stage2Selector : MonoBehaviour
{
    [SerializeField]
    private PlayerInput _input;

    [SerializeField]
    private GameObject[] _selectIcons;

    [SerializeField]
    private Stage2SelectSender _sender;

    private int _currentSelectNum = 0;

    public bool _canSelected { get; private set; } = false;

    private void Awake()
    {
        Intialize();
        Register();
    }

    private void Intialize()
    {
        // アクションマップの変更
        _input.SwitchCurrentActionMap("Stage2");

        foreach (var item in _selectIcons)
        {
            item.SetActive(false);
        }
    }

    /// <summary>
    /// 登録
    /// </summary>
    private void Register()
    {
        _input.actions["SelectUp"].started += Increment;
        _input.actions["SelectDown"].started += Decrement;
        _input.actions["Enter"].started += Enter;
    }

    /// <summary>
    /// 選択開始
    /// </summary>
    public void Begin()
    {
        _currentSelectNum = 0;
        _canSelected = true;
        _selectIcons[0].SetActive(true);
    }

    /// <summary>
    /// 選択終了
    /// </summary>
    public void Stop()
    {
        _canSelected = false;
        foreach (var item in _selectIcons)
        {
            item.SetActive(false);
        }
        _currentSelectNum = 0;
    }

    /// <summary>
    /// 選択の更新
    /// </summary>
    private void Increment(InputAction.CallbackContext callback)
    {
        if (_canSelected)
            if (callback.started)
            {
                // 現在のアイコンを非表示に
                _selectIcons[_currentSelectNum].SetActive(false);
                // 現在選択しているアイコンを変更
                _currentSelectNum++;
                if (_currentSelectNum > 5)
                {
                    _currentSelectNum = 0;
                }
                // 選択アイコンの更新
                _selectIcons[_currentSelectNum].SetActive(true);
            }
    }

    /// <summary>
    /// 選択の更新
    /// </summary>
    private void Decrement(InputAction.CallbackContext callback)
    {
        if (_canSelected)
            if (callback.started)
            {
                // 現在のアイコンを非表示に
                _selectIcons[_currentSelectNum].SetActive(false);
                // 現在選択しているアイコンを変更
                _currentSelectNum--;
                if (_currentSelectNum < 0)
                {
                    _currentSelectNum = 5;
                }
                // 選択アイコンの更新
                _selectIcons[_currentSelectNum].SetActive(true);
            }
    }

    /// <summary>
    /// 現在選択している箱の情報を送る
    /// </summary>
    private void Enter(InputAction.CallbackContext callback)
    {
        if (_canSelected)
            if (callback.started)
            {
                _sender.SendSelectNumber(_currentSelectNum);
            }
    }
}
