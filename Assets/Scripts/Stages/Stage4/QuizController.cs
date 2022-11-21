using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

/// <summary>
/// �N�C�Y�̋@�\������Controller
/// </summary>
public class QuizController : MonoBehaviour
{
    #region serialize
    [SerializeField]
    int _answerTime = 10;

    [Header("UI")]
    [SerializeField]
    GameObject _choicePanel = default;

    [SerializeField]
    List<ChoiceButton> _choiceButtonList = new List<ChoiceButton>();

    [SerializeField]
    Text _questionText = default;

    [SerializeField]
    Text _AnswerTimeText = default;
    #endregion
    #region private
    int _currentAnswerValue = 0;
    int _playerChoiceValue = 0;
    bool _isChoiced = false;
    #endregion
    #region property
    #endregion

    private void Start()
    {
        foreach (var b in _choiceButtonList)
        {
            b.Button.onClick.AddListener(() =>
            {
                _playerChoiceValue = b.ChoiceAnswerValue;
                _isChoiced = true;
            });
        }
    }

    /// <summary>
    /// �𓚂�I������t�F�C�Y�̃R���[�`��
    /// </summary>
    /// <param name="manager"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public IEnumerator OnChoicePhaseCoroutine(ObjectManager manager, QuizType type)
    {
        //�I����Ԃ̃t���O�����Z�b�g
        _isChoiced = false;
        _choicePanel.SetActive(true);
        float timer = 0;

        switch (type)
        {
            case QuizType.RedRose:
                _questionText.text = "�Ԃ��o���̐��́H";
                _currentAnswerValue = manager.CurrentRedRoseCount;
                break;
            case QuizType.WhiteRose:
                _questionText.text = "�����o���̐��́H";
                _currentAnswerValue = manager.CurrentWhiteRoseCount;
                break;
            case QuizType.RedAndWhiteRose:
                _questionText.text = "�Ԃ��o���Ɣ����o���̍��v�́H";
                _currentAnswerValue = manager.CurrentRedRoseCount + manager.CurrentWhiteRoseCount;
                break;
            case QuizType.TrumpSolder:
                _questionText.text = "�g�����v���̐��́H";
                //_currentAnswerValue 
                break;
            case QuizType.All:
                _questionText.text = "�o���ƃg�����v���̍��v�́H";
                break;
            default:
                break;
        }

        ChoiceButtonSetUp(type);

        while (timer < _answerTime && !_isChoiced)
        {
            _AnswerTimeText.text = (_answerTime - timer).ToString("F0");
            timer += Time.deltaTime;
            yield return null;
        }

        if (Judge())
        {
            _questionText.text = "�����I";
        }
        else
        {
            _questionText.text = $"�s�����c ������{_currentAnswerValue}";
        }

        yield return new WaitForSeconds(3.0f);

        _questionText.text = "";
        _AnswerTimeText.text = "";
        _choicePanel.SetActive(false);
    }

    /// <summary>
    /// �𓚂̔���
    /// </summary>
    /// <returns> ���茋�� </returns>
    public bool Judge()
    {
        if (!_isChoiced) return false; //���ԓ��ɑI�����Ă��Ȃ�������s����

        return _currentAnswerValue == _playerChoiceValue; //�����Ɖ𓚂��r�����^�U��Ԃ�
    }

    void ChoiceButtonSetUp(QuizType type)
    {
        int[] answerValues = new int[_choiceButtonList.Count];
        int min;
        int max;

        while (!CheckValue(answerValues))
        {
            min = UnityEngine.Random.Range(_currentAnswerValue - 5, _currentAnswerValue);

            if (min < 1)
            {
                continue;
            }
            max = UnityEngine.Random.Range(_currentAnswerValue + 1, _currentAnswerValue + 6);

            for (int i = 0; i < _choiceButtonList.Count; i++)
            {
                answerValues[i] = UnityEngine.Random.Range(min, max + 1);
            }
        }

        var sort = answerValues.OrderBy(x => x).ToArray();

        for (int i = 0; i < _choiceButtonList.Count; i++)
        {
            _choiceButtonList[i].ChoiceAnswerValue = sort[i];
            _choiceButtonList[i].ChoiceText.text = type switch
            {
                QuizType.RedRose => $"{sort[i]} <size=80>�{</size>",
                QuizType.WhiteRose => $"{sort[i]} <size=80>�{</size>",
                QuizType.RedAndWhiteRose => $"{sort[i]} <size=80>�{</size>",
                QuizType.TrumpSolder => $"{sort[i]} <size=80>�l</size>",
                QuizType.All => $"{sort[i]} <size=80>��</size>",
                _ => $"{sort[i]} <size=80>�{</size>",
            };
        }

    }
    bool CheckValue(int[] values)
    {
        //�������܂܂�Ă��邩�m�F
        var result = values.Contains(_currentAnswerValue);

        //�����̒l���܂�ł����ꍇ�͊m�F�������s��
        if (result)
        {
            for (int i = 0; i < values.Length; i++)
            {
                for (int n = 0; n < values.Length; n++)
                {
                    //�����Y�����̏ꍇ�͏������΂�
                    if (i == n)
                    {
                        continue;
                    }

                    //�����l�����݂���ꍇ��false
                    if (values[i] == values[n])
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        else
        {
            return false;
        }
    }
}

/// <summary>
/// �N�C�Y�̎��
/// </summary>
public enum QuizType
{
    RedRose,
    WhiteRose,
    RedAndWhiteRose,
    TrumpSolder,
    All
}
[Serializable]
public class ChoiceButton
{
    public Button Button;
    public GamepadButtonType GamepadButtonType;
    public Text ChoiceText;
    public int ChoiceAnswerValue;

    public bool IsButtonInputed 
    { 
        get 
        {
            return GamepadButtonType switch
            {
                GamepadButtonType.A => UIInput.A,
                GamepadButtonType.B => UIInput.B,
                GamepadButtonType.X => UIInput.X,
                GamepadButtonType.Y => UIInput.Y,
                _ => false,
            };
        } 
    }
}
