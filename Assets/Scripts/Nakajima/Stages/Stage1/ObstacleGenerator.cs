using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �X�e�[�W1�̏�Q���𐶐�����N���X
/// </summary>
public class ObstacleGenerator : MonoBehaviour
{
    #region serialize
    [Header("Variables")]
    [Tooltip("�Q�[���J�n���̏�Q���������Ԃ̊Ԋu")]
    [SerializeField]
    float _generateInterval = 3.0f;

    [SerializeField]
    bool _isBackground = false;

    [Header("Objects")]
    [Tooltip("��Q���𐶐�����ʒu")]
    [SerializeField]
    protected Transform[] _generateTrans = default;

    [Tooltip("���������Q����List")]
    [SerializeField]
    protected List<ObstacleController> _obstacleList = new List<ObstacleController>();
    #endregion
    #region private
    bool _isInGamed;
    bool _isGenerating = false;
    #endregion

    #region property
    public static ObstacleGenerator Instance { get; private set; }
    #endregion

    protected virtual void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (!_isBackground)
        {
            FallGameManager.Instance.GameStart += () => StartCoroutine(OnGenerate());
        }
        else
        {
            StartCoroutine(OnBackgroundGenerate());
        }
        FallGameManager.Instance.GameEnd += StopGenerate;
    }

    protected virtual void Generate(int index)
    {
        int randPos = Random.Range(0, _generateTrans.Length);
        _obstacleList[index].Use(_generateTrans[randPos].position);
    }

    /// <summary>
    /// �����J�n
    /// </summary>
    IEnumerator OnGenerate()
    {
        yield return null;
        _isInGamed = true;

        while (_isInGamed)
        {
            int rand = Random.Range(0, _obstacleList.Count);
            Generate(rand);

            yield return new WaitForSeconds(_generateInterval);
        }
    }

    IEnumerator OnBackgroundGenerate()
    {
        yield return null;
        _isGenerating = true;

        while (_isGenerating)
        {
            int rand = Random.Range(0, _obstacleList.Count);
            Generate(rand);

            yield return new WaitForSeconds(_generateInterval);
        }
        
    }
    IEnumerator StopBackgroundModel()
    {
        yield return new WaitForSeconds(1.5f);

        foreach (var o in _obstacleList)
        {
            o.Return();
        }
        _isGenerating = false;
    }
    /// <summary>
    /// �A�N�e�B�u�̃I�u�W�F�N�g��S�Ĕ�A�N�e�B�u�����A�������I������
    /// </summary>
    void StopGenerate()
    {
        if (!_isBackground)
        {
            foreach (var o in _obstacleList)
            {
                o.Return();
            }
            _isInGamed = false;
        }
        else
        {
            StartCoroutine(StopBackgroundModel());
        }
    }

    /// <summary>
    /// �������Ԃ�ݒ肷��
    /// </summary>
    /// <param name="value"> ��������Ԋu </param>
    public void SetInterval(float value)
    {
        _generateInterval = value;
    }
}
public enum ObstacleType
{
    Chair,
    Clock,
    OldClock,
    BookStand,
    Table,
    Mirror
}
