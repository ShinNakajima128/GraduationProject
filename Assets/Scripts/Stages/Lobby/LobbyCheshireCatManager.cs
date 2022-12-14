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
                     .CatObject.TryGetComponent(out _movableCat);
    }

    private void Start()
    {
        //動き回るチェシャ猫のアクティブ時のアクションを登録
        _activateActionDic[LobbyCheshireCatType.Movable] += () => _movableCat.ChangeState(CheshireCatState.Idle);
    }
    /// <summary>
    /// 指定したチェシャ猫をアクティブにする
    /// </summary>
    /// <param name="type"> チェシャ猫オブジェクトの種類 </param>
    public void ActiveCheshireCat(LobbyCheshireCatType type)
    {
        foreach (var cat in _cheshireCats)
        {
            cat.CatObject.SetActive(false);
        }

        _cheshireCats.FirstOrDefault(c => c.CatType == type).CatObject.SetActive(true);
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
    public GameObject CatObject;
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
