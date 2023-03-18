using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// ステージ1の背景の動きを管理するクラス
/// </summary>
public class BackgroundController : MonoBehaviour
{
    #region serialize
    [Tooltip("地面のモデルのアニメーション時間")]
    [SerializeField]
    float _groundModelSpeed = 3.0f;

    [SerializeField]
    float _groundHeight = 2.5f;

    [Tooltip("地面のモデル")]
    [SerializeField]
    Transform _groundModel = default;
    
    [Tooltip("ゲーム終了時の着地地点のObject")]
    [SerializeField]
    Transform _finishTrans = default;

    [SerializeField]
    Renderer _background = default;

    #endregion

    #region private
    Vector3 _originPos;
    Material _mat;
    int _propertyId = 0;
    #endregion

    private void Awake()
    {
        _originPos = _groundModel.position;
    }
    private void Start()
    {
        _mat = _background.material;
        _propertyId = Shader.PropertyToID("Vector2_f819a373818e42bd843147b85c47a54d");
        //FallGameManager.Instance.GameStart += Init;
        FallGameManager.Instance.GameEnd += FinishAnimation;
        Init();
    }

    void Init()
    {
        _groundModel.position = _originPos;
        _groundModel.gameObject.SetActive(false);
        SetScrollSpeed(new Vector2(0f, -0.03f), 0f);
    }

    void FinishAnimation()
    {
        StartCoroutine(FinishCoroutine());
    }

    IEnumerator FinishCoroutine()
    {
        yield return new WaitForSeconds(1.5f);

        SetScrollSpeed(new Vector2(0f, 0f), 0f);
        _groundModel.gameObject.SetActive(true);
        _groundModel.DOMoveY(_finishTrans.position.y + _groundHeight, 0);
    }
    void SetScrollSpeed(Vector2 vector, float time, Action action = null)
    {
        _mat.DOVector(vector, _propertyId, time)
            .OnComplete(() => 
            {
                action?.Invoke();
            });
    }
}
