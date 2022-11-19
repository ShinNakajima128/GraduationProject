using System;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// �I�u�W�F�N�g�̑I��������@�\
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
        // �A�N�V�����}�b�v�̕ύX
        _input.SwitchCurrentActionMap("Stage2");

        foreach (var item in _selectIcons)
        {
            item.SetActive(false);
        }
    }

    /// <summary>
    /// �o�^
    /// </summary>
    private void Register()
    {
        _input.actions["SelectUp"].started += Increment;
        _input.actions["SelectDown"].started += Decrement;
        _input.actions["Enter"].started += Enter;
    }

    /// <summary>
    /// �I���J�n
    /// </summary>
    public void Begin()
    {
        _currentSelectNum = 0;
        _canSelected = true;
        _selectIcons[0].SetActive(true);
    }

    /// <summary>
    /// �I���I��
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
    /// �I���̍X�V
    /// </summary>
    private void Increment(InputAction.CallbackContext callback)
    {
        if (_canSelected)
            if (callback.started)
            {
                // ���݂̃A�C�R�����\����
                _selectIcons[_currentSelectNum].SetActive(false);
                // ���ݑI�����Ă���A�C�R����ύX
                _currentSelectNum++;
                if (_currentSelectNum > 5)
                {
                    _currentSelectNum = 0;
                }
                // �I���A�C�R���̍X�V
                _selectIcons[_currentSelectNum].SetActive(true);
            }
    }

    /// <summary>
    /// �I���̍X�V
    /// </summary>
    private void Decrement(InputAction.CallbackContext callback)
    {
        if (_canSelected)
            if (callback.started)
            {
                // ���݂̃A�C�R�����\����
                _selectIcons[_currentSelectNum].SetActive(false);
                // ���ݑI�����Ă���A�C�R����ύX
                _currentSelectNum--;
                if (_currentSelectNum < 0)
                {
                    _currentSelectNum = 5;
                }
                // �I���A�C�R���̍X�V
                _selectIcons[_currentSelectNum].SetActive(true);
            }
    }

    /// <summary>
    /// ���ݑI�����Ă��锠�̏��𑗂�
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
