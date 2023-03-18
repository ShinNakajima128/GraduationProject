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

    [SerializeField]
    CanvasGroup _hpGroup = default;

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

    [SerializeField]
    GameObject _nextImage = default;
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
        _nextImage.SetActive(false);
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

                _hpGroup.alpha = 0;
                break;
            case CroquetGameState.InGame:
                _orderGroup.alpha = 0;
                _inGameGroup.alpha = 1;
                _goalDirectionGroup.alpha = 0;

                _hpGroup.alpha = 1;
                break;
            case CroquetGameState.GoalDirection:
                _orderGroup.alpha = 0;
                _inGameGroup.alpha = 0;
                _goalDirectionGroup.alpha = 1;

                _hpGroup.alpha = 1;
                break;
            default:
                _orderGroup.alpha = 0;
                _inGameGroup.alpha = 0;
                _goalDirectionGroup.alpha = 0;

                _hpGroup.alpha = 0;
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

    /// <summary>
    /// �Q�[���̌��ʉ�ʂ�\������
    /// </summary>
    /// <param name="result"></param>
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

    /// <summary>
    /// ���֐i��UI��\������
    /// </summary>
    public void OnNextUI()
    {
        _nextImage.SetActive(true);
    }

    /// <summary>
    /// ���֐i��UI���\������
    /// </summary>
    public void OffResultUI()
    {
        _successImage.SetActive(false);
        _failureImage.SetActive(false);
        _nextImage.SetActive(false);
    }
}