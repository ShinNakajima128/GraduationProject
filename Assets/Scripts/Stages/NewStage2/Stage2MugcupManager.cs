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

    /// <summary>
    /// �V���b�t���O�̏�Ԃɂ���
    /// </summary>
    public void Initialise(Action action)
    {
        _mugcups[0].IsInMouse = true;
        _mugcups[0].CupDownRequest(action);
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
