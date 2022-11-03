using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

public class Stage1Player : MonoBehaviour, IEffectable, IDamagable
{
    #region serialize
    [Tooltip("エフェクトを再生する位置")]
    [SerializeField]
    Transform _effectPos = default;

    [Tooltip("プレイヤーのモデルオブジェクト")]
    [SerializeField]
    GameObject _playerModel = default;

    [Tooltip("ダメージ時の点滅回数")]
    [SerializeField]
    int _blinksNum = 5;

    [Tooltip("ダメージ時の点滅間隔")]
    [SerializeField]
    float _blinksInterVal = 0.2f;
    #endregion

    #region private
    PlayerInput _input;
    IMovable _move;
    /// <summary> 無敵状態かどうか </summary>
    bool _isInvincibled = false;
    #endregion

    #region property
    public Transform EffectPos => _effectPos;

    public bool IsInvincibled => _isInvincibled;
    #endregion

    private void Awake()
    {
        TryGetComponent(out _input);
        TryGetComponent(out _move);
    }

    private void OnEnable()
    {
        _input.actions["Move"].performed += OnMove;
        _input.actions["Move"].canceled += OnMoveStop;
    }
    private void OnDisable()
    {
        _input.actions["Move"].performed -= OnMove;
        _input.actions["Move"].canceled -= OnMoveStop;
    }
    private void OnMove(InputAction.CallbackContext obj)
    {
        var value = obj.ReadValue<Vector2>();
        _move.SetDirection(value);
    }
    private void OnMoveStop(InputAction.CallbackContext obj)
    {
        _move.SetDirection(Vector3.zero);
    }

    public void Damage(int value)
    {
        StartCoroutine(DamageCoroutine());
    }

    IEnumerator DamageCoroutine()
    {
        _isInvincibled = true;
        var wait = new WaitForSeconds(_blinksInterVal);

        for (int i = 0; i < _blinksNum; i++)
        {
            _playerModel.SetActive(!_playerModel.activeSelf);
            yield return wait;
        }

        _isInvincibled = false;
        _playerModel.SetActive(true);
    }
}
