using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

/// <summary>
/// クイズの機能を持つController
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
    /// 解答を選択するフェイズのコルーチン
    /// </summary>
    /// <param name="manager"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public IEnumerator OnChoicePhaseCoroutine(ObjectManager manager, QuizType type)
    {
        //選択状態のフラグをリセット
        _isChoiced = false;
        _choicePanel.SetActive(true);
        float timer = 0;

        switch (type)
        {
            case QuizType.RedRose:
                _questionText.text = "赤いバラの数は？";
                _currentAnswerValue = manager.CurrentRedRoseCount;
                break;
            case QuizType.WhiteRose:
                _questionText.text = "白いバラの数は？";
                _currentAnswerValue = manager.CurrentWhiteRoseCount;
                break;
            case QuizType.RedAndWhiteRose:
                _questionText.text = "赤いバラと白いバラの合計は？";
                _currentAnswerValue = manager.CurrentRedRoseCount + manager.CurrentWhiteRoseCount;
                break;
            case QuizType.TrumpSolder:
                _questionText.text = "トランプ兵の数は？";
                //_currentAnswerValue 
                break;
            case QuizType.All:
                _questionText.text = "バラとトランプ兵の合計は？";
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
            _questionText.text = "正解！";
        }
        else
        {
            _questionText.text = $"不正解… 正解は{_currentAnswerValue}";
        }

        yield return new WaitForSeconds(3.0f);

        _questionText.text = "";
        _AnswerTimeText.text = "";
        _choicePanel.SetActive(false);
    }

    /// <summary>
    /// 解答の判定
    /// </summary>
    /// <returns> 判定結果 </returns>
    public bool Judge()
    {
        if (!_isChoiced) return false; //時間内に選択していなかったら不正解

        return _currentAnswerValue == _playerChoiceValue; //正解と解答を比較した真偽を返す
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
                QuizType.RedRose => $"{sort[i]} <size=80>本</size>",
                QuizType.WhiteRose => $"{sort[i]} <size=80>本</size>",
                QuizType.RedAndWhiteRose => $"{sort[i]} <size=80>本</size>",
                QuizType.TrumpSolder => $"{sort[i]} <size=80>人</size>",
                QuizType.All => $"{sort[i]} <size=80>つ</size>",
                _ => $"{sort[i]} <size=80>本</size>",
            };
        }

    }
    bool CheckValue(int[] values)
    {
        //正解が含まれているか確認
        var result = values.Contains(_currentAnswerValue);

        //正解の値を含んでいた場合は確認処理を行う
        if (result)
        {
            for (int i = 0; i < values.Length; i++)
            {
                for (int n = 0; n < values.Length; n++)
                {
                    //同じ添え字の場合は処理を飛ばす
                    if (i == n)
                    {
                        continue;
                    }

                    //同じ値が存在する場合はfalse
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
/// クイズの種類
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
