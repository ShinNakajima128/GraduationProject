using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CroquetGameUI : MonoBehaviour
{
    #region serialize
    [SerializeField]
    CanvasGroup _orderGroup = default;

    [SerializeField]
    CanvasGroup _inGameGroup = default;

    [SerializeField]
    CanvasGroup _goalDirectionGroup = default;
    #endregion

    #region private
    #endregion
    
    #region public
    #endregion
    
    #region property
    #endregion

    private void Start()
    {
        
    }

    /// <summary>
    /// UIÇÃGroupÇêÿÇËë÷Ç¶ÇÈ
    /// </summary>
    /// <param name="state"> ÉQÅ[ÉÄÇÃèÛë‘ </param>
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
}
