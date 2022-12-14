using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// オブジェクトをプーリングする抽象クラス
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class ObjectPool<T> : MonoBehaviour
{
    #region serialize
    [Tooltip("プーリングするObject")]
    [SerializeField]
    protected GameObject _objectPrefab = default;

    [Tooltip("プーリングする数")]
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
        //指定した数のオブジェクトを生成し、プーリングする
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
                //Debug.LogError("コンポーネントが見つかりませんでした");
            }
            
            obj.SetActive(false);
        }
    }
    /// <summary>
    /// プーリングしたObjectをアクティブにする
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

                //ObjectのComponentを使用する側へ渡す
                var component = go.GetComponentInChildren<T>();
                if (component != null)
                {
                    callback?.Invoke(component);
                }
                return;
            }
        }
        Debug.LogError("使用可能なObjectがありませんでした");
    }

    /// <summary>
    /// プーリングしたObjectを全て非アクティブにする
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
