using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stage4�̃g�����v���̊Ǘ�������}�l�[�W���[
/// </summary>
public class Stage4TrumpManager : MonoBehaviour
{
    #region serialize
    [Header("Variables")]
    [Tooltip("�u�����v�g�����v�����v�[�����鐔")]
    [SerializeField]
    int _standingPoolingCount = 5;

    [Tooltip("�u�����v�g�����v�����v�[�����鐔")]
    [SerializeField]
    int _walkPoolingCount = 5;

    [Tooltip("�u�h��v�g�����v�����v�[�����鐔")]
    [SerializeField]
    int _paintPoolingCount = 5;

    [Tooltip("�u�T�{��v�g�����v�����v�[�����鐔")]
    [SerializeField]
    int _loafPoolingCount = 5;

    [Tooltip("�u�y���L������������v�g�����v�����v�[�����鐔")]
    [SerializeField]
    int _dipPoolingCount = 5;

    [Header("Objects")]
    [SerializeField]
    Stage4TrumpSolder _standingTrumpPrefab = default;

    [SerializeField]
    Stage4TrumpSolder _walkTrumpPrefab = default;

    [SerializeField]
    Stage4TrumpSolder _paintTrumpPrefab = default;

    [SerializeField]
    Stage4TrumpSolder _loafTrumpPrefab = default;

    [SerializeField]
    Stage4TrumpSolder _dipTrumpPrefab = default;
    #endregion

    #region private
    #endregion
    #region public
    #endregion
    #region property
    #endregion

    void Start()
    {
        
    }

    public void Use()
    {

    }

    /// <summary>
    /// �v�[�����O����Object��S�Ĕ�A�N�e�B�u�ɂ���
    /// </summary>
    /// <returns></returns>
    public void Return()
    {
        //foreach (var go in _objectList)
        //{
        //    go.SetActive(false);
        //}
    }
}
