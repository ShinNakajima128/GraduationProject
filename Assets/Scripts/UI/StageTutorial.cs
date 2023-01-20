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
    [SerializeField]
    Image _tutorialBackground = default;

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

    [Header("Description")]
    [SerializeField]
    GameObject[] _descriptionPanels = default;
    #endregion

    #region private
    CanvasGroup _tutorialGroup = default;
    bool _isPressed = false;
    int _currentPageIndex = 0;
    int _currentPageLength = 0;
    Dictionary<DescriptionType, GameObject> _descriptionPanelDic = new Dictionary<DescriptionType, GameObject>();
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

        //������ʂ�Dictionary���쐬
        for (int i = 0; i < (int)DescriptionType.Num; i++)
        {
            _descriptionPanelDic.Add((DescriptionType)i, _descriptionPanels[i]);
            _descriptionPanelDic[(DescriptionType)i].SetActive(false);
        }
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

        //�Ō�̃y�[�W��\�����Ă���ꍇ�̃Q�[���X�^�[�g������o�^
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

        foreach (var image in _tutorialImages)
        {
            image.enabled = false;
        }

        foreach (var panel in _descriptionPanelDic)
        {
            panel.Value.SetActive(false);
        }

        //���݂̃y�[�W��m�点��A�C�R���̃Z�b�g�A�b�v
        foreach (var icon in _pageIconsTrans)
        {
            icon.gameObject.SetActive(false);
        }

        var data = _tutorialData.FirstOrDefault(d => d.Stage == stage);

        SetDescription(data); //�X�e�[�W�ڍ׃f�[�^��UI�ɔ��f
        _tutorialBackground.sprite = data.TutorialBackground; //�w�i�摜���Z�b�g
        _currentPageLength = data.DescriptionDatas.Length; //�y�[�W�̒������擾
        _currentPageIcon.sprite = data.CurrentPageIcon; //�X�e�[�W���Ƃ̃y�[�W�A�C�R�����Z�b�g

        //�T���v����ʂ̃Z�b�g�A�b�v
        for (int i = 0; i < _currentPageLength; i++)
        {
            _tutorialImages[i].sprite = data.DescriptionDatas[i].PageSprite;
            _pageIconsTrans[i].gameObject.SetActive(true);
        }
        _tutorialImages[_currentPageIndex].enabled = true;
        SetCurrentPageIcon();
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
        
        //�����̃Q�[�����Image��؂�ւ���
        _tutorialImages[_currentPageIndex].enabled = true;
        _tutorialImages[_currentPageIndex + 1].enabled = false;

        //�E���̐�����ʂ�؂�ւ���
        _descriptionPanelDic[(DescriptionType)_currentPageIndex].SetActive(true);
        _descriptionPanelDic[(DescriptionType)_currentPageIndex + 1].SetActive(false);

        _currentPageIcon.transform.position = _pageIconsTrans[_currentPageIndex].position;
        _crossKeyImagesTrans[0].transform.DOScale(1.2f, 0.1f)
                                         .SetLoops(2, LoopType.Yoyo)
                                         .OnComplete(() =>
                                         {
                                             _isPressed = false;
                                         });

        //�Ō�̃y�[�W�ȊO�̏ꍇ�̓Q�[���X�^�[�g�{�^�����A�N�e�B�u�ɂ���
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

        //�����̃Q�[�����Image��؂�ւ���
        _tutorialImages[_currentPageIndex].enabled = true;
        _tutorialImages[_currentPageIndex - 1].enabled = false;

        //�E���̐�����ʂ�؂�ւ���
        _descriptionPanelDic[(DescriptionType)_currentPageIndex].SetActive(true);
        _descriptionPanelDic[(DescriptionType)_currentPageIndex - 1].SetActive(false);

        _currentPageIcon.transform.position = _pageIconsTrans[_currentPageIndex].position;
        _crossKeyImagesTrans[1].transform.DOScale(1.2f, 0.1f)
                                         .SetLoops(2, LoopType.Yoyo)
                                         .OnComplete(() =>
                                         {
                                             _isPressed = false;
                                         });

        //�Ō�̃y�[�W�̏ꍇ�̓Q�[���X�^�[�g�{�^�����A�N�e�B�u�ɂ���
        if (_currentPageIndex >= _currentPageLength - 1)
        {
            _gameStartImage.DOFade(1f, 0.2f);
        }
    }

    void SetDescription(StageTutorialData data)
    {
        _descriptionPanelDic[DescriptionType.GameDescription].SetActive(true);
        //for (int i = 0; i < data.DescriptionDatas.Length; i++)
        //{

        //}
    }

    /// <summary>
    /// ���݂̃y�[�W��\���A�C�R���̈ʒu���Z�b�g����
    /// </summary>
    void SetCurrentPageIcon()
    {
        StartCoroutine(SetCurrentPageIconCoroutine());
    }

    IEnumerator SetCurrentPageIconCoroutine()
    {
        yield return null;

        _currentPageIcon.transform.position = _pageIconsTrans[_currentPageIndex].position;
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
    public Sprite TutorialBackground;

}

/// <summary>
/// �`���[�g���A���̐����f�[�^
/// </summary>
[Serializable]
public struct StageDescriptionData
{
    public string DescriptionName;
    public DescriptionType DescriptionType;
    public Sprite PageSprite;
    [TextArea(1, 5)]
    public string DescriptionText;
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
    Tips3,
    Num
}