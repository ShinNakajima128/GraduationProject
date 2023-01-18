using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;
using UnityEngine.EventSystems;

/// <summary>
/// 各ステージの詳細を表示するUIの機能を持つコンポーネント
/// </summary>
public class StageDescriptionUI : MonoBehaviour
{
    #region serialize
    [Header("UIObjects")]
    [Tooltip("ステージ詳細画面のButton")]
    [SerializeField]
    Button[] _descriptionButtons = default;

    [Tooltip("カーソルのUIImage")]
    [SerializeField]
    Image _cursorImage = default;

    [Tooltip("カーソルの移動位置")]
    [SerializeField]
    Transform[] _cursorTrans = default;

    [Tooltip("チュートリアル画面")]
    [SerializeField]
    CanvasGroup _tutorialGroup = default;
    #endregion

    #region private
    bool _isActiveUI = false;
    SceneType _currentSelectScene = default;
    #endregion

    #region public
    #endregion

    #region property
    public static StageDescriptionUI Instance { get; private set; }
    #endregion

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        ButtonSetup();
    }

    /// <summary>
    /// ステージ詳細をアクティブにする
    /// </summary>
    public void ActiveDescription(SceneType stage)
    {
        _currentSelectScene = stage;
        _isActiveUI = true;
        _descriptionButtons[0].Select();
        print("ステージ詳細表示");
    }

    /// <summary>
    /// ステージ詳細を非アクティブにする
    /// </summary>
    public void InActiceDescription()
    {
        _isActiveUI = false;
    }

    void ButtonSetup()
    {
        _descriptionButtons[0].gameObject.TryGetComponent<EventTrigger>(out var trigger);

        var selectEntry = new EventTrigger.Entry();
        selectEntry.eventID = EventTriggerType.Select;

        selectEntry.callback.AddListener(eventData =>
        {
            _cursorImage.transform.localPosition = _cursorTrans[0].localPosition;
            Debug.Log("カーソルを左に移動");
        });
        trigger.triggers.Add(selectEntry);

        //ボタン選択時の処理を登録
        _descriptionButtons[0].onClick.AddListener(() =>
        {
            if (_isActiveUI)
            {
                Debug.Log("チュートリアル表示");
            }
        });

        _descriptionButtons[1].gameObject.TryGetComponent<EventTrigger>(out var trigger2);

        var selectEntry2 = new EventTrigger.Entry();
        selectEntry2.eventID = EventTriggerType.Select;

        selectEntry2.callback.AddListener(eventData =>
        {
            _cursorImage.transform.localPosition = _cursorTrans[1].localPosition;
            Debug.Log("カーソルを右に移動");
        });
        trigger2.triggers.Add(selectEntry2);

        //ボタン選択時の処理を登録
        _descriptionButtons[1].onClick.AddListener(() =>
        {
            if (_isActiveUI)
            {
                TransitionManager.SceneTransition(_currentSelectScene);
            }
        });
    }
}
