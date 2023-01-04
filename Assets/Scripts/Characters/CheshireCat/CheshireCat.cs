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
    CheshireCatState _currentState = default;
    #endregion

    #region private
    Animator _anim;
    Coroutine _currentCoroutine;
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
        ChangeState(_currentState);
    }

    /// <summary>
    /// チェシャ猫のステータスを変更する
    /// </summary>
    /// <param name="state"> ステータスの種類 </param>
    public void ChangeState(CheshireCatState state, float animBlend = 0.2f)
    {
        _currentState = state;

        OnAction(_currentState, animBlend);
    }

    /// <summary>
    /// 指定したステータスのアクションを実行
    /// </summary>
    /// <param name="state"> ステータスの種類 </param>
    void OnAction(CheshireCatState state, float animBlend)
    {
        //以前のステータスのコルーチンを中断
        if (_currentCoroutine != null)
        {
            StopCoroutine(_currentCoroutine);
            _currentCoroutine = null;
        }

        switch (state)
        {
            case CheshireCatState.Idle:
                _currentCoroutine = StartCoroutine(IdleCoroutine());
                break;
            case CheshireCatState.Idle_Lick:
                break;
            case CheshireCatState.LyingDown:
                _anim.CrossFadeInFixedTime(CheshireCatState.LyingDown.ToString(), animBlend);
                break;
            case CheshireCatState.Walk:
                _anim.CrossFadeInFixedTime(CheshireCatState.Walk.ToString(), animBlend);
                break;
            case CheshireCatState.FastWalk:
                _anim.CrossFadeInFixedTime(CheshireCatState.FastWalk.ToString(), animBlend);
                break;
            case CheshireCatState.Jump:
                break;
            case CheshireCatState.Appearance:
                break;
            case CheshireCatState.Idle_Standing:
                _anim.CrossFadeInFixedTime(CheshireCatState.Idle_Standing.ToString(), animBlend);
                break;
            default:
                break;
        }
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
            random = Random.Range(7, 10);

            yield return new WaitForSeconds(random);

            _anim.CrossFadeInFixedTime(CheshireCatState.Idle_Lick.ToString(), 0.1f);

            yield return new WaitForSeconds(2.3f);

            _anim.CrossFadeInFixedTime(CheshireCatState.Idle.ToString(), 0.25f);
        }
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
    /// <summary> カメラワーク付きの登場演出 </summary>
    Appearance,
    /// <summary> アイドル(立つ) </summary>
    Idle_Standing
}
