using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ステージ1の障害物を生成するクラス
/// </summary>
public class ObstacleGenerator : MonoBehaviour
{
    #region serialize
    [Tooltip("ゲーム開始時の障害物生成時間の間隔")]
    [SerializeField]
    float _generateInterval = 3.0f;

    [Tooltip("障害物を生成する位置")]
    [SerializeField]
    Transform[] _generateTrans = default;

    [Tooltip("生成する障害物のList")]
    [SerializeField]
    List<ObstacleController> _obstacleList = new List<ObstacleController>();
    #endregion
    #region private
    bool _isInGamed;
    #endregion

    void Start()
    {
        FallGameManager.Instance.GameStart +=() => StartCoroutine(OnGenerate());
        FallGameManager.Instance.GameEnd += StopGenerate;
    }

    void Generate(int index)
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
    /// <summary>
    /// アクティブのオブジェクトを全て非アクティブ化し、生成を終了する
    /// </summary>
    void StopGenerate()
    {
        foreach (var o in _obstacleList)
        {
            o.Return();
        }
        _isInGamed = false;
    }
}
public enum ObstacleType
{
    Chair,
    Clock,
    Table,
    Mirror
}
