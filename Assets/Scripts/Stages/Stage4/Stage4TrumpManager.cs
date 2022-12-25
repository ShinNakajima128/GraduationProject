using System.Linq;
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
    /// トランプ兵を使用する
    /// </summary>
    /// <param name="type"> トランプ兵の種類 </param>
    /// <param name="target"> 出現位置 </param>
    public void Use(Stage4TrumpDirectionType type, Transform target)
    {
        ActivateTrump(type, target);
    }

    /// <summary>
    /// プーリングしたObjectを全て非アクティブにする
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
    /// 現在アクティブとなっているトランプ兵の数を取得する
    /// </summary>
    /// <returns> 出現しているトランプ兵の数 </returns>
    public int GetTrumpActiveCount()
    {
        int total = 0;

        total += _standingTrumpList.Count(t => t.gameObject.activeSelf);
        total += _walkTrumpList.Count(t => t.gameObject.activeSelf);
        total += _paintTrumpList.Count(t => t.gameObject.activeSelf);
        total += _loafTrumpList.Count(t => t.gameObject.activeSelf);
        total += _dipTrumpList.Count(t => t.gameObject.activeSelf);
        Debug.Log($"トランプ兵の現在のアクティブ数{total}");
        return total;
    }

    /// <summary>
    /// トランプ兵オブジェクトのセットアップ
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
    /// トランプ兵をアクティブにする
    /// </summary>
    /// <param name="type"> トランプの種類 </param>
    /// <param name="target"> 出現位置のTransform </param>
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
                Debug.LogError("使用可能な「直立」のトランプ兵がありませんでした");
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
                Debug.LogError("使用可能な「歩く」のトランプ兵がありませんでした");
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
                Debug.LogError("使用可能な「塗る」のトランプ兵がありませんでした");
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
                Debug.LogError("使用可能な「サボり」のトランプ兵がありませんでした");
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
                Debug.LogError("使用可能な「かき混ぜる」のトランプ兵がありませんでした");
                break;
            default:
                break;
        }
        
    }
}
