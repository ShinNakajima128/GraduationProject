using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

/// <summary>
/// �Q�[���I�[�o�[���o�̋@�\�����R���|�[�l���g
/// </summary>
public class GameoverDirection : MonoBehaviour
{
    #region serialize
    [Header("UI")]
    [SerializeField]
    CanvasGroup _gameoverUIGroup = default;

    [Tooltip("�Q�[���I�[�o�[��ʂ̃{�^��")]
    [SerializeField]
    Button[] _gameoverSelectButtons = default;

    [SerializeField]
    Sprite[] _returnButtonSprite = default;

    [SerializeField]
    Transform _cursorImageTrans = default;

    [SerializeField]
    Transform[] _cursorPosTrans = default;

    [Header("Components")]
    [SerializeField]
    AliceMotionController _motionCtrl = default;

    [SerializeField]
    AliceFaceController _faceCtrl = default;

    [Header("Debug")]
    [SerializeField]
    bool _debugMode = false;
    #endregion

    #region private
    bool _isActiveUI = false;
    SceneType _currentSceneType = default;
    Image _returnButtonImage = default;
    #endregion

    #region public
    public event Action GameoverUIActivateAction;
    #endregion

    #region property
    public static GameoverDirection Instance { get; private set; }
    #endregion

    private void Awake()
    {
        Instance = this;

        _gameoverSelectButtons[1].gameObject.TryGetComponent(out _returnButtonImage);
    }

    private IEnumerator Start()
    {
        ButtonSetup();
        SetCurrentSceneType(GameManager.Instance.CurrentStage);
        
        yield return null;

        if (_debugMode)
        {
            ActivateGameoverUI(true);
        }
        else
        {
            ActivateGameoverUI(false);
        }
    }

    public void OnGameoverDirection()
    {
        StartCoroutine(GameoverDirectionCoroutine());
    }

    IEnumerator GameoverDirectionCoroutine()
    {
        TransitionManager.FadeIn(FadeType.Mask_KeyHole, 2.0f);

        yield return new WaitForSeconds(3.0f);

        TransitionManager.FadeOut(FadeType.Black_default, 2.0f);
        ActivateGameoverUI(true);
    }

    /// <summary>
    /// �Q�[���I�[�o�[��ʂ̕\��/��\�����s��
    /// </summary>
    /// <param name="isActivate"> �A�N�e�B�u����A�N�e�B�u�� </param>
    public void ActivateGameoverUI(bool isActivate)
    {
        if (isActivate)
        {
            _isActiveUI = true;
            _gameoverUIGroup.alpha = 1;
            EventSystem.current.firstSelectedGameObject = _gameoverSelectButtons[0].gameObject;
            _gameoverSelectButtons[0].Select();

            AudioManager.StopBGM(0.5f);
            AudioManager.PlaySE(SEType.Gameover_Jingle);
        }
        else
        {
            _isActiveUI = false;
            _gameoverUIGroup.alpha = 0;
        }
    }

    void ButtonSetup()
    {
        _gameoverSelectButtons[0].gameObject.TryGetComponent<EventTrigger>(out var trigger);

        var selectEntry = new EventTrigger.Entry();
        selectEntry.eventID = EventTriggerType.Select;

        selectEntry.callback.AddListener(eventData =>
        {
            Debug.Log("�J�[�\�������Ɉړ�");
            _cursorImageTrans.SetParent(_gameoverSelectButtons[0].transform);
            _cursorImageTrans.localPosition = _cursorPosTrans[0].localPosition;
        });
        trigger.triggers.Add(selectEntry);

        //�{�^���I�����̏�����o�^
        _gameoverSelectButtons[0].onClick.AddListener(() =>
        {
            if (_isActiveUI)
            {
                StartCoroutine(DirectionCoroutine(true));
                _gameoverSelectButtons[0].transform.DOLocalMoveY(_gameoverSelectButtons[0].transform.localPosition.y - 15, 0.05f)
                                                   .SetLoops(2, LoopType.Yoyo);
            }
        });

        _gameoverSelectButtons[1].gameObject.TryGetComponent<EventTrigger>(out var trigger2);

        var selectEntry2 = new EventTrigger.Entry();
        selectEntry2.eventID = EventTriggerType.Select;

        selectEntry2.callback.AddListener(eventData =>
        {
            _cursorImageTrans.SetParent(_gameoverSelectButtons[1].transform);
            _cursorImageTrans.localPosition = _cursorPosTrans[1].localPosition;
            Debug.Log("�J�[�\�����E�Ɉړ�");
        });
        trigger2.triggers.Add(selectEntry2);

        //�{�^���I�����̏�����o�^
        _gameoverSelectButtons[1].onClick.AddListener(() =>
        {
            if (_isActiveUI)
            {
                StartCoroutine(DirectionCoroutine(false));
                _gameoverSelectButtons[1].transform.DOLocalMoveY(_gameoverSelectButtons[1].transform.localPosition.y - 15, 0.05f)
                                                   .SetLoops(2, LoopType.Yoyo);
            }
        });
    }

    void SetCurrentSceneType(Stages stage)
    {
        switch (stage)
        {
            case Stages.Stage1:
                _currentSceneType = SceneType.Stage1_Fall;
                _returnButtonImage.sprite = _returnButtonSprite[0];
                break;
            case Stages.Stage2:
                _currentSceneType = SceneType.RE_Stage2;
                _returnButtonImage.sprite = _returnButtonSprite[0];
                break;
            case Stages.Stage3:
                _currentSceneType = SceneType.RE_Stage3;
                _returnButtonImage.sprite = _returnButtonSprite[0];
                break;
            case Stages.Stage4:
                _currentSceneType = SceneType.Stage4;
                _returnButtonImage.sprite = _returnButtonSprite[0];
                break;
            case Stages.Stage_Boss:
                _currentSceneType = SceneType.Stage_Boss;
                _returnButtonImage.sprite = _returnButtonSprite[1];
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// ���o�̃R���[�`��
    /// </summary>
    /// <param name="isRetryed"> �����邩���r�[�ɖ߂邩 </param>
    /// <returns></returns>
    IEnumerator DirectionCoroutine(bool isRetryed)
    {
        //������x�V�ԃ{�^�����������ꍇ
        if (isRetryed)
        {
            _motionCtrl.ChangeAnimation(AliceDirectionAnimType.Rise);
            _faceCtrl.ChangeFaceType(FaceType.Cry);

            yield return new WaitForSeconds(4.0f);

            _motionCtrl.ChangeAnimation(AliceDirectionAnimType.Retry);

            yield return new WaitForSeconds(1.5f);
            
            _faceCtrl.ChangeFaceType(FaceType.Angry);
            
            yield return new WaitForSeconds(3.0f);

            TransitionManager.SceneTransition(_currentSceneType);
        }
        //�߂�{�^�����������ꍇ
        else
        {
            //���݂�Scene���{�X�X�e�[�W�̏ꍇ�͒n�����r�[�ɖ߂�
            if (_currentSceneType == SceneType.Stage_Boss)
            {
                TransitionManager.SceneTransition(SceneType.UnderLobby);
            }
            else
            {
                TransitionManager.SceneTransition(SceneType.Lobby);
            }
        }
    }
}
