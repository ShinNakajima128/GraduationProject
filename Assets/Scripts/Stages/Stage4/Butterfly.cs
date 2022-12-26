using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;

/// <summary>
/// ステージ4の蝶の機能の持つComponent
/// </summary>
public class Butterfly : MonoBehaviour
{
    #region serialize
    [SerializeField]
    float _moveSpeed = 3.0f;

    [SerializeField]
    float _flyAnimTime = 2.5f;

    [Header("Components")]
    [SerializeField]
    Renderer _butterfltRenderer = default;

    [Tooltip("飛び去る時のPath")]
    [SerializeField]
    Transform[] _flyPath = default;

    [Header("Materials")]
    [SerializeField]
    Material _redMat = default;

    [SerializeField]
    Material _whiteMat = default;
    #endregion

    #region private
    Animator _anim;
    BoxCollider _playerSearchCollider;
    Vector3 _originEulerAngles;
    bool _init = false;
    #endregion

    #region public
    #endregion

    #region property
    #endregion

    private void Awake()
    {
        TryGetComponent(out _anim);
        TryGetComponent(out _playerSearchCollider);
        _originEulerAngles = transform.localEulerAngles;
        _init = true;
    }

    private void OnDisable()
    {
        _playerSearchCollider.enabled = false;

        if (_init)
        {
            transform.localEulerAngles = _originEulerAngles;
        }
    }

    /// <summary>
    /// 蝶のマテリアル(色)を変更する
    /// </summary>
    /// <param name="color"> 色 </param>
    public void ChangeMaterial(ButterflyColor color)
    {
        switch (color)
        {
            case ButterflyColor.Red:
                _butterfltRenderer.material = _redMat;
                break;
            case ButterflyColor.White:
                _butterfltRenderer.material = _whiteMat;
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 指定したステータスに変更する
    /// </summary>
    /// <param name="state"> 蝶のステータス </param>
    public void ChangeState(ButterflyState state)
    {
        switch (state)
        {
            case ButterflyState.Idle:
                _playerSearchCollider.enabled = true;
                break;
            case ButterflyState.Flapping:
                break;
            case ButterflyState.Fly_Fast:
                _playerSearchCollider.enabled = false;
                StartCoroutine(FlyingCoroutine());
                break;
            case ButterflyState.Fly_Slow:
                break;
            case ButterflyState.Fly_VerySlow:
                break;
            default:
                break;
        }
        _anim.CrossFadeInFixedTime(state.ToString(), 0.1f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("アリスにヒット");

            int random = UnityEngine.Random.Range(0, 2);

            if (random == 0)
            {
                ChangeState(ButterflyState.Flapping);
            }
            else
            {
                ChangeState(ButterflyState.Fly_Fast);
            }
        }
    }

    /// <summary>
    /// 飛ぶ去るモーションのコルーチン
    /// </summary>
    IEnumerator FlyingCoroutine()
    {
        var path = _flyPath.Select(x => x.transform.position).ToArray();

        transform.localRotation = new Quaternion(transform.localEulerAngles.x, transform.localEulerAngles.y, -90, 0);

        yield return transform.DOPath(path, _flyAnimTime, PathType.CatmullRom)
                              .WaitForCompletion();

        gameObject.SetActive(false);
    }
}

public enum ButterflyState
{
    /// <summary> 蝶の状態 </summary>
    Idle,
    /// <summary> 止まりながら羽を動かす </summary>
    Flapping,
    /// <summary> 羽ばたき(速め) </summary>
    Fly_Fast,
    /// <summary> 羽ばたき(遅め) </summary>
    Fly_Slow,
    /// <summary> 羽ばたき(かなり遅め) </summary>
    Fly_VerySlow
}

/// <summary>
/// 蝶の色
/// </summary>
public enum ButterflyColor
{
    Red,
    White
}

