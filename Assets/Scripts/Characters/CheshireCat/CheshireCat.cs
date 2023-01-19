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
    float _fadeTime = 1.0f;

    [SerializeField]
    CheshireCatState _currentState = default;

    [Header("Renderer")]
    [SerializeField]
    Renderer _cheshireRenderer = default;
    #endregion

    #region private
    Animator _anim;
    Coroutine _currentCoroutine;
    Material _cheshireBodyMat;
    Material _cheshireFaceMat;
    #endregion

    #region public
    #endregion

    #region property
    #endregion

    private void Awake()
    {
        TryGetComponent(out _anim);
    }

    IEnumerator Start()
    {
        EventManager.ListenEvents(Events.Cheshire_StartGrinning, () => 
        {
            OnAction(CheshireCatState.Jump, 0.2f); 
        });
        _cheshireBodyMat = _cheshireRenderer.materials[0];
        _cheshireFaceMat = _cheshireRenderer.materials[1];

        _cheshireRenderer.materials[0] = _cheshireBodyMat;
        _cheshireRenderer.materials[1] = _cheshireFaceMat;

        yield return null;

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

    public void ActivateDissolve(bool isActive)
    {
        Debug.Log("Dissolve");
        float bodyValue;
        float faceValue;

        if (isActive)
        {
            bodyValue = -1f;

            DOTween.To(() => bodyValue,
                v => bodyValue = v,
                0f,
                _fadeTime)
                .OnUpdate(() => 
                { 
                    _cheshireBodyMat.SetVector("_DissolveParams", new Vector4(3, 1, bodyValue, 0.1f));
                });

            faceValue = -1;

            DOTween.To(() => faceValue,
                v => faceValue = v,
                0f,
                _fadeTime)
                .OnUpdate(() =>
                {
                    _cheshireFaceMat.SetVector("_DissolveParams", new Vector4(3, 1, faceValue, 0.1f));
                });
        }
        else
        {
            bodyValue = 0f;

            DOTween.To(() => bodyValue,
                v => bodyValue = v,
                -1f,
                _fadeTime)
                .OnUpdate(() =>
                {
                    _cheshireBodyMat.SetVector("_DissolveParams", new Vector4(3, 1, bodyValue, 0.1f));
                });

            faceValue = 0f;

            DOTween.To(() => faceValue,
                v => faceValue = v,
                -1f,
                _fadeTime)
                .OnUpdate(() =>
                {
                    _cheshireFaceMat.SetVector("_DissolveParams", new Vector4(3, 1, faceValue, 0.1f));
                });
        }
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
                _anim.CrossFadeInFixedTime(CheshireCatState.Idle.ToString(), 0.25f);
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
                _anim.CrossFadeInFixedTime(CheshireCatState.Jump.ToString(), animBlend);
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
