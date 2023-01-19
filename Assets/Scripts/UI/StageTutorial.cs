using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;
using DG.Tweening;

/// <summary>
/// �X�e�[�W�̃`���[�g���A����\������@�\�����R���|�[�l���g
/// </summary>
public class StageTutorial : MonoBehaviour
{
    #region serialize
    [Header("Datas")]
    [SerializeField]
    StageTutorialData[] _tutorialData = default;

    [Header("UIObjects")]
    [Header("Page")]
    [SerializeField]
    Image _currentPageIcon = default;

    [SerializeField]
    Transform[] _pageIconsTrans = default;

    [SerializeField]
    Transform[] _crossKeyImagesTrans = default;

    [SerializeField]
    Image[] _tutorialImages = default;

    [SerializeField]
    Image _gameStartImage = default;
    //[Header("GameDescription")]
    #endregion

    #region private
    CanvasGroup _tutorialGroup = default;
    bool _isPressed = false;
    int _currentPageIndex = 0;
    int _currentPageLength = 0;
    #endregion

    #region public
    public event Action PlayButtonPressAction;
    #endregion

    #region property
    public bool IsActivateTutorial => _tutorialGroup.alpha == 1;
    #endregion

    private void Awake()
    {
        TryGetComponent(out _tutorialGroup);
    }

    private void Start()
    {
        //�u���\���L�[�v�����������̏�����o�^
        this.UpdateAsObservable()
            .Where(_ => IsActivateTutorial && UIInput.LeftCrossKey && !_isPressed)
            .Subscribe(_ => 
            {
                if (_currentPageIndex <= 0)
                {
                    _isPressed = true;
                    _crossKeyImagesTrans[0].transform.DOShakePosition(0.2f, 10, 20)
                                         .OnComplete(() =>
                                         {
                                             _isPressed = false;
                                         });
                    return;
                }
                _currentPageIndex--;
                OnLeftPage();
            })
            .AddTo(this);

        //�u�E�\���L�[�v�����������̏�����o�^
        this.UpdateAsObservable()
            .Where(_ => IsActivateTutorial && UIInput.RightCrossKey && !_isPressed)
            .Subscribe(_ =>
            {
                if (_currentPageIndex >= _currentPageLength - 1)
                {
                    _isPressed = true;
                    _crossKeyImagesTrans[1].transform.DOShakePosition(0.2f, 10, 20)
                                         .OnComplete(() =>
                                         {
                                             _isPressed = false;
                                         });
                    return;
                }
                _currentPageIndex++;
                OnRightPage();
            })
            .AddTo(this);

        this.UpdateAsObservable()
            .Where(_ => IsActivateTutorial && 
                        _currentPageIndex == _currentPageLength - 1 && 
                        UIInput.B && 
                        !_isPressed)
            .Subscribe(_ =>
            {
                _isPressed = true;
                PlayButtonPressAction?.Invoke(); //�o�^���Ă���Scene�J�ڏ������s��
            });
    }

    /// <summary>
    /// �`���[�g���A����ʂ̃Z�b�g�A�b�v
    /// </summary>
    /// <param name="stage"> �\������X�e�[�W </param>
    /// <param name="transitionAction"> �~�j�Q�[��Scene�ɑJ�ڂ��鏈�� </param>
    public void TutorialSetup(Stages stage, Action transitionAction = null)
    {
        if (transitionAction != null)
        {
            PlayButtonPressAction += transitionAction;
        }

        _currentPageIndex = 0;
        _gameStartImage.DOFade(0.3f, 0f);

        var data = _tutorialData.FirstOrDefault(d => d.Stage == stage);

        _currentPageLength = data.DescriptionDatas.Length;
        _currentPageIcon.sprite = data.CurrentPageIcon;

        foreach (var image in _tutorialImages)
        {
            image.enabled = false;
        }

        //���݂̃y�[�W��m�点��A�C�R���̃Z�b�g�A�b�v
        foreach (var icon in _pageIconsTrans)
        {
            icon.gameObject.SetActive(false);
        }

        //�T���v����ʂ̃Z�b�g�A�b�v
        for (int i = 0; i < _currentPageLength; i++)
        {
            _tutorialImages[i].sprite = data.DescriptionDatas[i].PageSprite;
            _pageIconsTrans[i].gameObject.SetActive(true);
        }
        _tutorialImages[_currentPageIndex].enabled = true;
        _currentPageIcon.transform.localPosition = _pageIconsTrans[_currentPageIndex].localPosition;
    }

    /// <summary>
    /// �`���[�g���A����ʂ�ON/OFF��؂�ւ���
    /// </summary>
    /// <param name="isActivate"> �\������\�����ǂ��� </param>
    public void ActivateTutorialUI(bool isActivate)
    {
        if (isActivate)
        {
            _tutorialGroup.alpha = 1;
        }
        else
        {
            _tutorialGroup.alpha = 0;
            PlayButtonPressAction = null;
        }
    }

    /// <summary>
    /// ���݂̃y�[�W�̍��y�[�W��\������
    /// </summary>
    void OnLeftPage()
    {
        Debug.Log($"{_currentPageIndex + 1}�y�[�W�ڂ�\��");
        _isPressed = true;
        
        _tutorialImages[_currentPageIndex].enabled = true;
        _tutorialImages[_currentPageIndex + 1].enabled = false;

        _currentPageIcon.transform.localPosition = _pageIconsTrans[_currentPageIndex].localPosition;
        _crossKeyImagesTrans[0].transform.DOScale(1.2f, 0.1f)
                                         .SetLoops(2, LoopType.Yoyo)
                                         .OnComplete(() =>
                                         {
                                             _isPressed = false;
                                         });

        if (_currentPageIndex < _currentPageLength - 1)
        {
            _gameStartImage.DOFade(0.3f, 0.2f);
        }
    }

    // <summary>
    /// ���݂̃y�[�W�̉E�y�[�W��\������
    /// </summary>
    void OnRightPage()
    {
        Debug.Log($"{_currentPageIndex + 1}�y�[�W�ڂ�\��");

        _isPressed = true;

        _tutorialImages[_currentPageIndex].enabled = true;
        _tutorialImages[_currentPageIndex - 1].enabled = false;

        _currentPageIcon.transform.localPosition = _pageIconsTrans[_currentPageIndex].localPosition;
        _crossKeyImagesTrans[1].transform.DOScale(1.2f, 0.1f)
                                         .SetLoops(2, LoopType.Yoyo)
                                         .OnComplete(() =>
                                         {
                                             _isPressed = false;
                                         });

        if (_currentPageIndex >= _currentPageLength - 1)
        {
            _gameStartImage.DOFade(1f, 0.2f);
        }
    }
}

/// <summary>
/// �X�e�[�W�̃`���[�g���A���f�[�^
/// </summary>
[Serializable]
public struct StageTutorialData
{
    public string StageName;
    public Stages Stage;
    public StageDescriptionData[] DescriptionDatas;
    public Sprite CurrentPageIcon;

}

/// <summary>
/// �`���[�g���A���̐����f�[�^
/// </summary>
[Serializable]
public struct StageDescriptionData
{
    public DescriptionType DescriptionType;
    public Sprite PageSprite;
}

/// <summary>
/// �����̎��
/// </summary>
public enum DescriptionType
{
    /// <summary> �Q�[������ </summary>
    GameDescription,
    /// <summary> ������@ </summary>
    HowToOperate,
    /// <summary> �q���g1 </summary>
    Tips1,
    /// <summary> �q���g2 </summary>
    Tips2,
    /// <summary> �q���g3 </summary>
    Tips3
}