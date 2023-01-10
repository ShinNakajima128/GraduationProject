using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeacupManager : MonoBehaviour
{
    #region serialize
    [Header("Objects")]
    [SerializeField]
    Teacup[] _teacups = default;

    [SerializeField]
    DorMouse _mouse = default;

    [Header("UI")]
    [SerializeField]
    GameObject _selectButtonInfo = default;

    [Header("Components")]
    [SerializeField]
    TeacupSelecter _selecter = default;
    #endregion

    #region private
    bool _isChoiced = false;
    bool _isCorrect = false;
    int _selectIndex;
    #endregion

    #region public
    #endregion

    #region property
    public Teacup[] Teacups { get => _teacups; set => _teacups = value; }
    #endregion

    void Start()
    {
        ButtonSetup();
        _selectButtonInfo.SetActive(false);
    }

    /// <summary>
    /// �����_���Ńl�Y�~���ړ�����
    /// </summary>
    public void RandomHideMouse()
    {
        foreach (var t in _teacups)
        {
            t.IsInMouse = false;
        }

        int index = UnityEngine.Random.Range(0, _teacups.Length);

        _mouse.transform.position = _teacups[index].MousePos;
        _mouse.transform.SetParent(_teacups[index].transform);
        _teacups[index].IsInMouse = true;

    }

    public void AllCupDown()
    {
        foreach (var c in _teacups)
        {
            c.DownCup();
        }
    }

    public void AllCupOpen()
    {
        foreach (var c in _teacups)
        {
            c.UpCup();
        }
    }

    public void SelectCupOpen(int index)
    {
        _teacups[index].UpCup();
    }

    public void OnMouseAnimation(MouseState state, float animTime = 0.3f)
    {
        _mouse.OnAnimation(state, animTime);

    }

    public IEnumerator ChoicePhaseCoroutine(Action<bool, int> callback)
    {
        _selecter.SelectButtons[0].SelectButton.Select();
        _selectButtonInfo.SetActive(true);

        yield return new WaitUntil(() => _isChoiced);

        _selectButtonInfo.SetActive(false);
        callback?.Invoke(_isCorrect, _selectIndex);
        _isChoiced = false;

        yield return null;
    }

    void ButtonSetup()
    {
        _selecter.SelectButtons[0].SelectButton.onClick.AddListener(() =>
        {
            _isChoiced = true;

            Debug.Log(0);
            //�I�������J�b�v�Ƀl�Y�~�������琳�𔻒�Ƃ���
            if (_teacups[0].IsInMouse)
            {
                _isCorrect = true;
                _selectIndex = 0;
                Debug.Log("����");
            }
            else
            {
                _isCorrect = false;
                _selectIndex = 0;
                Debug.Log("�s����");
            }
        });
        _selecter.SelectButtons[1].SelectButton.onClick.AddListener(() =>
        {
            _isChoiced = true;

            Debug.Log(1);
            //�I�������J�b�v�Ƀl�Y�~�������琳�𔻒�Ƃ���
            if (_teacups[1].IsInMouse)
            {
                _isCorrect = true;
                _selectIndex = 1;
                Debug.Log("����");
            }
            else
            {
                _isCorrect = false;
                _selectIndex = 1;
                Debug.Log("�s����");
            }
        });
        _selecter.SelectButtons[2].SelectButton.onClick.AddListener(() =>
        {
            _isChoiced = true;

            Debug.Log(2);
            //�I�������J�b�v�Ƀl�Y�~�������琳�𔻒�Ƃ���
            if (_teacups[2].IsInMouse)
            {
                _isCorrect = true;
                _selectIndex = 2;
                Debug.Log("����");
            }
            else
            {
                _isCorrect = false;
                _selectIndex = 2;
                Debug.Log("�s����");
            }
        });
        _selecter.SelectButtons[3].SelectButton.onClick.AddListener(() =>
        {
            _isChoiced = true;

            Debug.Log(3);
            //�I�������J�b�v�Ƀl�Y�~�������琳�𔻒�Ƃ���
            if (_teacups[3].IsInMouse)
            {
                _isCorrect = true;
                _selectIndex = 3;
                Debug.Log("����");
            }
            else
            {
                _isCorrect = false;
                _selectIndex = 3;
                Debug.Log("�s����");
            }
        });
        _selecter.SelectButtons[4].SelectButton.onClick.AddListener(() =>
        {
            _isChoiced = true;

            Debug.Log(4);
            //�I�������J�b�v�Ƀl�Y�~�������琳�𔻒�Ƃ���
            if (_teacups[4].IsInMouse)
            {
                _isCorrect = true;
                _selectIndex = 4;
                Debug.Log("����");
            }
            else
            {
                _isCorrect = false;
                _selectIndex = 4;
                Debug.Log("�s����");
            }
        });
        _selecter.SelectButtons[5].SelectButton.onClick.AddListener(() =>
        {
            _isChoiced = true;

            Debug.Log(5);
            //�I�������J�b�v�Ƀl�Y�~�������琳�𔻒�Ƃ���
            if (_teacups[5].IsInMouse)
            {
                _isCorrect = true;
                _selectIndex = 5;
                Debug.Log("����");
            }
            else
            {
                _isCorrect = false;
                _selectIndex = 5;
                Debug.Log("�s����");
            }
        });

        //for���ŃZ�b�g���悤�Ƃ���Ɛ������������s���Ȃ�����
        //for (int i = 0; i < _selecter.SelectButtons.Length; i++)
        //{
        //    _selecter.SelectButtons[i].SelectButton.onClick.AddListener(() =>
        //    {
        //        int index = i;
        //        _isChoiced = true;

        //        Debug.Log(index);

        //        //�I�������J�b�v�Ƀl�Y�~�������琳�𔻒�Ƃ���
        //        if (_teacups[index].IsInMouse)
        //        {
        //            _isCorrect = true;
        //            _selectIndex = index;
        //            Debug.Log("����");
        //        }
        //        else
        //        {
        //            _isCorrect = false;
        //            _selectIndex = index;
        //            Debug.Log("�s����");
        //        }
        //    });
        //}
    }
}
