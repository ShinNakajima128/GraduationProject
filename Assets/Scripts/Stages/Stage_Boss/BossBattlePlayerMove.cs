using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBattlePlayerMove : MonoBehaviour, IMovable
{
    #region serialize
    [SerializeField]
    float _moveSpeed = 5.0f;

    [SerializeField]
    float _turnSpeed = 5.0f;

    [SerializeField]
    float _rigidTime = 1.0f;
    #endregion

    #region private
    Rigidbody _rb;
    Animator _anim;
    Vector3 _dir;
    bool _isMoving;
    Coroutine _stoppingCoroutine;
    #endregion

    private void Awake()
    {
        TryGetComponent(out _rb);
        TryGetComponent(out _anim);
    }

    private void Start()
    {
        PlayerMovable(false);
        BossStageManager.Instance.CharacterMovable += PlayerMovable;
        BossStageManager.Instance.DirectionSetUp += ResetAnimation;
        EventManager.ListenEvents(Events.Boss_GroundShake, Stopping);
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
                Quaternion targetRotation = Quaternion.LookRotation(_dir);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * _turnSpeed);
                Vector3 velocity = _dir.normalized * _moveSpeed;
                velocity.y = _rb.velocity.y;
                _rb.velocity = velocity;
            }

            CharacterAnimation();
        }
        else
        {
            _rb.velocity = new Vector3(0f, _rb.velocity.y, 0f);
        }
    }
    public void SetDirection(Vector3 dir)
    {
        _dir = Vector3.forward * dir.y + Vector3.right * dir.x;
        _dir = Camera.main.transform.TransformDirection(_dir);    // メインカメラを基準に入力方向のベクトルを変換する
        _dir.y = 0;  // y 軸方向はゼロにして水平方向のベクトルにする
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

    /// <summary>
    /// プレイヤーのアニメーションをリセットする
    /// </summary>
    void ResetAnimation()
    {
        _anim.SetFloat("Move", 0f);
    }

    void Stopping()
    {
        //演出中はアニメーションのみ
        if (!_isMoving)
        {
            _anim.CrossFadeInFixedTime("Stagger", 0.1f);
        }
        //戦闘中はしばらく動けなくなるコルーチンを開始
        else
        {
            if (_stoppingCoroutine != null)
            {
                StopCoroutine(_stoppingCoroutine);
                _stoppingCoroutine = null;
            }

            StartCoroutine(RigidCoroutine());
        }
    }

    IEnumerator RigidCoroutine()
    {
        PlayerMovable(false);
        _anim.CrossFadeInFixedTime("Stagger", 0.1f);

        yield return new WaitForSeconds(_rigidTime);

        PlayerMovable(true);
    }
}
