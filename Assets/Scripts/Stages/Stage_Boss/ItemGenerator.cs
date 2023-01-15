using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGenerator : MonoBehaviour
{
    #region serialize
    [Header("Variables")]
    [Tooltip("�A�C�e���𐶐����鐔")]
    [SerializeField]
    int _generateCount = 3;

    [Tooltip("��x�ɐ������鐔�̌��E�l")]
    [SerializeField]
    int _currentGenerateLimit = 5;

    [Header("Components")]
    [Tooltip("�񕜃A�C�e�����Ǘ�����Controller")]
    [SerializeField]
    ItemController _itemCtrl = default;
    #endregion

    #region private
    #endregion

    #region public
    #endregion

    #region property
    public static ItemGenerator Instance { get; private set; }
    #endregion

    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// ��������
    /// </summary>
    /// <param name="generatePos"> ��������ʒu </param>
    public void Generate(Vector3 generatePos)
    {
        //���ɃA�N�e�B�u�ƂȂ��Ă��鐔�����ȏ�̏ꍇ�͏������s��Ȃ�
        if (_currentGenerateLimit <= _itemCtrl.CurrentActiveCount)
        {
            print("���Ɏw��ȏ�̐�����������Ă��܂�");
            return;
        }
        _itemCtrl.Use(generatePos);
    }

    /// <summary>
    /// �������̃A�C�e����S�Ĕ�A�N�e�B�u�ɂ���
    /// </summary>
    public void Return()
    {
        _itemCtrl.Return();
    }
}
