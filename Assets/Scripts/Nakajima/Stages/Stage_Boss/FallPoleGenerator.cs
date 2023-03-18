using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���𐶐�����@�\�����R���|�[�l���g
/// </summary>
public class FallPoleGenerator : MonoBehaviour
{
    #region serialize
    [Header("Variables")]
    [SerializeField]
    int _generateCount = 3;

    [SerializeField]
    Transform[] _generatePoints = default;

    [Header("Components")]
    [SerializeField]
    FallPoleController _fallpoleCtrl = default;
    #endregion

    #region private
    #endregion

    #region public
    #endregion

    #region property
    #endregion

    /// <summary>
    /// ���𐶐�����
    /// </summary>
    /// <param name="generateCount"> �������鐔 </param>
    public void Generate(int generateCount = 0)
    {
        //���������w�肳��Ă��Ȃ��ꍇ��Generator�ɐݒ肳��Ă��鐔�𔽉f
        if (generateCount == 0)
        {
            generateCount = _generateCount;
        }
        else if (generateCount > _generatePoints.Length)
        {
            generateCount = _generatePoints.Length;
        }

        for (int i = 0; i < generateCount; i++)
        {
            _fallpoleCtrl.Use(_generatePoints[i]);
        }
    }

    /// <summary>
    /// �g�p���̒���S�Ĕ�A�N�e�B�u�ɂ���
    /// </summary>
    public void Return()
    {
        _fallpoleCtrl.Return();
    }
}
