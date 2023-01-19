using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

/// <summary>
/// ゲームオーバー演出の機能を持つコンポーネント
/// </summary>
public class GameoverDirection : MonoBehaviour
{
    #region serialize
    [Header("UI")]
    [SerializeField]
    CanvasGroup _gameoverUIGroup = default;

    [Tooltip("ゲームオーバー画面のボタン")]
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
    /// ゲームオーバー画面の表示/非表示を行う
    /// </summary>
    /// <param name="isActivate"> アクティブか非アクティブか </param>
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
            Debug.Log("カーソルを左に移動");
            _cursorImageTrans.SetParent(_gameoverSelectButtons[0].transform);
            _cursorImageTrans.localPosition = _cursorPosTrans[0].localPosition;
        });
        trigger.triggers.Add(selectEntry);

        //ボタン選択時の処理を登録
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
            Debug.Log("カーソルを右に移動");
        });
        trigger2.triggers.Add(selectEntry2);

        //ボタン選択時の処理を登録
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
    /// 演出のコルーチン
    /// </summary>
    /// <param name="isRetryed"> 続けるかロビーに戻るか </param>
    /// <returns></returns>
    IEnumerator DirectionCoroutine(bool isRetryed)
    {
        //もう一度遊ぶボタンを押した場合
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
        //戻るボタンを押した場合
        else
        {
            //現在のSceneがボスステージの場合は地下ロビーに戻る
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
