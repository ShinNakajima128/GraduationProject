using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// チェシャ猫の表情をコントロールするコンポーネント
/// </summary>
public class CheshireCatFaceController : MonoBehaviour
{
    #region serialize
    [Header("Variables")]
    [SerializeField]
    float _blinkSwitchTime = 0.05f;

    [SerializeField]
    float _broadlySwitchTime = 0.05f;

    [SerializeField]
    float _talkSwitchTime = 0.05f;

    [Tooltip("次のまばたきを開始するまでの待機時間の最大値")]
    [SerializeField]
    float _blinkMaxInterval = 5.0f;

    [Tooltip("現在の表情の種類")]
    [SerializeField]
    CheshireCatFaceType _currentFaceType = default;

    [Header("Components")]
    [Tooltip("チェシャ猫のRenderer")]
    [SerializeField]
    Renderer _catRenderer = default;
    #endregion

    #region private
    Dictionary<CatFacePatternType, Vector2> _catFacePatternDic = new Dictionary<CatFacePatternType, Vector2>();
    Coroutine _currentFaceCoroutine;
    Material _faceMat;
    bool _init;
    bool _isBlinking = false;
    bool _isGrinning = false;
    bool _isTalking = false;
    #endregion

    #region public
    #endregion

    #region property
    #endregion

    private void OnValidate()
    {
        if (_init)
        {
            ChangeFaceType(_currentFaceType);
        }
    }
    private void Awake()
    {
        int faceCount = 0;

        for (int i = 0; i < 4; i++)
        {
            for (int n = 0; n < 2; n++)
            {
                _catFacePatternDic.Add((CatFacePatternType)faceCount, new Vector2(n * 0.5f, i * -0.25f));
                faceCount++;
            }
        }
        _faceMat = _catRenderer.materials[1];
        _init = true;
    }

    private void Start()
    {
        EventManager.ListenEvents(Events.Cheshire_Talk, OnTaking);
        EventManager.ListenEvents(Events.Cheshire_StartGrinning, OnGrinning);
        EventManager.ListenEvents(Events.FinishTalking, OnReturnDefaultFace);
        EventManager.ListenEvents(Events.BossStage_FrontCheshire, OnTaking);
    }

