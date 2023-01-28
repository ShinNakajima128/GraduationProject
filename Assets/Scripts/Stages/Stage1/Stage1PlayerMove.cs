using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// ステージ1のプレイヤーの移動機能を管理する
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class Stage1PlayerMove : MonoBehaviour, IMovable
{
    #region serialize
    [SerializeField]
    float _moveSpeed = 5.0f;

    [SerializeField]
    Transform _finishTrans = default;
    #endregion

    #region private
    Animator _anim;
    Rigidbody _rb;
    Vector3 _dir;
    Vector3 _moveDir;
    bool _isMovable = false;
    #endregion

    private void Awake()
    {
        TryGetComponent(out _anim);
        TryGetComponent(out _rb);
    }

    private void Start()
    {
        _rb.Sleep();
        FallGameManager.Instance.GameStart += OnMove;
        FallGameManager.Instance.GameEnd += OffMove;
    }

    private void FixedUpdate()
    {
        if (_isMovable)
        {
            _moveDir = _dir.normalized * _moveSpeed;

            if (_moveDir != Vector3.zero)
            {
                _rb.velocity += _moveDir * Time.fixedDeltaTime;

            }
            //if (_moveDir != Vector3.zero)
            //{
            //    _cc.Move(_moveDir * Time.deltaTime);

            //}
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
        _rb.Sleep();
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

        yield return new WaitForSeconds(0.3f);
        AudioManager.PlaySE(SEType.Alice_Landing);
        yield return new WaitForSeconds(2.7f);
        _anim.Play("Clear");
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.name);
    }
}
