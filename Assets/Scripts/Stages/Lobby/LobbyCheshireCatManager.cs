using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ロビーのチェシャ猫を管理するManagerクラス
/// </summary>
public class LobbyCheshireCatManager : MonoBehaviour
{
    #region serialize
    [Header("Objects")]
    [Tooltip("ロビーのチェシャ猫データ")]
    [SerializeField]
    LobbyCheshireCat[] _cheshireCats = default;

    [Header("Components")]
    [Tooltip("ロビーを動き回るチェシャ猫のコンポーネント")]
    [SerializeField]
    CheshireCat _cheshireCat = default;

    [Tooltip("ロビーを動き回るチェシャ猫の行動処理のコンポーネント")]
    [SerializeField]
    CheshireCatBehaviour _catBehaviour = default;

    [Header("Debug")]
    [SerializeField]
    bool _debugMode = false;
    #endregion

    #region private
    /// <summary> 動き廻るチェシャ猫のComponent </summary>
    CheshireCat _movableCat = default;
    /// <summary> ロビーのチェシャ猫がアクティブになった時のアクション </summary>
    Dictionary<LobbyCheshireCatType, Action> _activateActionDic = new Dictionary<LobbyCheshireCatType, Action>();
    #endregion

    #region public
    #endregion

    #region property
    public static LobbyCheshireCatManager Instance { get; private set; }
    public CheshireCat MovableCat => _movableCat;
    #endregion

    private void Awake()
    {
        Instance = this;

        for (int i = 0; i < _cheshireCats.Length; i++)
        {
            _activateActionDic.Add((LobbyCheshireCatType)i, null);
        }

        //ロビーを動き回るチェシャ猫の機能を持つComponentを取得
        _cheshireCats.FirstOrDefault(c => c.CatType == LobbyCheshireCatType.Movable)
                     .CheshireCat.TryGetComponent(out _movableCat);
    }

    IEnumerator Start()
    {
        //動き回るチェシャ猫のアクティブ時のアクションを登録
        _activateActionDic[LobbyCheshireCatType.Movable] += () => _movableCat.ChangeState(CheshireCatState.Idle);

        if (_debugMode)
        {
            ActiveCheshireCat(LobbyCheshireCatType.Movable);
        }

        yield return null;

        foreach (var cat in _cheshireCats)
        {
            cat.CheshireCat.SetActive(false);
        }
    }

    /// <summary>
    /// 指定したチェシャ猫をアクティブにする
    /// </summary>
    /// <param name="type"> チェシャ猫オブジェクトの種類 </param>
    public void ActiveCheshireCat(LobbyCheshireCatType type, bool isMoving = false)
    {
        foreach (var cat in _cheshireCats)
        {
            cat.CheshireCat.SetActive(false);
        }

        var activeCat = _cheshireCats.FirstOrDefault(c => c.CatType == type).CheshireCat;
        
        activeCat.SetActive(true);

        if (type == LobbyCheshireCatType.Movable) 
        {
            if (isMoving)
            {
                _catBehaviour.StartMoving();
            }
        }

        print($"{type}");
    }
}

/// <summary>
/// ロビーのチェシャ猫データ
/// </summary>
[Serializable]
public struct LobbyCheshireCat
{
    public string TypeName;
    public LobbyCheshireCatType CatType;
    public GameObject CheshireCat;
}

/// <summary>
/// ロビーのチェシャ猫の種類
/// </summary>
public enum LobbyCheshireCatType
{
    /// <summary> 演出 </summary>
    Appearance,
    /// <summary> ロビーを歩き回る </summary>
    Movable
}
