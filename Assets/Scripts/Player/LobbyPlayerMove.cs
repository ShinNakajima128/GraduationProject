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
    #endregion

    private void Awake()
    {
        TryGetComponent(out _rb);
        TryGetComponent(out _anim);
    }

    private void FixedUpdate()
    {
        if (_dir == Vector3.zero)
        {
            _rb.velocity = new Vector3(0f, _rb.velocity.y, 0f);
        }
        else
        {
            var look = Camera.main.transform;
            _dir = look.forward * _dir.z + look.right * _dir.x;
            //_dir = Camera.main.transform.TransformDirection(_dir);    // ���C���J��������ɓ��͕����̃x�N�g����ϊ�����
            _dir.y = 0;  // y �������̓[���ɂ��Đ��������̃x�N�g���ɂ���
            Debug.Log(_dir);
            Quaternion targetRotation = Quaternion.LookRotation(_dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * _turnSpeed);
        }

        Vector3 velocity = _dir.normalized * _moveSpeed;
        velocity.y = _rb.velocity.y;
        _rb.velocity = velocity;
        CharacterAnimation();
    }

    public void SetDirection(Vector3 dir)
    {
        _dir = Vector3.forward * dir.y + Vector3.right * dir.x;
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
}
