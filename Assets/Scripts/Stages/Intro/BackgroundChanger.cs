using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using AliceProject;

/// <summary>
/// ƒCƒ“ƒgƒScene‚Ì”wŒi‚ğ‘€ì‚·‚éƒRƒ“ƒ|[ƒlƒ“ƒg
/// </summary>
public class BackgroundChanger : MonoBehaviour
{
    #region serialize
    [SerializeField]
    Image[] _backgrounds = default;

    [SerializeField]
    float _changeTime = 2.0f;
    #endregion

    #region private
    BackgroundState _state = BackgroundState.panel1; 
    #endregion

    /// <summary>
    /// ”wŒi‚ğ•ÏX‚·‚é
    /// </summary>
    /// <param name="background"> •ÏX‚·‚é”wŒi </param>
    /// <param name="action"> •ÏXŒã‚ÉÀs‚·‚éAction </param>
    public void BackgroundChange(ScenarioData data, Action action = null)
    {

        var background = data.Background;

        if (background == null)
        {
            action?.Invoke();
            Debug.Log("”wŒi–³‚µ");
            return;
        }

        switch (_state)
        {
            case BackgroundState.panel1:
                if (!_backgrounds[0].enabled)
                {
                    _backgrounds[0].enabled = true;
                }
                _backgrounds[1].sprite = background;
                _backgrounds[1].DOFade(1f, _changeTime)
                               .OnComplete(() => 
                               {
                                   action?.Invoke();
                                   _state = BackgroundState.panel2;
                               });
                break;
            case BackgroundState.panel2:
                if (!_backgrounds[1].enabled)
                {
                    _backgrounds[1].enabled = true;
                }
                _backgrounds[0].sprite = background;
                _backgrounds[1].DOFade(0f, _changeTime)
                               .OnComplete(() =>
                               {
                                   action?.Invoke();
                                   _state = BackgroundState.panel1;
                               });
                break;
            default:
                break;
        }
    }
}
public enum BackgroundState
{
    panel1,
    panel2
}
