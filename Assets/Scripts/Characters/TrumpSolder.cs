using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrumpSolder : MonoBehaviour
{
    #region serialize
    [Header("Variables")]
    [SerializeField]
    TrumpPatternType _currentPatternType = default;

    [Tooltip("ランダムで柄を変更するかどうか")]
    [SerializeField]
    bool _isRandomPattern = false;

    [Header("Renderer")]
    [SerializeField]
    Renderer _patternRenderer = default;
    #endregion

    #region private
    Dictionary<TrumpPatternType, Vector2> _patternTypeDic = new Dictionary<TrumpPatternType, Vector2>();
    Material _patternMat;
    TrumpColorType _currentColorType = default;
    bool _init = false;
    #endregion
    #region public
    #endregion
    #region property
    public TrumpColorType CurrentColorType => _currentColorType;
    #endregion

    private void OnValidate()
    {
        if (_init)
        {
            ChangePatternType(_currentPatternType);
        }
    }

    private void Awake()
    {
        int patternCount = 0;

        for (int i = 0; i < 4; i++)
        {
            for (int n = 0; n < 2; n++)
            {
                _patternTypeDic.Add((TrumpPatternType)patternCount, new Vector2(n * 0.5f, i * -0.25f));
                patternCount++;
            }
        }
        _patternMat = _patternRenderer.materials[1];
        _init = true;
    }

    private void Start()
    {
        if (_isRandomPattern)
        {
            ChangeRandomPattern();
        }
        else
        {
            ChangePatternType(_currentPatternType);
        }
    }

    /// <summary>
    /// ランダムな柄に変更する
    /// </summary>
    public void ChangeRandomPattern()
    {
        var randomPattern = (TrumpPatternType)Random.Range(0, 8);

        _patternMat.SetTextureOffset("_MainTex", GetPatternOffset(randomPattern));
        _patternRenderer.materials[1] = _patternMat;
        _currentPatternType = randomPattern;

        if ((int)randomPattern < 4)
        {
            _currentColorType = TrumpColorType.Red;
        }
        else
        {
            _currentColorType = TrumpColorType.Black;
        }
    }

    /// <summary>
    /// 指定された色でランダムな数字のパターンに変更する
    /// </summary>
    /// <param name="type"> 色の種類 </param>
    public void ChangeRandomPattern(TrumpColorType type)
    {
        TrumpPatternType randomPattern = default;

        switch (type)
        {
            case TrumpColorType.Red:
                randomPattern = (TrumpPatternType)Random.Range(0, 4);
                _currentColorType = TrumpColorType.Red;
                break;
            case TrumpColorType.Black:
                randomPattern = (TrumpPatternType)Random.Range(4, 8);
                _currentColorType = TrumpColorType.Black;
                break;
            default:
                break;
        }
        _patternMat.SetTextureOffset("_MainTex", GetPatternOffset(randomPattern));
        _patternRenderer.materials[1] = _patternMat;
    }

    public void ChangePatternType(TrumpPatternType patternType)
    {
        _patternMat.SetTextureOffset("_MainTex", GetPatternOffset(patternType));
        _patternRenderer.materials[1] = _patternMat;

        if ((int)patternType < 4)
        {
            _currentColorType = TrumpColorType.Red;
        }
        else
        {
            _currentColorType = TrumpColorType.Black;
        }
    }

    Vector2 GetPatternOffset(TrumpPatternType type)
    {
        var p = _patternTypeDic.FirstOrDefault(x => x.Key == type).Value;

        return p;
    }
}

/// <summary>
/// トランプ兵の柄の種類
/// </summary>
public enum TrumpPatternType
{
    Red_One,
    Red_Seven,
    Red_Eight,
    Red_Six,
    Black_Five,
    Black_Four,
    Black_Nine,
    Black_Three
}
/// <summary>
/// トランプ兵の色の種類
/// </summary>
public enum TrumpColorType
{
    Red,
    Black
}
