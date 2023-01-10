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

    #region property
    public static WhitePaperGenerator Instance { get; private set; }
    #endregion

    private void Awake()
    {
        Instance = this;
    }

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

    /// <summary>
    /// アクティブのオブジェクトを全て非アクティブ化し、生成を終了する
    /// </summary>
    void StopGenerate()
    {
        _wpc.Return();
        _isInGamed = false;
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
