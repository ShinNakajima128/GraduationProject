using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;


[RequireComponent(typeof(EventTrigger))]
public class ButtonReactionController : MonoBehaviour
{
    #region serialize
    [Header("variables")]
    [SerializeField]
    float _animScale = 1.3f;

    [Tooltip("カーソルのX座標の補正値")]
    [SerializeField]
    float _correctionValue_x = 2.5f;

    [Tooltip("カーソルのY座標の補正値")]
    [SerializeField]
    float _correctionValue_y = 2.5f;

    [SerializeField]
    float _animTime = 0.25f;

    [SerializeField]
    ButtonData _buttonData = default;
    #endregion

    #region private
    EventTrigger _trigger;
    Image _buttonBackground;
    Text _buttonText;
    Outline _textOutline;
    #endregion
    #region public
    #endregion
    #region property
    #endregion

    private void Awake()
    {
        TryGetComponent(out _trigger);
        TryGetComponent(out _buttonBackground);
        _buttonText = GetComponentInChildren<Text>();
        _textOutline = GetComponentInChildren<Outline>();

        _buttonBackground.sprite = _buttonData.DeselectSprite;
        _buttonText.color = _buttonData.DeselectTextColor;
        _textOutline.effectColor = _buttonData.TextOutlineColor;

    }
    private void Start()
    {
        //ボタンにカーソルが合った時のイベントを登録
        var selectEntry = new EventTrigger.Entry();
        selectEntry.eventID = EventTriggerType.Select;
        selectEntry.callback.AddListener(eventData => OnSelectEvent());
        _trigger.triggers.Add(selectEntry);

        var deselectEntry = new EventTrigger.Entry();
        deselectEntry.eventID = EventTriggerType.Deselect;
        deselectEntry.callback.AddListener(eventData => OnDeselectEvent());
        _trigger.triggers.Add(deselectEntry);
    }

    void OnSelectEvent()
    {
        ButtonCursor.MoveCursor(new Vector3(transform.position.x + _correctionValue_x, 
                                            transform.position.y + _correctionValue_y, 
                                            transform.position.z), 
                                            transform);
        transform.DOScale(_animScale, _animTime);
        _buttonBackground.sprite = _buttonData.SelectSprite;
        _buttonText.color = _buttonData.SelectTextColor;
    }
    void OnDeselectEvent()
    {
        transform.DOScale(1.0f, _animTime);
        _buttonBackground.sprite = _buttonData.DeselectSprite;
        _buttonText.color = _buttonData.DeselectTextColor;
    }
}
