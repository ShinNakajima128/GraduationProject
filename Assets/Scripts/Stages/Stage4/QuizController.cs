using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    Text _AnswerTimeText = default;
    #endregion
    #region private
    #endregion
    #region property
    #endregion

    public IEnumerator OnChoicePhaseCoroutine(int answer1, int answe2, int answer3, int answer4)
    {
        var timer = _answerTime;
        var waitTime = new WaitForSeconds(1.0f);

        while (timer > 0)
        {
            yield return waitTime;
            timer--;
        }
    }

    void SetUp()
    {

    }
}

[Serializable]
public class ChoiceButton
{
    public Button Button;
    public Text ChoiceText;
    public int ChoiceAnswerValue;
}
