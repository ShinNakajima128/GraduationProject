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
        FallGameManager.Instance.GameStart += Init;
        FallGameManager.Instance.GameEnd += FinishAnimation;
    }

    void Init()
    {
        _groundModel.position = _originPos;
        _groundModel.gameObject.SetActive(false);
        SetScrollSpeed(new Vector2(0f, -0.2f), 0f);
    }

    void FinishAnimation()
    {
        _groundModel.gameObject.SetActive(true);
        _groundModel.DOMoveY(_finishTrans.position.y - 2.5f, _groundModelSpeed);
        SetScrollSpeed(new Vector2(0f, 0f), _groundModelSpeed);
    }

    void SetScrollSpeed(Vector2 vector, float time)
    {
        _mat.DOVector(vector, _propertyId, time);
        //_background.material = _mat;
    }
}
