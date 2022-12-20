using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CroquetTrump : TrumpSolder
{
    #region serialize
    [SerializeField]
    float _moveSpeed = 5.0f;

    [SerializeField]
    MoveDir _currentDir = default;
    #endregion

    #region private
    #endregion

    #region public
    #endregion

    #region property
    #endregion

    private void OnDisable()
    {
        transform.localPosition = Vector3.zero;
    }

    protected override void Start()
    {
        base.Start();
        ChangeMoveState(MoveDir.Left);
    }

    private void FixedUpdate()
    {
        if (_currentDir != MoveDir.Blowoff)
        {
            Move();
        }
    }

    private void Move()
    {
        var pos = transform.localPosition;

        switch (_currentDir)
        {
            case MoveDir.None:
                break;
            case MoveDir.Left:
                // 左に動く
                pos.x -= _moveSpeed * Time.deltaTime;
                break;
            // 右に動く
            case MoveDir.Right:
                pos.x += _moveSpeed * Time.deltaTime;
                break;
            default:
                break;
        }

        transform.localPosition = pos;
    }

    public void ChangeMoveState(MoveDir dir)
    {
        _currentDir = dir;
    }

    /// <summary>
    /// 吹き飛ぶアニメーションのコルーチン
    /// </summary>
    IEnumerator BlowoffCoroutine()
    {
        yield return transform.DOLocalMoveY(5, 1.0f)
                              .SetEase(Ease.OutQuart);

        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            ChangeMoveState(MoveDir.Blowoff);
            CroquetGameManager.Instance.AddScore(CurrentColorType);
            StartCoroutine(BlowoffCoroutine());
        }

        if (other.CompareTag("Block"))
        {
            switch (_currentDir)
            {
                case MoveDir.None:
                    break;
                case MoveDir.Left:
                    ChangeMoveState(MoveDir.Right);
                    break;
                case MoveDir.Right:
                    ChangeMoveState(MoveDir.Left);
                    break;
                case MoveDir.Blowoff:
                    break;
                default:
                    break;
            }
        }
    }
}

/// <summary>
/// 進行方向
/// </summary>
public enum MoveDir
{
    None,
    Left,
    Right,
    Blowoff
}
