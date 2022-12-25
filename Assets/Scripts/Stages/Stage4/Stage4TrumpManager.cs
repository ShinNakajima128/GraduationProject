using System.Linq;
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
    Transform[] _trumpParents = default;

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
    List<Stage4TrumpSolder> _standingTrumpList = new List<Stage4TrumpSolder>();
    List<Stage4TrumpSolder> _walkTrumpList = new List<Stage4TrumpSolder>();
    List<Stage4TrumpSolder> _paintTrumpList = new List<Stage4TrumpSolder>();
    List<Stage4TrumpSolder> _loafTrumpList = new List<Stage4TrumpSolder>();
    List<Stage4TrumpSolder> _dipTrumpList = new List<Stage4TrumpSolder>();
    #endregion
    #region public
    #endregion
    #region property
    #endregion


    private void Awake()
    {
        Setup();
    }

    /// <summary>
    /// �g�����v�����g�p����
    /// </summary>
    /// <param name="type"> �g�����v���̎�� </param>
    /// <param name="target"> �o���ʒu </param>
    public void Use(Stage4TrumpDirectionType type, Transform target)
    {
        ActivateTrump(type, target);
    }

    /// <summary>
    /// �v�[�����O����Object��S�Ĕ�A�N�e�B�u�ɂ���
    /// </summary>
    /// <returns></returns>
    public void Return()
    {
        foreach (var t in _standingTrumpList)
        {
            t.gameObject.SetActive(false);
        }

        foreach (var t in _walkTrumpList)
        {
            t.gameObject.SetActive(false);
        }

        foreach (var t in _paintTrumpList)
        {
            t.gameObject.SetActive(false);
        }

        foreach (var t in _loafTrumpList)
        {
            t.gameObject.SetActive(false);
        }

        foreach (var t in _dipTrumpList)
        {
            t.gameObject.SetActive(false);
        }

    }

    /// <summary>
    /// ���݃A�N�e�B�u�ƂȂ��Ă���g�����v���̐����擾����
    /// </summary>
    /// <returns> �o�����Ă���g�����v���̐� </returns>
    public int GetTrumpActiveCount()
    {
        int total = 0;

        total += _standingTrumpList.Count(t => t.gameObject.activeSelf);
        total += _walkTrumpList.Count(t => t.gameObject.activeSelf);
        total += _paintTrumpList.Count(t => t.gameObject.activeSelf);
        total += _loafTrumpList.Count(t => t.gameObject.activeSelf);
        total += _dipTrumpList.Count(t => t.gameObject.activeSelf);
        Debug.Log($"�g�����v���̌��݂̃A�N�e�B�u��{total}");
        return total;
    }

    /// <summary>
    /// �g�����v���I�u�W�F�N�g�̃Z�b�g�A�b�v
    /// </summary>
    void Setup()
    {
        for (int i = 0; i < _standingPoolingCount; i++)
        {
            var trump = Instantiate(_standingTrumpPrefab, _trumpParents[0]);
            _standingTrumpList.Add(trump);
            trump.gameObject.SetActive(false);
        }

        for (int i = 0; i < _walkPoolingCount; i++)
        {
            var trump = Instantiate(_walkTrumpPrefab, _trumpParents[1]);
            _walkTrumpList.Add(trump);
            trump.gameObject.SetActive(false);
        }

        for (int i = 0; i < _paintPoolingCount; i++)
        {
            var trump = Instantiate(_paintTrumpPrefab, _trumpParents[2]);
            _paintTrumpList.Add(trump);
            trump.gameObject.SetActive(false);
        }

        for (int i = 0; i < _loafPoolingCount; i++)
        {
            var trump = Instantiate(_loafTrumpPrefab, _trumpParents[3]);
            _loafTrumpList.Add(trump);
            trump.gameObject.SetActive(false);
        }

        for (int i = 0; i < _dipPoolingCount; i++)
        {
            var trump = Instantiate(_dipTrumpPrefab, _trumpParents[4]);
            _dipTrumpList.Add(trump);
            trump.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// �g�����v�����A�N�e�B�u�ɂ���
    /// </summary>
    /// <param name="type"> �g�����v�̎�� </param>
    /// <param name="target"> �o���ʒu��Transform </param>
    void ActivateTrump(Stage4TrumpDirectionType type, Transform target)
    {
        switch (type)
        {
            case Stage4TrumpDirectionType.Standing:
                foreach (var t in _standingTrumpList)
                {
                    if (!t.gameObject.activeSelf)
                    {
                        t.gameObject.SetActive(true);
                        t.transform.localPosition = target.position;
                        t.transform.localRotation = target.localRotation;
                        return;
                    }
                }
                Debug.LogError("�g�p�\�ȁu�����v�̃g�����v��������܂���ł���");
                break;
            case Stage4TrumpDirectionType.Walk:
                foreach (var t in _walkTrumpList)
                {
                    if (!t.gameObject.activeSelf)
                    {
                        t.gameObject.SetActive(true);
                        t.transform.localPosition = target.position;
                        t.transform.localRotation = target.localRotation;
                        return;
                    }
                }
                Debug.LogError("�g�p�\�ȁu�����v�̃g�����v��������܂���ł���");
                break;
            case Stage4TrumpDirectionType.Paint:
                foreach (var t in _paintTrumpList)
                {
                    if (!t.gameObject.activeSelf)
                    {
                        t.gameObject.SetActive(true);
                        t.transform.localPosition = target.position;
                        t.transform.localRotation = target.localRotation;
                        return;
                    }
                }
                Debug.LogError("�g�p�\�ȁu�h��v�̃g�����v��������܂���ł���");
                break;
            case Stage4TrumpDirectionType.Loaf:
                foreach (var t in _loafTrumpList)
                {
                    if (!t.gameObject.activeSelf)
                    {
                        t.gameObject.SetActive(true);
                        t.transform.localPosition = target.position;
                        t.transform.localRotation = target.localRotation;
                        return;
                    }
                }
                Debug.LogError("�g�p�\�ȁu�T�{��v�̃g�����v��������܂���ł���");
                break;
            case Stage4TrumpDirectionType.Dip:
                foreach (var t in _dipTrumpList)
                {
                    if (!t.gameObject.activeSelf)
                    {
                        t.gameObject.SetActive(true);
                        t.transform.localPosition = target.position;
                        t.transform.localRotation = target.localRotation;
                        return;
                    }
                }
                Debug.LogError("�g�p�\�ȁu����������v�̃g�����v��������܂���ł���");
                break;
            default:
                break;
        }
        
    }
}
