using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// ステージ1のプレイヤーの移動機能を管理する
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class PlayerMove : MonoBehaviour, IMovable
{
    #region serialize
    [SerializeField]
    float _moveSpeed = 5.0f;

    [SerializeField]
    Transform _finishTrans = default;
    #endregion

    #region private
    CharacterController _cc;
    Animator _anim;
    Vector3 _dir;
    Vector3 _moveDir;
    bool _isMovable = false;
    #endregion

    private void Awake()
    {
        TryGetComponent(out _cc);
        TryGetComponent(out _anim);
    }

    private void Start()
    {
        FallGameManager.Instance.GameStart += OnMove;
        FallGameManager.Instance.GameEnd += OffMove;
    }

    private void Update()
    {
        if (_isMovable)
        {
            _moveDir = _dir * _moveSpeed;
            _cc.Move(_moveDir * Time.deltaTime);
        }
    }

    public void SetDirection(Vector3 dir)
    {
        _dir = dir;
    }

    void OnMove()
    {
        _isMovable = true;
    }

    void OffMove()
    {
        _isMovable = false;
        gameObject.transform.DOMoveY(_finishTrans.position.y, 3f)
                            .OnComplete(() => 
                            {
                                StartCoroutine(FinishAnimation());
                            });
    }

    IEnumerator FinishAnimation()
    {
        _anim.Play("Landing");
        yield return new WaitForSeconds(1.0f);
        _anim.Play("Clear");
    }
}
