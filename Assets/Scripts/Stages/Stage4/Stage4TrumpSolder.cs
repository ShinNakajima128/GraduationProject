using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage4TrumpSolder : MonoBehaviour
{
    #region serialize
    [Header("Variable")]
    [SerializeField]
    float _maxSpeed = 5.0f;

    [SerializeField]
    float _moveSpeed = 2.0f;

    [SerializeField]
    Stage4TrumpType _trumpType = default;

    [Header("Data")]
    [SerializeField]
    TrumpSolderData _data = default;

    [SerializeField]
    bool _isDirectionTrump = false;
    #endregion
    #region private
    SpriteRenderer _renderer;
    bool _init = false;
    #endregion
    #region public
    #endregion
    #region property
    public Stage4TrumpType CurrentTrumpType => _trumpType;
    public float CurrentMoveSpeed
    {
        get => _moveSpeed;
        set
        {
            if (value <= 0)
            {
                value = 0.1f;
            }
            _moveSpeed = value;
        }
    }
    #endregion

    private void Awake()
    {
        TryGetComponent(out _renderer);
        _init = true;
    }

    private void OnEnable()
    {
        if (_init)
        {
            var randomTrumpType = (Stage4TrumpType)Random.Range(0, 2);
            var sprite = _data.Trumps.FirstOrDefault(t => t.TrumpType == randomTrumpType).TrumpSprite; //一致した種類の画像データ差し替え
            _renderer.sprite = sprite;
            _trumpType = randomTrumpType;
            _moveSpeed = Random.Range(0.1f, _maxSpeed);
        }
    }
    private void OnDisable()
    {
        transform.localPosition = Vector3.zero;
        _trumpType = Stage4TrumpType.OFF;
    }

    private void Update()
    {
        //演出用のトランプ以外は左へ進む
        if (!_isDirectionTrump)
        {
            transform.localPosition -= new Vector3(_moveSpeed * Time.deltaTime, 0f, 0f);
        }
    }
}
