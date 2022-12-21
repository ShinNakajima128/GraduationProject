using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// チェシャ猫の機能を持つコンポーネント
/// </summary>
public class CheshireCat : MonoBehaviour
{
    #region serialize
    [Header("Variables")]
    [SerializeField]
    float _moveSpeed = 1.5f;

    [SerializeField]
    CheshireCatState _state = default;
    #endregion

    #region private
    Animator _anim;
    #endregion

    #region public
    #endregion

    #region property
    #endregion

    private void Awake()
    {
        TryGetComponent(out _anim);
    }

    private void Start()
    {
        StartCoroutine(IdleCoroutine());
    }

    /// <summary>
    /// アイドルモーションのコルーチン。テスト用
    /// </summary>
    /// <returns></returns>
    IEnumerator IdleCoroutine()
    {
        int random;

        while (true)
        {
            random = Random.Range(5, 8);

            yield return new WaitForSeconds(random);

            _anim.CrossFadeInFixedTime(CheshireCatState.Idle_Lick.ToString(), 0.1f);

            yield return new WaitForSeconds(2.3f);

            _anim.CrossFadeInFixedTime(CheshireCatState.Idle.ToString(), 0.25f);
        }
    }

    void ChangeState(CheshireCatState state)
    {
        _state = state;
    }
}

/// <summary>
/// チェシャ猫のステータス
/// </summary>
public enum CheshireCatState
{
    /// <summary> アイドル(座る) </summary>
    Idle,
    /// <summary> 座りながら手を舐める </summary>
    Idle_Lick,
    /// <summary> アイドル(伏せる) </summary>
    LyingDown,
    /// <summary> 歩く </summary>
    Walk,
    /// <summary> 早歩き </summary>
    FastWalk,
    /// <summary> ジャンプ </summary>
    Jump,
    /// <summary> カメラワーク付きの登場演出(現状使用不可) </summary>
    Appearance
}
