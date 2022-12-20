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
        
    }

    /// <summary>
    /// UI��Group��؂�ւ���
    /// </summary>
    /// <param name="state"> �Q�[���̏�� </param>
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
    /// ����̃e�L�X�g��Text�Z�b�g����
    /// </summary>
    /// <param name="round">���݂̃��E���h</param>
    /// <param name="order"> ���� </param>
    public void SetOrderText(int round, string order)
    {
        _currentRoundText.text = $"{round}";
        _currentOrderText.text = order;
    }

    /// <summary>
    /// ���ʂ�\������
    /// </summary>
    /// <param name="result"> ����̌��� </param>
    public void SetResultText(string result)
    {
        _resultText.text = result;
    }
}
