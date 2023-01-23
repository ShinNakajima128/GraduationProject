using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UniRx;
using UniRx.Triggers;


public class GameEnd : MonoBehaviour
{
    #region serialize
    [Header("UIObjects")]
    [SerializeField]
    Button[] _gameEndButtons = default;

    [SerializeField]
    Transform _cursorImageTrans = default;

    [SerializeField]
    Transform[] _cursorPosTrans = default;
    #endregion

    #region private
    CanvasGroup _gameEndGroup = default;
    bool _isPressed = false;
    #endregion

    #region public
    #endregion

    #region property
    public bool IsActived => _gameEndGroup.alpha == 1;
    #endregion

    private void Awake()
    {
        TryGetComponent(out _gameEndGroup);
        Setup();
        ButtonSetup();
    }

    private void Start()
    {
        EventSystem.current.firstSelectedGameObject = _gameEndButtons[0].gameObject;
        _gameEndButtons[0].Select();

        this.UpdateAsObservable()
            .Where(_ => !IsActived &&
                        UIInput.Exit &&
                        !UIManager.Instance.IsAnyPanelOpened)
            .Subscribe(_ =>
            {
                StartCoroutine(ActivateCoroutine(true));
            })
            .AddTo(this);

        //this.UpdateAsObservable()
        //    .Where(_ => IsActived)
        //    .Where(_ => UIInput.Exit || UIInput.A)
        //    .Subscribe(_ =>
        //    {
        //        StartCoroutine(ActivateCoroutine(false));
        //    });
    }

    void Setup()
    {
        var trigger1 = _gameEndButtons[0].gameObject.GetComponent<EventTrigger>();
        //ボタンにカーソルが合った時のイベントを登録
        var selectEntry1 = new EventTrigger.Entry();
        selectEntry1.eventID = EventTriggerType.Select;
        selectEntry1.callback.AddListener(eventData => OnSelectEvent(0));
        trigger1.triggers.Add(selectEntry1);

        var deselectEntry1 = new EventTrigger.Entry();
        deselectEntry1.eventID = EventTriggerType.Deselect;
        deselectEntry1.callback.AddListener(eventData => OnDeselectEvent());
        trigger1.triggers.Add(deselectEntry1);


        var trigger2 = _gameEndButtons[1].gameObject.GetComponent<EventTrigger>();
        //ボタンにカーソルが合った時のイベントを登録
        var selectEntry2 = new EventTrigger.Entry();
        selectEntry2.eventID = EventTriggerType.Select;
        selectEntry2.callback.AddListener(eventData => OnSelectEvent(1));
        trigger2.triggers.Add(selectEntry2);

        var deselectEntry2 = new EventTrigger.Entry();
        deselectEntry2.eventID = EventTriggerType.Deselect;
        deselectEntry2.callback.AddListener(eventData => OnDeselectEvent());
        trigger2.triggers.Add(deselectEntry2);
    }

    void ButtonSetup()
    {
        _gameEndButtons[0].onClick.AddListener(() =>
        {
            StartCoroutine(ActivateCoroutine(false));
        });

        _gameEndButtons[1].onClick.AddListener(() =>
        {
            TransitionManager.SceneTransition(SceneType.Title, FadeType.Mask_KeyHole);
        });
    }

    void OnSelectEvent(int index)
    {
        _cursorImageTrans.SetParent(_gameEndButtons[index].transform);
        _cursorImageTrans.localPosition = _cursorPosTrans[index].localPosition;
    }

    void OnDeselectEvent()
    {

    }

    IEnumerator ActivateCoroutine(bool isActivate)
    {
        _isPressed = true;

        yield return new WaitForSeconds(0.1f);

        _isPressed = false;

        if (isActivate)
        {
            UIManager.ActivatePanel(UIPanelType.GameEnd);
            LobbyManager.Instance.PlayerMove?.Invoke(false);
            _gameEndGroup.alpha = 1;
            EventSystem.current.firstSelectedGameObject = _gameEndButtons[0].gameObject;
            _gameEndButtons[0].Select();
            print("ゲーム終了画面ON");
        }
        else
        {
            UIManager.InactivatePanel(UIPanelType.GameEnd);
            LobbyManager.Instance.PlayerMove?.Invoke(true);

            if (StageDescriptionUI.Instance.IsActived)
            {
                StageDescriptionUI.Instance.ActiveButton();
            }
            _gameEndGroup.alpha = 0;
            print("ゲーム終了画面OFF");
        }
    }
}
