using System;
using System.Linq;
using UnityEngine;

public class Stage2MugcupManager : MonoBehaviour
{
    [SerializeField]
    private Stage2MugcupController[] _mugcups;

    [SerializeField]
    private Stage2MugcupShuffler _shuffler;

    [Header("�ŏ��Ƀl�Y�~�������Ă�ꏊ(Index)")]
    [SerializeField]
    private int _indexOfInMouse;

    [Header("�V���b�t���̎���")]
    [SerializeField]
    private float _shuffleTime;

    private Vector3[] _defaultPositions;

    public bool IsOpened { get; set; }

    private void Awake()
    {
        _defaultPositions = new Vector3[_mugcups.Length];

        for (int index = 0; index < _mugcups.Length; index++)
        {
            _mugcups[index].ID = index;
            _defaultPositions[index] = _mugcups[index].transform.position;
        }
    }

    /// <summary>
    /// �V���b�t���O�̏�Ԃɂ���
    /// </summary>
    public void Initialise(Action action)
    {
        _mugcups[_indexOfInMouse].IsInMouse = true;
        _mugcups[_indexOfInMouse].CupDownRequest(action);
    }

    /// <summary>
    /// �z��̒��g��������Ԃɖ߂�
    /// </summary>
    public void ResetForArray(Action action = null)
    {
        // ID���Ƀ\�[�g
        _mugcups = _mugcups.OrderBy(item => item.ID).ToArray();

        // ���W�̏�����
        for (int index = 0; index < _mugcups.Length; index++)
        {
            _mugcups[index].transform.position = _defaultPositions[index];
        }

        action?.Invoke();
    }

    /// <summary>
    /// �S�ẴJ�b�v��������
    /// </summary>
    public void OpenAllMugCup(Action action = null)
    {
        Debug.Log("All Opened");
        IsOpened = true;

        for (int i = 0; i < _mugcups.Length; i++)
        {
            if (i == 5)
            {
                _mugcups[i].CupUpRequest(action);
                continue;
            }
            _mugcups[i].CupUpRequest();
        }
    }

    /// <summary>
    /// �S�ẴJ�b�v��������
    /// </summary>
    public void CloseAllMugCup(Action action = null)
    {
        Debug.Log("All Closed");

        for (int i = 0; i < _mugcups.Length; i++)
        {
            if (i == _mugcups.Length - 1)
            {
                //  �Ō�̃J�b�v�������肫������
                _mugcups[i].CupDownRequest(() => 
                {
                    action();
                    IsOpened = false;
                });

                continue;
            }
            _mugcups[i].CupDownRequest();
        }
    }

    /// <summary>
    /// �V���b�t��
    /// </summary>
    public void BeginShuffle(ShufflePhase fase, Action action = null)
    {
        Debug.Log("�V���b�t���J�n�̃��N�G�X�g");
        // �󂢂Ă�����A�S�ĕ��Ă���V���b�t��������
        if (IsOpened)
        {
            CloseAllMugCup(() => _shuffler.ShuffleRequest(fase, _mugcups, _shuffleTime, action));
            return;
        }
        
        _shuffler.ShuffleRequest(fase, _mugcups, _shuffleTime, action);
    }

    /// <summary>
    /// �l�Y�~�������Ă���ԍ���Ԃ�
    /// </summary>
    public int GetInMouseCupNumber()
    {
        for (int i = 0; i < _mugcups.Length; i++)
        {
            // �l�Y�~�������Ă���ԍ���Ԃ�
            if (_mugcups[i].IsInMouse) return i;
        }
        return -1;
    }

    /// <summary>
    /// �J�b�v��������
    /// </summary>
    public void OpenRequest(int index, Action action = null)
    {
        Debug.Log($"{index}�̃J�b�v��������");
        _mugcups[index].CupUpRequest(action);
    }

    /// <summary>
    /// �J�b�v��������
    /// </summary>
    public void CloseRequest(int index, Action action = null)
    {
        IsOpened = true;
        Debug.Log($"{index}�̃J�b�v��������");
        _mugcups[index].CupDownRequest(action);
    }
}
