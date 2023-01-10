using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebrisGenerator : MonoBehaviour
{
    #region serialize
    [Header("Variables")]
    [Tooltip("がれきを生成する数")]
    [SerializeField]
    int _generateCount = 3;

    [Tooltip("同じタイミングで生成したがれきの生成間隔")]
    [SerializeField]
    float _sometimeGenerateInterval = 0.2f;

    [Header("Objects")]
    [Tooltip("障害物を生成する位置")]
    [SerializeField]
    protected Transform[] _generateTrans = default;

    [Header("Components")]
    [Tooltip("がれきのController")]
    [SerializeField]
    DebrisController _debrisCtrl = default;

    [Tooltip("がれきの影のController")]
    [SerializeField]
    DebrisShadowController _debrisShadowCtrl = default;

    [Header("Debug")]
    [SerializeField]
    bool _debugMode = false;

    [SerializeField]
    float _debugGenerateInterval = 3.0f;
    #endregion

    #region private
    int[] _generateTransIndex;
    Coroutine _generateCoroutine;
    #endregion

    #region public
    #endregion

    #region property
    #endregion

    private void Awake()
    {
        _generateTransIndex = new int[_generateTrans.Length];
    }

    private void Start()
    {
        if (_debugMode)
        {
            StartCoroutine(GenerateCoroutine());
        }
    }
    /// <summary>
    /// がれきを生成する
    /// </summary>
    /// <param name="generateCount"> 生成する数 </param>
    public void Generate(int generateCount)
    {
        bool isSet;
        int randomIndex;

        ResetGenerateIndex();

        for (int i = 0; i < _generateTransIndex.Length; i++)
        {
            isSet = false;

            //番号がセット完了するまでループ
            while (!isSet)
            {
                randomIndex = Random.Range(0, _generateTrans.Length); //ランダムな要素番号を取得

                bool result = _generateTransIndex.Any(i => randomIndex == i); //既に同じ番号がないか確認

                //同じ番号がない場合は更新
                if (!result)
                {
                    _generateTransIndex[i] = randomIndex;
                    isSet = true;
                }
            }
        }

        StartCoroutine(GenerateIntervalCoroutine(generateCount));
    }

    /// <summary>
    /// 使用中のがれきを全て非アクティブにする
    /// </summary>
    public void Return()
    {
        _debrisCtrl.Return();
    }

    /// <summary>
    /// がれきを生成開始する
    /// </summary>
    public void StartGenerate()
    {
        _generateCoroutine = StartCoroutine(GenerateCoroutine());
    }

    /// <summary>
    /// がれきの生成を停止する
    /// </summary>
    public void StopGenerate()
    {
        if (_generateCoroutine != null)
        {
            StopCoroutine(_generateCoroutine);
            _generateCoroutine = null;
        }

        _debrisCtrl.Return();
        _debrisShadowCtrl.Return();
    }

    /// <summary>
    /// 生成する位置を初期化
    /// </summary>
    private void ResetGenerateIndex()
    {
        for (int i = 0; i < _generateTransIndex.Length; i++)
        {
            _generateTransIndex[i] = -1;
        }
    }

    IEnumerator GenerateCoroutine()
    {
        yield return new WaitForSeconds(0.5f);

        var interval = new WaitForSeconds(_debugGenerateInterval);

        while (true)
        {
            Generate(_generateCount);
            yield return interval;
        }
    }
    IEnumerator GenerateIntervalCoroutine(int generateCount)
    {
        var interval = new WaitForSeconds(_sometimeGenerateInterval);

        for (int i = 0; i < generateCount; i++)
        {
            GameObject shadow = default;
            Vector3 shadowGeneratePoint = new Vector3(_generateTrans[_generateTransIndex[i]].position.x,
                                              0.01f, //影を表示するY座標はこの数値でないとステージに埋まるため 
                                              _generateTrans[_generateTransIndex[i]].position.z);
            
            var point = _generateTrans[_generateTransIndex[i]].position;

            _debrisShadowCtrl.Use(shadowGeneratePoint, s =>
            {
                s.OnAnimation(2.5f, () => 
                {
                    _debrisCtrl.Use(point, d =>
                    {
                        d.OnAnimation(() => 
                        {
                            s.gameObject.SetActive(false);
                        });

                        if (shadow != null)
                        {
                            d.SetShadow(shadow);
                        }
                    });
                });
                shadow = s.gameObject;
            });

            yield return interval;
        }
    }
}
