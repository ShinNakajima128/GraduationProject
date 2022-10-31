using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhitePaperGenerator : MonoBehaviour
{
    #region serialize
    [Tooltip("ゲーム開始時の白紙生成時間の間隔")]
    [SerializeField]
    float _generateInterval = 3.0f;

    [Tooltip("白紙を生成する位置")]
    [SerializeField]
    Transform[] _generateTrans = default;

    [Tooltip("生成する白紙のController")]
    [SerializeField]
    WhitePaperController _wpc = default;
    #endregion

    #region private
    bool _isInGamed;
    #endregion

    void Start()
    {
        FallGameManager.Instance.GameStart += () => StartCoroutine(OnGenerate());
        FallGameManager.Instance.GameEnd += StopGenerate;
    }
    void Generate()
    {
        int randPos = Random.Range(0, _generateTrans.Length);
        _wpc.Use(_generateTrans[randPos].position);
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
            Generate();

            yield return new WaitForSeconds(_generateInterval);
        }
    }

    void StopGenerate()
    {
        _isInGamed = false;
    }
}