    /// <summary>
    /// 顔の表情を変更する
    /// </summary>
    /// <param name="type"> 表情の種類 </param>
    public void ChangeFaceType(CheshireCatFaceType type)
    {
        if (_currentFaceCoroutine != null)
        {
            StopCoroutine(_currentFaceCoroutine);
            _currentFaceCoroutine = null;
            _isBlinking = false;
            _isTalking = false;
        }

        switch (type)
        {
            case CheshireCatFaceType.Default:
                ChangeFacePatternType(CatFacePatternType.Default);
                break;
            case CheshireCatFaceType.Blink:
                _isBlinking = true;
                _currentFaceCoroutine = StartCoroutine(BlinkCoroutine());
                break;
            case CheshireCatFaceType.StartGrinning:
                _isGrinning = true;
                _currentFaceCoroutine = StartCoroutine(StartGrinningCoroutine());
                break;
            case CheshireCatFaceType.EndGrinning:
                _isGrinning = false;
                _currentFaceCoroutine = StartCoroutine(EndGrinningCoroutine());
                break;
            case CheshireCatFaceType.Talking:
                _isTalking = true;
                _currentFaceCoroutine = StartCoroutine(TalkCoroutine());
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 顔のパターンを変更する
    /// </summary>
    /// <param name="type"> パターンの種類 </param>
    void ChangeFacePatternType(CatFacePatternType type)
    {
        _faceMat.SetTextureOffset("_MainTex", GetFaceOffset(type));
        _catRenderer.materials[1] = _faceMat;
    }

    /// <summary>
    /// 表情の位置を取得する
    /// </summary>
    /// <param name="type"> 表情の種類 </param>
    /// <returns></returns>
    Vector2 GetFaceOffset(CatFacePatternType type)
    {
        var v = _catFacePatternDic.FirstOrDefault(x => x.Key == type).Value;

        return v;
    }

    /// <summary>
    /// まばたきのコルーチン
    /// </summary>
    IEnumerator BlinkCoroutine()
    {
        float timer;
        float rand;

        ChangeFacePatternType(CatFacePatternType.Default);

        //瞬きの処理のループ
        while (_isBlinking)
        {
            yield return new WaitForSeconds(_blinkSwitchTime);

            ChangeFacePatternType(CatFacePatternType.HalfOpenEye);

            yield return new WaitForSeconds(_blinkSwitchTime);

            ChangeFacePatternType(CatFacePatternType.CloseEye);

            yield return new WaitForSeconds(_blinkSwitchTime);

            ChangeFacePatternType(CatFacePatternType.HalfOpenEye);

            yield return new WaitForSeconds(_blinkSwitchTime);

            ChangeFacePatternType(CatFacePatternType.Default);

            //ここで指定した瞬きの秒数の間、処理を待機
            timer = 0;
            rand = Random.Range(3, _blinkMaxInterval);
            while (_isBlinking && timer < rand)
            {
                timer += Time.deltaTime;
                yield return null;
            }
        }
    }

    /// <summary>
    /// ニヤリ顔になるコルーチン
    /// </summary>
    IEnumerator StartGrinningCoroutine()
    {
        _isGrinning = true;

        ChangeFacePatternType(CatFacePatternType.Default);

        yield return new WaitForSeconds(_broadlySwitchTime);

        ChangeFacePatternType(CatFacePatternType.Broadly_First);

        yield return new WaitForSeconds(_broadlySwitchTime);

        ChangeFacePatternType(CatFacePatternType.Broadly_Second);

        yield return new WaitForSeconds(_broadlySwitchTime);

        ChangeFacePatternType(CatFacePatternType.Broadly_Third);
    }

    /// <summary>
    /// ニヤリ顔から通常顔に戻るコルーチン
    /// </summary>
    IEnumerator EndGrinningCoroutine()
    {
        ChangeFacePatternType(CatFacePatternType.Broadly_Third);

        yield return new WaitForSeconds(_broadlySwitchTime);

        ChangeFacePatternType(CatFacePatternType.Broadly_Second);

        yield return new WaitForSeconds(_broadlySwitchTime);

        ChangeFacePatternType(CatFacePatternType.Broadly_First);

        yield return new WaitForSeconds(_broadlySwitchTime);

        ChangeFacePatternType(CatFacePatternType.Default);
    }

    /// <summary>
    /// 喋るコルーチン
    /// </summary>
    IEnumerator TalkCoroutine()
    {
        int random;

        while (_isTalking)
        {
            random = Random.Range(0, 3);

            if (random == 0)
            {
                ChangeFacePatternType(CatFacePatternType.Default);
            }
            else if (random == 1)
            {
                ChangeFacePatternType(CatFacePatternType.Talk1);
            }
            else
            {
                ChangeFacePatternType(CatFacePatternType.Talk2);
            }

            yield return new WaitForSeconds(_talkSwitchTime);
        }
    }

    #region MessageActions
    /// <summary>
    /// 喋る
    /// </summary>
    void OnTaking()
    {
        ChangeFaceType(CheshireCatFaceType.Talking);
    }
    /// <summary>
    /// ニヤける
    /// </summary>
    void OnGrinning()
    {
        ChangeFaceType(CheshireCatFaceType.StartGrinning);
    }
    /// <summary>
    /// 通常顔に戻る
    /// </summary>
    void OnReturnDefaultFace()
    {
        //話していない時は処理を行わない
        if (!_isTalking)
        {
            return;
        }
        ChangeFaceType(CheshireCatFaceType.Blink);
    }
    #endregion
}

/// <summary>
/// チェシャ猫の表情の種類
/// </summary>
public enum CheshireCatFaceType
{
    /// <summary> 通常の表情 </summary>
    Default,
    /// <summary> まばたき </summary>
    Blink,
    /// <summary> 通常の顔からニヤリ顔になる </summary>
    StartGrinning,
    /// <summary> ニヤリ顔から通常顔になる </summary>
    EndGrinning,
    /// <summary> 喋る </summary>
    Talking
}

/// <summary>
/// チェシャ猫の表情パターン
/// </summary>
public enum CatFacePatternType
{
    Default,
    HalfOpenEye,
    CloseEye,
    Broadly_First,
    Broadly_Second,
    Broadly_Third,
    Talk1,
    Talk2
}
