using System;
using UnityEngine;

public class Stage2MugcupManager : MonoBehaviour
{
    [SerializeField]
    private Stage2MugcupController[] _mugcups;

    [SerializeField]
    private Stage2MugcupShuffler _shuffler;

    [SerializeField]
    private float _shuffleTime;

    private Vector3[] _defaultPositions;

    private void Awake()
    {
        _defaultPositions = new Vector3[_mugcups.Length];

        for (int i = 0; i < _mugcups.Length; i++)
        {
            _mugcups[i].ID = i;
            _defaultPositions[i] = _mugcups[i].transform.position;
        }
    }

    /// <summary>
    /// �V���b�t���O�̏�Ԃɂ���
    /// </summary>
    public void Initialise(Action action)
    {
        _mugcups[0].IsInMouse = true;
        _mugcups[0].CupDownRequest(action);
    }

    /// <summary>
    /// �z��̒��g��������Ԃɖ߂�
    /// </summary>
    public void ResetForArray()
    {
        Debug.Log("Reset");
        for (int i = 0; i < _mugcups.Length; i++)
        {
            for (int x = 0; x < _mugcups.Length; x++)
            {
                if (i == _mugcups[x].ID)
                {
                    var tmp = _mugcups[i];
                    _mugcups[i] = _mugcups[x];
                    _mugcups[x] = tmp;
                    break;
                }
            }
            // ���W�̃��Z�b�g
            _mugcups[i].gameObject.transform.position = _defaultPositions[i];
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
