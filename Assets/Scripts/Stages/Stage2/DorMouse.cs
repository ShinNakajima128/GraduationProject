using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ステージ2のカップに隠れるネズミのコンポーネント
/// </summary>
public class DorMouse : MonoBehaviour
{
    #region serialize
    [Header("variable")]
    [Tooltip("目の瞬きの間隔")]
    [SerializeField]
    float _interval = 1.5f;

    [Header("Material")]
    [Tooltip("ネズミの表情を変更する用のMaterial")]
    [SerializeField]
    Material[] _mouseMaterials = default;

    [Header("Debug")]
    [SerializeField]
    bool _debugMode = false;
    #endregion
    #region private
    Animator _anim;
    Renderer _mouseRenderer;
    bool _isAwaking = false;
    #endregion
    #region property
    #endregion

    private void Awake()
    {
        TryGetComponent(out _anim);
        TryGetComponent(out _mouseRenderer);
    }

    IEnumerator Start()
    {
        if (_debugMode)
        {
            while (true)
            {
                yield return StartCoroutine(TestAnimation());
            }
        }
    }

    /// <summary>
    /// マウスのアニメーションを再生
    /// </summary>
    /// <param name="state"> 再生するアニメーション </param>
    public void OnAnimation(MouseState state,float duration)
    {
        switch (state)
        {
            case MouseState.WakeUp:
                _isAwaking = true;
                StartCoroutine(WakeUpMouseCoroutine());
                break;
            default:
                _isAwaking = false;
                break;
        }
        _anim.CrossFadeInFixedTime(state.ToString(), duration);
    }

    /// <summary>
    /// 発見時のSEを再生
    /// </summary>
    void PlayFindSE()
    {
        try
        {
            AudioManager.PlaySE(SEType.Finding);
        }
        catch
        {
            Debug.LogError("AudioManagerがHierarchyにないため、再生できません");
        }
    }

    IEnumerator WakeUpMouseCoroutine()
    {
        yield return new WaitForSeconds(1f);

        _mouseRenderer.material = _mouseMaterials[1];

        yield return new WaitForSeconds(0.05f);

        _mouseRenderer.material = _mouseMaterials[2];

        //瞬きの処理のループ
        while (_isAwaking)
        {
            //ここで指定した瞬きの秒数の間、処理を待機
            yield return new WaitForSeconds(_interval);

            yield return new WaitForSeconds(0.05f);

            _mouseRenderer.material = _mouseMaterials[1];

            yield return new WaitForSeconds(0.05f);

            _mouseRenderer.material = _mouseMaterials[0];

            yield return new WaitForSeconds(0.05f);

            _mouseRenderer.material = _mouseMaterials[1];

            yield return new WaitForSeconds(0.05f);

            _mouseRenderer.material = _mouseMaterials[2];
        }

        //目を閉じているテクスチャに変更
        _mouseRenderer.material = _mouseMaterials[0];
        Debug.Log("瞬きアニメーション終了");
    }

    IEnumerator TestAnimation()
    {
        yield return new WaitForSeconds(1.0f);

        // OnAnimation(MouseState.WakeUp);

        yield return new WaitForSeconds(8.0f);

        // OnAnimation(MouseState.CloseEar);
    }
}
public enum MouseState
{
    OpenEar,
    CloseEar,
    WakeUp
}

