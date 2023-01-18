using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;
using UnityEngine.EventSystems;

/// <summary>
/// �e�X�e�[�W�̏ڍׂ�\������UI�̋@�\�����R���|�[�l���g
/// </summary>
public class StageDescriptionUI : MonoBehaviour
{
    #region serialize
    [Header("UIObjects")]
    [Tooltip("�X�e�[�W�ڍ׉�ʂ�Button")]
    [SerializeField]
    Button[] _descriptionButtons = default;

    [Tooltip("�J�[�\����UIImage")]
    [SerializeField]
    Image _cursorImage = default;

    [Tooltip("�J�[�\���̈ړ��ʒu")]
    [SerializeField]
    Transform[] _cursorTrans = default;

    [Tooltip("�`���[�g���A�����")]
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
    /// �X�e�[�W�ڍׂ��A�N�e�B�u�ɂ���
    /// </summary>
    public void ActiveDescription(SceneType stage)
    {
        _currentSelectScene = stage;
        _isActiveUI = true;
        _descriptionButtons[0].Select();
        print("�X�e�[�W�ڍו\��");
    }

    /// <summary>
    /// �X�e�[�W�ڍׂ��A�N�e�B�u�ɂ���
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
            Debug.Log("�J�[�\�������Ɉړ�");
        });
        trigger.triggers.Add(selectEntry);

        //�{�^���I�����̏�����o�^
        _descriptionButtons[0].onClick.AddListener(() =>
        {
            if (_isActiveUI)
            {
                Debug.Log("�`���[�g���A���\��");
            }
        });

        _descriptionButtons[1].gameObject.TryGetComponent<EventTrigger>(out var trigger2);

        var selectEntry2 = new EventTrigger.Entry();
        selectEntry2.eventID = EventTriggerType.Select;

        selectEntry2.callback.AddListener(eventData =>
        {
            _cursorImage.transform.localPosition = _cursorTrans[1].localPosition;
            Debug.Log("�J�[�\�����E�Ɉړ�");
        });
        trigger2.triggers.Add(selectEntry2);

        //�{�^���I�����̏�����o�^
        _descriptionButtons[1].onClick.AddListener(() =>
        {
            if (_isActiveUI)
            {
                TransitionManager.SceneTransition(_currentSelectScene);
            }
        });
    }
}
