using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;
using UnityEngine.EventSystems;
using DG.Tweening;

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
    StageTutorial _tutorial = default;
    #endregion

    #region private
    bool _isActiveUI = false;
    bool _isButtonClicking = false;
    SceneType _currentSelectScene = default;
    #endregion

    #region public
    public event Action<bool> OpenTutorialAction;
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

        this.UpdateAsObservable()
            .Where(_ => _tutorial.IsActivateTutorial && UIInput.A && !_isButtonClicking)
            .Subscribe(_ =>
            {
                OffTutorialPanel();
            })
            .AddTo(this);
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
            //�`���[�g���A����ʂ��J���Ă���ꍇ�͏������s��Ȃ�
            if (_tutorial.IsActivateTutorial)
            {
                return;
            }
            _cursorImage.transform.SetParent(_descriptionButtons[0].transform);
            _cursorImage.transform.localPosition = _cursorTrans[0].localPosition;
            Debug.Log("�J�[�\�������Ɉړ�");
        });
        trigger.triggers.Add(selectEntry);

        //�{�^���I�����̏�����o�^
        _descriptionButtons[0].onClick.AddListener(() =>
        {
            if (_isActiveUI)
            {
                //�`���[�g���A����ʂ��J���Ă���ꍇ�͏������s��Ȃ�
                if (_tutorial.IsActivateTutorial || _isButtonClicking)
                {
                    return;
                }
                _descriptionButtons[0].transform.DOLocalMoveY(_descriptionButtons[0].transform.localPosition.y - 15, 0.05f)
                                                .SetLoops(2, LoopType.Yoyo);
                _isButtonClicking = true;
                OpenTutorialAction?.Invoke(false);
                Debug.Log("�`���[�g���A���\��");

                switch (_currentSelectScene)
                {
                    
                    case SceneType.Stage1_Fall:
                        _tutorial.TutorialSetup(Stages.Stage1, () => 
                        {
                            TransitionManager.SceneTransition(SceneType.Stage1_Fall, FadeType.Mask_KeyHole);
                        });
                        break;
                    case SceneType.RE_Stage2:
                        _tutorial.TutorialSetup(Stages.Stage2, () =>
                        {
                            TransitionManager.SceneTransition(SceneType.RE_Stage2, FadeType.Mask_KeyHole);
                        });
                        break;
                    case SceneType.RE_Stage3:
                        _tutorial.TutorialSetup(Stages.Stage3, () =>
                        {
                            TransitionManager.SceneTransition(SceneType.RE_Stage3, FadeType.Mask_KeyHole);
                        });
                        break;
                    case SceneType.Stage4:
                        _tutorial.TutorialSetup(Stages.Stage4, () =>
                        {
                            TransitionManager.SceneTransition(SceneType.Stage4, FadeType.Mask_KeyHole);
                        });
                        break;
                    case SceneType.Stage_Boss:
                        _tutorial.TutorialSetup(Stages.Stage_Boss, () =>
                        {
                            TransitionManager.SceneTransition(SceneType.Stage_Boss, FadeType.Mask_KeyHole);
                        });
                        break;
                    default:
                        Debug.LogError($"�X�e�[�W�̎w�肪�Ԉ���Ă��܂�{_currentSelectScene}");
                        break;
                }

                TransitionManager.FadeIn(FadeType.Mask_CheshireCat,
                              �@  0.5f,
                            �@    () =>
                            �@    {
                            �@        _tutorial.ActivateTutorialUI(true); //�`���[�g���A����\������
                              �@      TransitionManager.FadeOut(FadeType.Mask_CheshireCat,
                                    �@0.5f,
                            �@        () =>
                               �@     {
                              �@          _isButtonClicking = false;
                              �@      });
                             �@   });
            }
        });

        _descriptionButtons[1].gameObject.TryGetComponent<EventTrigger>(out var trigger2);

        var selectEntry2 = new EventTrigger.Entry();
        selectEntry2.eventID = EventTriggerType.Select;

        selectEntry2.callback.AddListener(eventData =>
        {
            //�`���[�g���A����ʂ��J���Ă���ꍇ�͏������s��Ȃ�
            if (_tutorial.IsActivateTutorial)
            {
                return;
            }

            _cursorImage.transform.SetParent(_descriptionButtons[1].transform);
            _cursorImage.transform.localPosition = _cursorTrans[1].localPosition;
            Debug.Log("�J�[�\�����E�Ɉړ�");
        });
        trigger2.triggers.Add(selectEntry2);

        //�{�^���I�����̏�����o�^
        _descriptionButtons[1].onClick.AddListener(() =>
        {
            if (_isActiveUI)
            {
                //�`���[�g���A����ʂ��J���Ă���ꍇ�ƁA�{�^���������Ă���ꍇ�͏������s��Ȃ�
                if (_tutorial.IsActivateTutorial || _isButtonClicking)
                {
                    return;
                }

                _isButtonClicking = true;
                TransitionManager.SceneTransition(_currentSelectScene);
                _descriptionButtons[1].transform.DOLocalMoveY(_descriptionButtons[1].transform.localPosition.y - 15, 0.05f)
                                                   .SetLoops(2, LoopType.Yoyo);
            }
        });
    }

    void OffTutorialPanel()
    {
        StartCoroutine(OffTutorialCoroutine());
    }

    IEnumerator OffTutorialCoroutine()
    {
        _isButtonClicking = true;
        TransitionManager.FadeIn(FadeType.Mask_CheshireCat,
                         0.5f,
                         () => 
                         {
                             _tutorial.ActivateTutorialUI(false);
                             TransitionManager.FadeOut(FadeType.Mask_CheshireCat, 0.5f);
                             OpenTutorialAction?.Invoke(true);
                         });

        yield return new WaitForSeconds(1.5f);
        
        _isButtonClicking = false;
        _descriptionButtons[0].Select();
    }
}
