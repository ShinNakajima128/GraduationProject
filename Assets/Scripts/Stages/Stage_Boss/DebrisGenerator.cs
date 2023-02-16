using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// がれきを生成、消去する機能を持つComponent
/// </summary>
public class DebrisGenerator : MonoBehaviour
{
    #region serialize
    [Header("Variables")]
    [Tooltip("がれきを生成する数")]
    [SerializeField]
    int _generateCount = 3;

    [Tooltip("フェーズ2の生成範囲の座標")]
    [SerializeField]
    Transform[] _generateRangeTrans = default;

    [Tooltip("同じタイミングで生成したがれきの生成間隔")]
    [SerializeField]
    float _sometimeGenerateInterval = 0.2f;

    [Tooltip("回復アイテムが発生する確率")]
    [SerializeField, Range(0f, 1f)]
    float _itemGeneratePercent = 0.3f;

    [SerializeField]
    float _nextGenerateInterval = 5.0f;

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

    [Tooltip("回復アイテムのGenerator")]
    [SerializeField]
    ItemGenerator _itemGenerator = default;

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
    public bool IsGenerating { get; private set; } = false;
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
    /// がれきを設定された「範囲」にランダムで生成する
    /// </summary>
    /// <param name="generateCount"> 生成する数 </param>
    public void RandomGenerate(int generateCount)
    {
        StartCoroutine(RandomIntervalCoroutine(generateCount));
    }

    /// <summary>
    /// がれきを設定された「座標」にランダムで生成する
    /// </summary>
    /// <param name="generateCount"> 生成する数 </param>
    public void DefiniteGenerate(int generateCount)
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
        if (_generateCoroutine != null)
        {
            StopCoroutine(_generateCoroutine);
            _generateCoroutine = null;
        }
        _debrisCtrl.Return();
    }

    /// <summary>
    /// がれきを生成開始する
    /// </summary>
    public void StartGenerate(GenerateType type)
    {
        switch (type)
        {
            case GenerateType.Random:
                _generateCoroutine = StartCoroutine(RandomGenerateCoroutine());
                break;
            case GenerateType.Definite:
                _generateCoroutine = StartCoroutine(GenerateCoroutine());
                break;
            default:
                break;
        }
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

        IsGenerating = false;
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

    IEnumerator RandomGenerateCoroutine()
    {
        yield return new WaitForSeconds(0.5f);

        WaitForSeconds interval;

        if (_debugMode)
        {
            interval = new WaitForSeconds(_debugGenerateInterval);
        }
        else
        {
            interval = new WaitForSeconds(_nextGenerateInterval);
        }

        while (true)
        {
            RandomGenerate(_generateCount);
            yield return interval;
        }
    }

    IEnumerator GenerateCoroutine()
    {
        yield return new WaitForSeconds(0.5f);

        WaitForSeconds interval;

        if (_debugMode)
        {
            interval = new WaitForSeconds(_debugGenerateInterval);
        }
        else
        {
            interval = new WaitForSeconds(_nextGenerateInterval); 
        }

        while (true)
        {
            DefiniteGenerate(_generateCount);
            yield return interval;
        }
    }

    IEnumerator RandomIntervalCoroutine(int generateCount)
    {
        var interval = new WaitForSeconds(_sometimeGenerateInterval);

        IsGenerating = true;

        for (int i = 0; i < generateCount; i++)
        {
            GameObject shadow = default;

            float randomX = Random.Range(_generateRangeTrans[0].position.x, _generateRangeTrans[1].position.x);
            float randomZ = Random.Range(_generateRangeTrans[1].position.z, _generateRangeTrans[0].position.z);

            Vector3 randomPoint = new Vector3(randomX, 10f, randomZ);

            Vector3 shadowGeneratePoint = new Vector3(randomPoint.x,
                                              0.01f, //影を表示するY座標はこの数値でないとステージに埋まるため 
                                              randomPoint.z);

            _debrisShadowCtrl.Use(shadowGeneratePoint, s =>
            {
                s.OnAnimation(2.5f, () =>
                {
                    _debrisCtrl.Use(randomPoint, d =>
                    {
                        d.OnAnimation(() =>
                        {
                            s.gameObject.SetActive(false);
                        });

                        if (shadow != null)
                        {
                            d.SetShadow(shadow);
                        }

                        int percent = Random.Range(0, 10);

                        //指定したアイテム生成確率を超えたらアイテムを発生させる
                        if (_itemGeneratePercent >= percent / 10.0f)
                        {
                            d.IsItemGenerate = true;
                        }
                    });
                });
                shadow = s.gameObject;
            });

            yield return interval;
        }
    }

    IEnumerator GenerateIntervalCoroutine(int generateCount)
    {
        var interval = new WaitForSeconds(_sometimeGenerateInterval);

        IsGenerating = true;

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

                        int percent = Random.Range(0, 10);

                        //指定したアイテム生成確率を超えたらアイテムを発生させる
                        if (_itemGeneratePercent >= percent / 10.0f)
                        {
                            d.IsItemGenerate = true;
                        }
                    });
                });
                shadow = s.gameObject;
            });

            yield return interval;
        }
    }
}

/// <summary>
/// 生成の種類
/// </summary>
public enum GenerateType
{
    Random, //指定した範囲をランダム
    Definite //定位置
}
