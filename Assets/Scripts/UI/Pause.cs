using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UniRx;
using UniRx.Triggers;

/// <summary>
/// ポーズ画面の機能を持つコンポーネント
/// </summary>
public class Pause : MonoBehaviour
{
    #region serialize
    [Header("Variables")]
    [Tooltip("現在のSceneの種類")]
    [SerializeField]
    SceneType _sceneType = default;

    [Header("UIObjects")]
    [SerializeField]
    Button[] _pauseButtons = default;
    #endregion

    #region private
    CanvasGroup _pauseGroup = default;
    bool _isPressed = false;
    #endregion

    #region public
    #endregion

    #region property
    public bool IsActived => _pauseGroup.alpha == 1;
    #endregion

    private void Awake()
    {
        TryGetComponent(out _pauseGroup);
    }
    private void Start()
    {
        Setup();
    }

    void Setup()
    {
        this.UpdateAsObservable()
            .Where(_ => !IsActived && UIInput.Option)
            .Subscribe(_ =>
            {
                StartCoroutine(ActivateCoroutine(true));
            });

        this.UpdateAsObservable()
            .Where(_ => IsActived && UIInput.Option)
            .Subscribe(_ =>
            {
                StartCoroutine(ActivateCoroutine(false));
            });

        _pauseGroup.alpha = 0;

        foreach (var b in _pauseButtons)
        {
            b.gameObject.SetActive(false);
        }

        _pauseButtons[0].gameObject.SetActive(true);
        _pauseButtons[1].gameObject.SetActive(true);

        switch (_sceneType)
        {
            case SceneType.Lobby:
                _pauseButtons[4].gameObject.SetActive(true);
                break;
            case SceneType.UnderLobby:
                _pauseButtons[4].gameObject.SetActive(true);
                break;
            case SceneType.Stage_Boss:
                _pauseButtons[3].gameObject.SetActive(true);
                break;
            default:
                _pauseButtons[2].gameObject.SetActive(true);
                break;
        }
        ButtonCursor.MoveCursor(new Vector3(_pauseButtons[0].gameObject.transform.position.x + 2.5f, 
                                            _pauseButtons[0].gameObject.transform.position.y + 5f, 
                                            _pauseButtons[0].gameObject.transform.position.z), 
                                            _pauseButtons[0].gameObject.transform);
    }

    IEnumerator ActivateCoroutine(bool isActivate)
    {
        _isPressed = true;

        yield return new WaitForSeconds(0.1f);

        _isPressed = false;

        if (isActivate)
        {
            _pauseGroup.alpha = 1;
            EventSystem.current.firstSelectedGameObject = _pauseButtons[0].gameObject;
            _pauseButtons[0].Select();
            print("ポーズメニューON");
        }
        else
        {
            _pauseGroup.alpha = 0;
            print("ポーズメニューOFF");
        }
    }
}

public enum PauseButtonType
{
    ReturnGame,
    Option,
    Return_Lobby,
    Return_UnderLobby,
    Return_Title
}
