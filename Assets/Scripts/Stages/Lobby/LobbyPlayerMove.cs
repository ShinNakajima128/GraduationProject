using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class LobbyPlayerMove : MonoBehaviour, IMovable
{
    #region serialize
    [SerializeField]
    float _moveSpeed = 5.0f;

    [SerializeField]
    float _turnSpeed = 5.0f;
    #endregion

    #region private
    Rigidbody _rb;
    Animator _anim;
    Vector3 _dir;
    Vector3 _inputMove;
    bool _isMoving;
    #endregion

    private void Awake()
    {
        TryGetComponent(out _rb);
        TryGetComponent(out _anim);
    }

    private void Start()
    {
        LobbyManager.Instance.PlayerMove += PlayerMovable;
    }

    private void FixedUpdate()
    {
        if (_isMoving)
        {
            if (_dir == Vector3.zero)
            {
                _rb.velocity = new Vector3(0f, _rb.velocity.y, 0f);
            }
            else
            {
                _inputMove = Vector3.forward * _dir.y + Vector3.right * _dir.x;
                _inputMove = Camera.main.transform.TransformDirection(_inputMove);
                Quaternion targetRotation = Quaternion.LookRotation(_inputMove);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * _turnSpeed);
                Vector3 velocity = _inputMove.normalized * _moveSpeed;
                velocity.y = _rb.velocity.y;
                _rb.velocity = velocity;
            }

            CharacterAnimation();
        }
    }

    public void SetDirection(Vector3 dir)
    {
        _dir = dir;
    }
    void CharacterAnimation()
    {
        if (_anim)
        {
            Vector3 velo = _rb.velocity;
            velo.y = 0;
            _anim.SetFloat("Move", velo.magnitude);
        }
    }

    void PlayerMovable(bool isMove)
    {
        _isMoving = isMove;
    }
}
