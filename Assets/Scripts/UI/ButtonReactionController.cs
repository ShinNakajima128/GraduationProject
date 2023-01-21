using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;


[RequireComponent(typeof(EventTrigger))]
public class ButtonReactionController : MonoBehaviour
{
    #region serialize
    [Header("variables")]
    [SerializeField]
    float _animScale = 1.3f;

    [SerializeField]
    float _animTime = 0.25f;
    #endregion

    #region private
    EventTrigger _trigger;

    Transform _cursorImageTrans = default;
    #endregion
    #region public
    #endregion
    #region property
    #endregion

    private void Awake()
    {
        TryGetComponent(out _trigger);
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
        ButtonCursor.MoveCursor(new Vector3(transform.position.x + 2.5f, transform.position.y + 2.5f, transform.position.z), transform);
        transform.DOScale(_animScale, _animTime);
    }
    void OnDeselectEvent()
    {
        transform.DOScale(1.0f, _animTime);
    }
}
