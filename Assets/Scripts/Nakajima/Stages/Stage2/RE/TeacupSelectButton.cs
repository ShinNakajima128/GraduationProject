using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class TeacupSelectButton : MonoBehaviour
{
    #region serialize
    [SerializeField]
    Button _selectButton = default;
    #endregion

    #region private
    Image _buttonImage = default;
    EventTrigger _trigger;
    #endregion

    #region public
    #endregion

    #region property
    public Button SelectButton => _selectButton;
    public Image ButtonImage => _buttonImage;
    #endregion
    
    private void Awake()
    {
        _selectButton.TryGetComponent(out _buttonImage);
        _selectButton.TryGetComponent(out _trigger);
    }

    private void Start()
    {
        _buttonImage.enabled = false;

        //ボタンにカーソルが合った時のイベントを登録
        var selectEntry = new EventTrigger.Entry();
        selectEntry.eventID = EventTriggerType.Select;
        selectEntry.callback.AddListener(eventData => OnSelectEvent());
        _trigger.triggers.Add(selectEntry);

        var deselectEntry = new EventTrigger.Entry();
        deselectEntry.eventID = EventTriggerType.Deselect;
        deselectEntry.callback.AddListener(eventData => OnDeselectEvent());
        _trigger.triggers.Add(deselectEntry);

        _buttonImage.transform.DOLocalMoveY(-15f, 0.5f)
                              .SetEase(Ease.InCubic)
                              .SetLoops(-1, LoopType.Yoyo);
    }

    void OnSelectEvent()
    {
        _buttonImage.enabled = true;
    }

    void OnDeselectEvent()
    {
        _buttonImage.enabled = false;
    }
}
