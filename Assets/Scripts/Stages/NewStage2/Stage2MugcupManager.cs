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
    public void ResetForArray()
    {
        // ID���Ƀ\�[�g
        _mugcups = _mugcups.OrderBy(item => item.ID).ToArray();

        // ���W�̏�����
        for (int index = 0; index < _mugcups.Length; index++)
        {
            _mugcups[index].transform.position = _defaultPositions[index];
        }
    }

    /// <summary>
    /// �V���b�t��
    /// </summary>
    public void Shuffle(ShuffleFase fase, Action action = null)
    {
        Debug.Log("�V���b�t���J�n�̃��N�G�X�g");
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
}
