using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �I�u�W�F�N�g���v�[�����O���钊�ۃN���X
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class ObjectPool<T> : MonoBehaviour
{
    #region serialize
    [Tooltip("�v�[�����O����Object")]
    [SerializeField]
    protected GameObject _objectPrefab = default;

    [Tooltip("�v�[�����O���鐔")]
    [SerializeField]
    int _poolingCount = 0;
    #endregion

    #region private
    List<GameObject> _objectList = new List<GameObject>();
    List<T> _componentList = new List<T>();
    #endregion

    #region property
    public List<T> ComponentList => _componentList;
    #endregion

    protected virtual void Start()
    {
        //�w�肵�����̃I�u�W�F�N�g�𐶐����A�v�[�����O����
        for (int i = 0; i < _poolingCount; i++)
        {
            var obj = Instantiate(_objectPrefab, transform);
            _objectList.Add(obj);

            if (obj.TryGetComponent<T>(out var com))
            {
                _componentList.Add(com);
            }
            else
            {
                //Debug.LogError("�R���|�[�l���g��������܂���ł���");
            }
            
            obj.SetActive(false);
        }
    }
    /// <summary>
    /// �v�[�����O����Object���A�N�e�B�u�ɂ���
    /// </summary>
    /// <returns></returns>
    public void Use(Vector3 pos, Action<T> callback = null)
    {
        foreach (var go in _objectList)
        {
            if (!go.activeSelf)
            {
                go.SetActive(true);
                go.transform.localPosition = pos;

                //Object��Component���g�p���鑤�֓n��
                var component = go.GetComponentInChildren<T>();
                if (component != null)
                {
                    callback?.Invoke(component);
                }
                return;
            }
        }
        Debug.LogError("�g�p�\��Object������܂���ł���");
    }

    /// <summary>
    /// �v�[�����O����Object��S�Ĕ�A�N�e�B�u�ɂ���
    /// </summary>
    /// <returns></returns>
    public void Return()
    {
        foreach (var go in _objectList)
        {
            go.SetActive(false);
        }
    }
}
