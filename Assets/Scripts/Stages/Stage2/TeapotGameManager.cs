using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �e�B�[�J�b�v�Q�[�����Ǘ�����Manager�N���X
/// </summary>
public class TeapotGameManager : MonoBehaviour
{
    #region serialize
    [Tooltip("�e�B�[�J�b�v��I������{�^����Panel")]
    [SerializeField]
    GameObject _selectButtonPanel = default;

    [Tooltip("���U���g��ʂ�Panel")]
    [SerializeField]
    GameObject _resultPanel = default;

    [Tooltip("�I������Button")]
    [SerializeField]
    Button[] _selectButtons = default;

    [Tooltip("������x�v���C����p��Button")]
    [SerializeField]
    Button _replayButton = default;

    [Tooltip("���ʂ�\������Text")]
    [SerializeField]
    Text _resultText = default;
    #endregion

    #region private
    /// <summary> �����̈ʒu�̔ԍ� </summary>
    int _correct = 0;
    #endregion
    #region property
    public static TeapotGameManager Instance { get; private set; }
    public Action GameStart { get; set; }
    public Action TeapotSelectPhase { get; set; }
    public Action<int, bool> OpenTeacupPhase { get; set; }
    public Action GameEnd { get; set; }
    public Action Initialize { get; set; }
    #endregion

    private void Awake()
    {
        Instance = this;
    }
    IEnumerator Start()
    {
        _replayButton.onClick.AddListener(() => OnGameStart());
        yield return null;
        OnGameStart();
    }

    /// <summary>
    /// �e�B�[�|�b�g��I�������ʂ�\������
    /// </summary>
    public void OnSelectTeacup(int correctNum)
    {
        _selectButtonPanel.SetActive(true);
        TeapotSelectPhase?.Invoke();
        _correct = correctNum;
    }

    /// <summary>
    /// �Q�[�����J�n����
    /// </summary>
    void OnGameStart()
    {
        InitializeObject();
        GameStart?.Invoke();
    }
    /// <summary>
    /// �Q�[�����I������
    /// </summary>
    void OnGameEnd()
    {
        GameEnd?.Invoke();
    }
    /// <summary>
    /// �I�u�W�F�N�g�̏�����
    /// </summary>
    void InitializeObject()
    {
        _selectButtonPanel.SetActive(false);
        _resultPanel.SetActive(false);
        Initialize?.Invoke();
    }
    /// <summary>
    /// Button��I��
    /// </summary>
    /// <param name="buttonNum"> �I������Button�̔ԍ� </param>
    public void Select(int buttonNum)
    {
        StartCoroutine(OnResult(buttonNum));
    }

    IEnumerator OnResult(int buttonNum)
    {
        var waitTime = new WaitForSeconds(1f);
        bool result = _correct == buttonNum;

        OpenTeacupPhase?.Invoke(buttonNum, result);
        _selectButtonPanel.SetActive(false);

        yield return waitTime;

        if (result) //�����̈ʒu�ƑI�������{�^���̔ԍ��������ꍇ
        {
            _resultText.text = "������I";
        }
        else
        {
            _resultText.text = "�͂���";
        }

        yield return waitTime;

        _resultPanel.SetActive(true);�@//���U���g��ʕ\��
    }
}
