using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// タイトル画面の機能を管理するマネージャー
/// </summary>
public class TitleManager : MonoBehaviour
{
    #region serialize
    [SerializeField]
    Button[] _titleButtons = default;
    #endregion

    #region private
    Dictionary<ButtonType, Button> _buttonDic = new Dictionary<ButtonType, Button>();
    #endregion

    #region property
    public static TitleManager Instance { get; private set; }
    #endregion

    private void Awake()
    {
        Instance = this;

        for (int i = 0; i < _titleButtons.Length; i++)
        {
            _buttonDic.Add((ButtonType)i, _titleButtons[i]);
        }
    }
    IEnumerator Start()
    {
        AudioManager.PlayBGM(BGMType.Title);
        TransitionManager.FadeOut(FadeType.Normal);
        ButtonSetup();

        yield return null;

        GameManager.GameReset();
    }
    void ButtonSetup()
    {
        foreach (var b in _buttonDic)
        {
            switch (b.Key)
            {
                case ButtonType.GameStart:
                    b.Value.onClick.AddListener(() =>
                    {
                        TransitionManager.SceneTransition(SceneType.Intro);
                    });
                    break;
                case ButtonType.Credit:
                    b.Value.onClick.AddListener(() =>
                    {
                        Debug.Log("Credit");
                    });
                    break;
                case ButtonType.Option:
                    b.Value.onClick.AddListener(() =>
                    {
                        Debug.Log("Option");
                    });
                    break;
                default:
                    break;
            }
        }
    }
}
public enum ButtonType
{
    GameStart,
    Credit,
    Option
}
