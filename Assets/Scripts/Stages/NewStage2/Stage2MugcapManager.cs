using System;
using UnityEngine;

public class Stage2MugcapManager : MonoBehaviour
{
    [SerializeField] // [0]�̓}�E�X�������Ă���
    private Stage2MugcapController[] _mugcapArray;

    private Stage2MugcapShuffle _shuffle;

    private void Awake()
    {
        _shuffle = new();
    }

    public void Setup(Action action = null)
    {
        // �J�b�v��������
        _mugcapArray[0].MoveDownRequest(action);
    }
}
