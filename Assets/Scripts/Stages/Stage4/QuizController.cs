using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UniRx;

/// <summary>
/// �N�C�Y�̋@�\������Controller
/// </summary>
public class QuizController : MonoBehaviour
{
    #region serialize
    [Header("Variable")]
    [Tooltip("�I������I�Ԏ���")]
    [SerializeField]
    int _answerTime = 10;

    [Tooltip("���ʕ\����A���̃t�F�C�Y�ɐi�ނ܂ł̎���")]
    [SerializeField]
    float _goToNextPhaseInterval = 2.5f;

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
            //�{�^���ɃJ�[�\�������������̃C�x���g��o�^
            b.Button.gameObject.TryGetComponent<EventTrigger>(out var trigger);

            var selectEntry = new EventTrigger.Entry();
            selectEntry.eventID = EventTriggerType.Select;

            selectEntry.callback.AddListener(eventData => 
            {
                _playerChoiceValue = b.ChoiceAnswerValue;
            });
            trigger.triggers.Add(selectEntry);

            //�{�^���I�����̏�����o�^
            b.Button.onClick.AddListener(() =>
            {
                _isChoiced = true;
            });
        }
        EventSystem.current.firstSelectedGameObject.GetComponent<Button>().Select();
        _choicePanel.SetActive(false);
    }

    /// <summary>
    /// �𓚂�I������t�F�C�Y�̃R���[�`��
    /// </summary>
    /// <param name="manager"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public IEnumerator OnChoicePhaseCoroutine(ObjectManager manager, QuizType type, Action<int> callback)
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
                var randomTrumpType = (Stage4TrumpType)UnityEngine.Random.Range(0, 2);

                switch (randomTrumpType)
                {
                    case Stage4TrumpType.Trump_Red:
                        _questionText.text = "�Ԃ��g�����v���̐��́H";
                        break;
                    case Stage4TrumpType.Trump_Black:
                        _questionText.text = "�����g�����v���̐��́H";
                        break;
                    default:
                        break;
                }
                
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
            callback?.Invoke(1);
        }
        else
        {
            _questionText.text = $"�s�����c ������{_currentAnswerValue}";
            callback?.Invoke(0);
        }

        yield return new WaitForSeconds(_goToNextPhaseInterval);

        _questionText.text = "";
        _AnswerTimeText.text = "";
        EventSystem.current.firstSelectedGameObject.GetComponent<Button>().Select();
        _choicePanel.SetActive(false);
    }

    /// <summary>
    /// �𓚂̔���
    /// </summary>
    /// <returns> ���茋�� </returns>
    public bool Judge()
    {
        //if (!_isChoiced) return false; //���ԓ��ɑI�����Ă��Ȃ�������s����

        Debug.Log($"����:{_currentAnswerValue}, �v���C���[�̉�:{_playerChoiceValue}");
        return _currentAnswerValue == _playerChoiceValue; //�����Ɖ𓚂��r�����^�U��Ԃ�
    }

    /// <summary>
    /// �I�����ƂȂ�̊e�{�^���̐ݒ���s��
    /// </summary>
    /// <param name="type"> ���̎�� </param>
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
                    //�����Y�����̏ꍇ�͏������X�L�b�v
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
    public Text ChoiceText;
    public int ChoiceAnswerValue;
}
