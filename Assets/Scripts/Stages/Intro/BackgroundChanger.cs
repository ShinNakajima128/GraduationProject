using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// イントロSceneの背景を操作するコンポーネント
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
    /// 背景を変更する
    /// </summary>
    /// <param name="background"> 変更する背景 </param>
    /// <param name="action"> 変更後に実行するAction </param>
    public void BackgroundChange(Sprite background, Action action = null)
    {
        if (background == null)
        {
            action?.Invoke();
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
