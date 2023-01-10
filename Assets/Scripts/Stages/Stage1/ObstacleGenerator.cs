using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ステージ1の障害物を生成するクラス
/// </summary>
public class ObstacleGenerator : MonoBehaviour
{
    #region serialize
    [Header("Variables")]
    [Tooltip("ゲーム開始時の障害物生成時間の間隔")]
    [SerializeField]
    float _generateInterval = 3.0f;

    [SerializeField]
    bool _isBackground = false;

    [Header("Objects")]
    [Tooltip("障害物を生成する位置")]
    [SerializeField]
    protected Transform[] _generateTrans = default;

    [Tooltip("生成する障害物のList")]
    [SerializeField]
    protected List<ObstacleController> _obstacleList = new List<ObstacleController>();
    #endregion
    #region private
    bool _isInGamed;
    bool _isGenerating = false;
    #endregion

    #region property
    public static ObstacleGenerator Instance { get; private set; }
    #endregion

    protected virtual void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (!_isBackground)
        {
            FallGameManager.Instance.GameStart += () => StartCoroutine(OnGenerate());
        }
        else
        {
            StartCoroutine(OnBackgroundGenerate());
        }
        FallGameManager.Instance.GameEnd += StopGenerate;
    }

    protected virtual void Generate(int index)
    {
        int randPos = Random.Range(0, _generateTrans.Length);
        _obstacleList[index].Use(_generateTrans[randPos].position);
    }

    /// <summary>
    /// 生成開始
    /// </summary>
    IEnumerator OnGenerate()
    {
        yield return null;
        _isInGamed = true;

        while (_isInGamed)
        {
            int rand = Random.Range(0, _obstacleList.Count);
            Generate(rand);

            yield return new WaitForSeconds(_generateInterval);
        }
    }

    IEnumerator OnBackgroundGenerate()
    {
        yield return null;
        _isGenerating = true;

        while (_isGenerating)
        {
            int rand = Random.Range(0, _obstacleList.Count);
            Generate(rand);

            yield return new WaitForSeconds(_generateInterval);
        }
        
    }
    IEnumerator StopBackgroundModel()
    {
        yield return new WaitForSeconds(1.5f);

        foreach (var o in _obstacleList)
        {
            o.Return();
        }
        _isGenerating = false;
    }
    /// <summary>
    /// アクティブのオブジェクトを全て非アクティブ化し、生成を終了する
    /// </summary>
    void StopGenerate()
    {
        if (!_isBackground)
        {
            foreach (var o in _obstacleList)
            {
                o.Return();
            }
            _isInGamed = false;
        }
        else
        {
            StartCoroutine(StopBackgroundModel());
        }
    }

    /// <summary>
    /// 生成時間を設定する
    /// </summary>
    /// <param name="value"> 生成する間隔 </param>
    public void SetInterval(float value)
    {
        _generateInterval = value;
    }
}
public enum ObstacleType
{
    Chair,
    Clock,
    OldClock,
    BookStand,
    Table,
    Mirror
}
