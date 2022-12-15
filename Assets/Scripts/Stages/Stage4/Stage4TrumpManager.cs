using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stage4のトランプ兵の管理をするマネージャー
/// </summary>
public class Stage4TrumpManager : MonoBehaviour
{
    #region serialize
    [Header("Variables")]
    [Tooltip("「直立」トランプ兵をプールする数")]
    [SerializeField]
    int _standingPoolingCount = 5;

    [Tooltip("「歩く」トランプ兵をプールする数")]
    [SerializeField]
    int _walkPoolingCount = 5;

    [Tooltip("「塗る」トランプ兵をプールする数")]
    [SerializeField]
    int _paintPoolingCount = 5;

    [Tooltip("「サボり」トランプ兵をプールする数")]
    [SerializeField]
    int _loafPoolingCount = 5;

    [Tooltip("「ペンキをかき混ぜる」トランプ兵をプールする数")]
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
    /// プーリングしたObjectを全て非アクティブにする
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
