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
        _cc.enabled = false;
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
        _cc.enabled = true;
        _isMovable = true;
    }

    void OffMove()
    {
        _cc.enabled = false;
        _isMovable = false;
        gameObject.transform.DOMoveY(_finishTrans.position.y - 3, 1.5f)
                            .OnComplete(() => 
                            {
                                gameObject.transform.DOMove(new Vector3(0f, 10f,0f), 0f);
                                gameObject.transform.DOMoveY(_finishTrans.position.y + 5f, 2.0f);
                                StartCoroutine(FinishAnimation());
                            });
    }

    IEnumerator FinishAnimation()
    {
        yield return new WaitForSeconds(2.0f);
        _anim.Play("Landing");
        yield return new WaitForSeconds(2.0f);
        _anim.Play("Clear");
    }
}
