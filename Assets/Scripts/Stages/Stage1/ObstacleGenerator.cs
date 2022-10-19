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

    void Start()
    {
        StartCoroutine(OnGenerate());
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
        while (true)
        {
            int rand = Random.Range(0, _obstacleList.Count);
            Generate(rand);

            yield return new WaitForSeconds(_generateInterval);
        }
    }
}
public enum ObstacleType
{
    Chair,
    Clock,
    Table,
    Mirror
}
