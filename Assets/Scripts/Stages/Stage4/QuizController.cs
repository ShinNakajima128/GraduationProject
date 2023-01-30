using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UniRx;

/// <summary>
/// クイズの機能を持つController
/// </summary>
public class QuizController : MonoBehaviour
{
    #region serialize
    [Header("Variable")]
    [Tooltip("選択肢を選ぶ時間")]
    [SerializeField]
    int _answerTime = 10;

    [Tooltip("結果表示後、次のフェイズに進むまでの時間")]
    [SerializeField]
    float _goToNextPhaseInterval = 2.5f;

    [Header("UI")]
    [SerializeField]
    CanvasGroup _choicePanel = default;

    [SerializeField]
    List<ChoiceButton> _choiceButtonList = new List<ChoiceButton>();

    [SerializeField]
    Text _questionText = default;

    [SerializeField]
    Text _AnswerTimeText = default;

    [Tooltip("正解のImage")]
    [SerializeField]
    Image _correctImage = default;

    [Tooltip("不正解のImage")]
    [SerializeField]
    Image _wrongImage = default;

    [Header("Debug")]
    [SerializeField]
    bool _debugMode = false;
    #endregion

    #region private
    int _currentAnswerValue = 0;
    int _playerChoiceValue = 0;
    int _currentButtonIndex = 0;
    bool _isChoiced = false;
    #endregion
    #region property
    #endregion

    private void Start()
    {
        foreach (var b in _choiceButtonList)
        {
            //ボタンにカーソルが合った時のイベントを登録
            b.Button.gameObject.TryGetComponent<EventTrigger>(out var trigger);

            var selectEntry = new EventTrigger.Entry();
            selectEntry.eventID = EventTriggerType.Select;

            selectEntry.callback.AddListener(eventData => 
            {
                _playerChoiceValue = b.ChoiceAnswerValue;
                _currentButtonIndex = b.ButtonIndex;
            });
            trigger.triggers.Add(selectEntry);

            //ボタン選択時の処理を登録
            b.Button.onClick.AddListener(() =>
            {
                _isChoiced = true;
                EventSystem.current.firstSelectedGameObject = null;
            });
        }
        _choicePanel.alpha = 0;
        _questionText.text = "";
        _AnswerTimeText.text = "";
    }

    /// <summary>
    /// 解答を選択するフェイズのコルーチン
    /// </summary>
    /// <param name="manager"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public IEnumerator OnChoicePhaseCoroutine(ObjectManager manager, QuizType type, Action<int> callback)
    {
        //選択状態のフラグをリセット
        _isChoiced = false;
        _correctImage.enabled = false;
        _wrongImage.enabled = false;

        foreach (var b in _choiceButtonList)
        {
            b.Button.gameObject.SetActive(true);
        }
        _choicePanel.alpha = 1;
        EventSystem.current.firstSelectedGameObject = _choiceButtonList[0].Button.gameObject;
        _choiceButtonList[0].Button.Select();

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
                _questionText.text = "赤いバラと白いバラの数は？";
                _currentAnswerValue = manager.CurrentRedRoseCount + manager.CurrentWhiteRoseCount;
                break;
            case QuizType.TrumpSolder:
                _questionText.text = "トランプ兵の数は？";
                _currentAnswerValue = manager.CurrentTrumpCount;
                
                break;
            case QuizType.All:
                _questionText.text = "バラとトランプ兵の数は？";
                _currentAnswerValue = manager.CurrentRedRoseCount + manager.CurrentWhiteRoseCount
                                    + manager.CurrentTrumpCount;
                break;
            default:
                break;
        }

        ChoiceButtonSetUp(type);

        yield return null;

        _playerChoiceValue = _choiceButtonList[0].ChoiceAnswerValue;
        _currentButtonIndex = _choiceButtonList[0].ButtonIndex;

        if (!_debugMode)
        {
            while (timer < _answerTime && !_isChoiced)
            {
                _AnswerTimeText.text = (_answerTime - timer).ToString("F0");
                timer += Time.deltaTime;
                yield return null;
            }
        }
        else
        {
            yield return new WaitUntil(() => _isChoiced);
        }

        _questionText.text = "";
        HighlightButton();

        if (Judge())
        {
            //_questionText.text = "正解！";
            _correctImage.enabled = true;
            AudioManager.PlaySE(SEType.Stage2_Correct);
            callback?.Invoke(1);
        }
        else
        {
            //_questionText.text = $"不正解… 正解は{_currentAnswerValue}";
            _wrongImage.enabled = true;
            AudioManager.PlaySE(SEType.Stage2_Wrong);
            callback?.Invoke(0);
        }

        if (HPManager.Instance.CurrentHP.Value <= 0)
        {
            yield break;
        }

        yield return new WaitForSeconds(_goToNextPhaseInterval);

        _choicePanel.alpha = 0;

        _AnswerTimeText.text = "";        
    }

    /// <summary>
    /// 解答の判定
    /// </summary>
    /// <returns> 判定結果 </returns>
    public bool Judge()
    {
        //if (!_isChoiced) return false; //時間内に選択していなかったら不正解

        Debug.Log($"正解:{_currentAnswerValue}, プレイヤーの解答:{_playerChoiceValue}");
        return _currentAnswerValue == _playerChoiceValue; //正解と解答を比較した真偽を返す
    }

    /// <summary>
    /// 選択肢となるの各ボタンの設定を行う
    /// </summary>
    /// <param name="type"> 問題の種類 </param>
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
            _choiceButtonList[i].Button.gameObject.SetActive(true);
            _choiceButtonList[i].ChoiceAnswerValue = sort[i];
            _choiceButtonList[i].ChoiceText.text = sort[i].ToString();

            switch (type)
            {
                case QuizType.TrumpSolder:
                    _choiceButtonList[i].TrumpUnitImage.enabled = true;
                    _choiceButtonList[i].RoseUnitImage.enabled = false;
                    break;
                case QuizType.All:
                    break;
                default:
                    _choiceButtonList[i].TrumpUnitImage.enabled = false;
                    _choiceButtonList[i].RoseUnitImage.enabled = true;
                    break;
            }
            //_choiceButtonList[i].ChoiceText.text = type switch
            //{
            //    QuizType.RedRose => $"{sort[i]}",
            //    QuizType.WhiteRose => $"{sort[i]}",
            //    QuizType.RedAndWhiteRose => $"{sort[i]}",
            //    QuizType.TrumpSolder => $"{sort[i]}",
            //    QuizType.All => $"{sort[i]}",
            //    _ => $"{sort[i]} ",
            //};
        }

    }

    void HighlightButton()
    {
        foreach (var b in _choiceButtonList)
        {
            b.Button.gameObject.SetActive(false);
        }

        _choiceButtonList[_currentButtonIndex].Button.gameObject.SetActive(true);
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
                    //同じ添え字の場合は処理をスキップ
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
    public Text ChoiceText;
    public int ChoiceAnswerValue;
    public Image RoseUnitImage;
    public Image TrumpUnitImage;
    public int ButtonIndex;
}
