using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using DG.Tweening;

/// <summary>
/// ステージ3の操作画面の機能を持つコンポーネント
/// </summary>
public class Stage3Operation : MonoBehaviour
{
    #region serialize
    [Header("Variables")]
    [SerializeField]
    float _ActiveScaleValue = 1.2f;

    [SerializeField]
    float _animTime = 0.15f;

    [Header("UIObjects")]
    [SerializeField]
    Transform _moveLeftImage = default;

    [SerializeField]
    Transform _moveRightImage = default;

    [SerializeField]
    Transform _rotateLeftImage = default;

    [SerializeField]
    Transform _rotateRightImage = default;

    [SerializeField]
    Image _raiseOverheadTextImage = default;

    [SerializeField]
    Image _shotTextImage = default;

    [Header("Components")]
    [SerializeField]
    Stage3PlayerController _player = default;
    #endregion

    #region private
    PlayerInput _input;
    CanvasGroup _operationGroup;
    bool _isOnLeftMoveAnimed = false;
    bool _isOnRightMoveAnimed = false;
    Tween _shotImageFillTween;
    #endregion

    #region public
    #endregion

    #region property
    public bool IsActived => _operationGroup.alpha == 1;
    #endregion

    private void Awake()
    {
        TryGetComponent(out _input);
        TryGetComponent(out _operationGroup);

        SubscribeAction();

        _operationGroup.alpha = 0;
        _shotTextImage.fillAmount = 0;
        _raiseOverheadTextImage.enabled = true;
        _shotTextImage.enabled = false;
    }

    void SubscribeAction()
    {
        _input.actions["Move"].performed += OnMoveAnimation;
        _input.actions["Move"].canceled += OffMoveAnimation;

        _input.actions["TurnLeft"].started += OnRotateLeftAnimation;
        _input.actions["TurnLeft"].canceled += OffRotateLeftAnimation;

        _input.actions["TurnRight"].started += OnRotateRightAnimation;
        _input.actions["TurnRight"].canceled += OffRotateRightAnimation;

        // 押したとき
        _input.actions["Throw"].started += OnStandby;
        // 離したとき
        _input.actions["Throw"].canceled += OnShot;
    }

    #region OnMoveImageAnimation
    void OnMoveAnimation(InputAction.CallbackContext context)
    {
        if (!IsActived)
        {
            return;
        }
        if (context.performed)
        {
            var value = context.ReadValue<Vector2>();

            if (value.x < -0.005f && !_isOnLeftMoveAnimed)
            {
                _moveLeftImage.DOScale(_ActiveScaleValue, _animTime);
                _moveRightImage.DOScale(1f, _animTime);
                _isOnLeftMoveAnimed = true;
                _isOnRightMoveAnimed = false;
            }
            else if (value.x > 0.005f && !_isOnRightMoveAnimed)
            {
                _moveLeftImage.DOScale(1f, _animTime);
                _moveRightImage.DOScale(_ActiveScaleValue, _animTime);
                _isOnLeftMoveAnimed = false;
                _isOnRightMoveAnimed = true;
            }
        }
        
    }

    void OffMoveAnimation(InputAction.CallbackContext context)
    {
        if (!IsActived)
        {
            return;
        }

        if (context.canceled)
        {
            _moveLeftImage.DOScale(1f, _animTime);
            _moveRightImage.DOScale(1f, _animTime);
            _isOnLeftMoveAnimed = false;
            _isOnRightMoveAnimed = false;
        }
    }
    #endregion

    #region OnRotateImageAnimation
    void OnRotateLeftAnimation(InputAction.CallbackContext context)
    {
        if (!IsActived)
        {
            return;
        }

        if (context.started)
        {
            _rotateLeftImage.DOScale(_ActiveScaleValue, _animTime);
        }
    }

    void OffRotateLeftAnimation(InputAction.CallbackContext context)
    {
        if (!IsActived)
        {
            return;
        }

        if (context.canceled)
        {
            _rotateLeftImage.DOScale(1f, _animTime);
        }
    }

    void OnRotateRightAnimation(InputAction.CallbackContext context)
    {
        if (!IsActived)
        {
            return;
        }

        if (context.started)
        {
            _rotateRightImage.DOScale(_ActiveScaleValue, _animTime);
        }
    }

    void OffRotateRightAnimation(InputAction.CallbackContext context)
    {
        if (!IsActived)
        {
            return;
        }

        if (context.canceled)
        {
            _rotateRightImage.DOScale(1f, _animTime);
        }
    }
    #endregion

    #region OnShotActions
    void OnStandby(InputAction.CallbackContext context)
    {
        if (!IsActived)
        {
            return;
        }

        if (context.started)
        {
            _raiseOverheadTextImage.enabled = false;
            _shotTextImage.enabled = true;

            _shotImageFillTween = _shotTextImage.DOFillAmount(1, 1.25f)
                                                .OnComplete(() => 
                                                {
                                                    _shotImageFillTween = _shotTextImage.transform.DOScale(1.1f, 0.25f)
                                                                            .SetLoops(-1, LoopType.Yoyo);
                                                });
        }
    }
    void OnShot(InputAction.CallbackContext context)
    {
        if (!IsActived)
        {
            return;
        }

        if (context.canceled)
        {
            if (!_player.IsStandbyed)
            {
                _raiseOverheadTextImage.enabled = true;
                _shotImageFillTween.Kill();
                _shotImageFillTween = null;
                _shotTextImage.fillAmount = 0;
                _shotTextImage.enabled = false;
            }
        }
    }
    #endregion
    public void ActivateOperation(bool isActived)
    {
        if (isActived)
        {
            _operationGroup.alpha = 1;
            _raiseOverheadTextImage.enabled = true;
            _shotTextImage.enabled = false;
        }
        else
        {
            _operationGroup.alpha = 0;
            _shotTextImage.fillAmount = 0;
            
            if (_shotImageFillTween != null)
            {
                _shotImageFillTween.Kill();
                _shotImageFillTween = null;
            }
        }
    }
}
