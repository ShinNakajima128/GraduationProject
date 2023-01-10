using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class CroquetGameUI : MonoBehaviour
{
    #region serialize
    [Header("UI_CanvasGroup")]
    [SerializeField]
    CanvasGroup _orderGroup = default;

    [SerializeField]
    CanvasGroup _inGameGroup = default;

    [SerializeField]
    CanvasGroup _goalDirectionGroup = default;

    [Header("UI_Text")]
    [SerializeField]
    Text _currentRoundText = default;

    [SerializeField]
    Text _currentOrderText = default;

    [SerializeField]
    Text _redTrumpCountText = default;

    [SerializeField]
    Text _blackTrumpCountText = default;

    [SerializeField]
    Text _resultText = default;

    [Header("UIObjects")]
    [SerializeField]
    GameObject _successImage = default;

    [SerializeField]
    GameObject _failureImage = default;
    #endregion

    #region private
    //ReactiveProperty<int> 
    #endregion

    #region public
    #endregion

    #region property
    #endregion

    private void Start()
    {
        _successImage.SetActive(false);
        _failureImage.SetActive(false);
    }

    /// <summary>
    /// UIのGroupを切り替える
    /// </summary>
    /// <param name="state"> ゲームの状態 </param>
    public void ChangeUIGroup(CroquetGameState state)
    {
        switch (state)
        {
            case CroquetGameState.Order:
                _orderGroup.alpha = 1;
                _inGameGroup.alpha = 0;
                _goalDirectionGroup.alpha = 0;
                break;
            case CroquetGameState.InGame:
                _orderGroup.alpha = 0;
                _inGameGroup.alpha = 1;
                _goalDirectionGroup.alpha = 0;
                break;
            case CroquetGameState.GoalDirection:
                _orderGroup.alpha = 0;
                _inGameGroup.alpha = 0;
                _goalDirectionGroup.alpha = 1;
                break;
            default:
                break;
        }
    }

    public void SetTrumpCount(int redCount, int blackCount)
    {
        _redTrumpCountText.text = $"{redCount}";
        _blackTrumpCountText.text  = $"{ blackCount}";
    }

    /// <summary>
    /// お題のテキストをTextセットする
    /// </summary>
    /// <param name="round">現在のラウンド</param>
    /// <param name="order"> お題 </param>
    public void SetOrderText(int round, string order)
    {
        _currentRoundText.text = $"{round}";
        _currentOrderText.text = order;
    }

    /// <summary>
    /// 結果を表示する
    /// </summary>
    /// <param name="result"> お題の結果 </param>
    public void SetResultText(string result)
    {
        _resultText.text = result;
    }

    public void OnResultUI(bool result)
    {
        if (result)
        {
            _successImage.SetActive(true);
        }
        else
        {
            _failureImage.SetActive(true);
        }
    }

    public void OffResultUI()
    {
        _successImage.SetActive(false);
        _failureImage.SetActive(false);
    }
}
