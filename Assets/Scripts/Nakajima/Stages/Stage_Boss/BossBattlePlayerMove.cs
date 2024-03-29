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

    [SerializeField]
    float _growMoveSpeed = 8.0f;

    [SerializeField]
    float _growAnimSpeed = 0.5f;
    #endregion

    #region private
    Rigidbody _rb;
    Animator _anim;
    Vector3 _dir;
    bool _isMoving;
    bool _isGrowuped = false;
    Coroutine _stoppingCoroutine;
    float _currentSpeed;
    #endregion

    private void Awake()
    {
        TryGetComponent(out _rb);
        TryGetComponent(out _anim);
        _currentSpeed = _moveSpeed;
    }

    private void Start()
    {
        PlayerMovable(false);
        BossStageManager.Instance.CharacterMovable += PlayerMovable;
        BossStageManager.Instance.DirectionSetUp += ResetAnimation;
        BossStageManager.Instance.GameOver += () =>
        {
            if (_stoppingCoroutine != null)
            {
                StopCoroutine(_stoppingCoroutine);
                _stoppingCoroutine = null;
            }
            ResetAnimation();
            PlayerMovable(false);
            _anim.CrossFadeInFixedTime("Death", 0.1f);
        };
        EventManager.ListenEvents(Events.BossStage_FrontAlice, () => { _anim.CrossFadeInFixedTime("Overlook", 0.2f); });
        EventManager.ListenEvents(Events.BossStage_BehindAlice, () => { _anim.CrossFadeInFixedTime("No", 0.2f); });
        //EventManager.ListenEvents(Events.BossStage_BehindAlice_RE2, () => { _anim.CrossFadeInFixedTime("OpenArms", 0.2f); });
        EventManager.ListenEvents(Events.Boss_GroundShake, Stopping);
        EventManager.ListenEvents(Events.BossStage_GrowAlice, ChangeGrowAliceMove);
        EventManager.ListenEvents(Events.BossStage_DiminishAlice, ChangeDiminishAliceMove);
        HPManager.Instance.DamageAction += () => { _anim.CrossFadeInFixedTime("Damage", 0.2f); };
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
                Vector3 velocity = _dir.normalized * _currentSpeed;
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
            _anim.SetFloat("MoveSpeed", velo.magnitude);
        }
    }

    void PlayerMovable(bool isMove)
    {
        _isMoving = isMove;
    }

    void ChangeGrowAliceMove()
    {
        _currentSpeed = _growMoveSpeed;
        _anim.speed = _growAnimSpeed;
        _isGrowuped = true;
        print(_anim.GetFloat("Move"));
    }

    void ChangeDiminishAliceMove()
    {
        _currentSpeed = _moveSpeed;
        _anim.speed = 1f;
        _isGrowuped = false;
    }

    /// <summary>
    /// プレイヤーのアニメーションをリセットする
    /// </summary>
    void ResetAnimation()
    {
        _anim.SetFloat("MoveSpeed", 0f);
    }

    void Stopping()
    {
        //既にHPが消失している場合は処理を行わない
        if (HPManager.Instance.IsLosted || _isGrowuped)
        {
            return;
        }

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

            _stoppingCoroutine = StartCoroutine(RigidCoroutine());
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
