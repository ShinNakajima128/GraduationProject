using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CroquetTrumpManager : MonoBehaviour
{
    #region serialize
    [Header("Variables")]
    [Tooltip("トランプ兵の生成数")]
    [SerializeField]
    int _generateCount = 15;

    [Tooltip("トランプ兵の縦の隙間")]
    [SerializeField]
    float _depthDistance = 1.0f;

    [Tooltip("トランプ兵の横方面の差")]
    [SerializeField]
    float _sideMisalign = 0.1f;

    [Tooltip("左の壁のTransform")]
    [SerializeField]
    Transform _leftWall = default;

    [Tooltip("右の壁のTransform")]
    [SerializeField]
    Transform _rightWall = default;

    [Tooltip("斜めに並ぶ時の基準")]
    [SerializeField]
    private Transform _pointOfXSlant;

    [Tooltip("直線上に並ぶ時の基準")]
    [SerializeField]
    private Transform _pointOfStraight;

    [Tooltip("交差に並ぶ時の基準")]
    [SerializeField]
    private Transform _pointOfXrossLeft;

    [SerializeField]
    private Transform _pointOfXrossRight;

    [Header("Components")]
    [SerializeField]
    CroquetTrump _croquetTrumpPrefab = default;

    [SerializeField]
    Transform _trumpsParent = default;
    #endregion

    [Header("Debug")]
    [SerializeField]
    bool _debugMode = default;

    [SerializeField]
    AlignmentType _debugAlignmentType = default;

    #region private
    List<CroquetTrump> _trumpsList = new List<CroquetTrump>();
    #endregion

    #region public
    #endregion

    #region property
    public static CroquetTrumpManager Instance { get; private set; }
    public int CurrentRedTrumpCount => _trumpsList.Count(t => t.CurrentColorType == TrumpColorType.Red && t.gameObject.activeSelf);
    public int CurrentBlackTrumpCount => _trumpsList.Count(t => t.CurrentColorType == TrumpColorType.Black && t.gameObject.activeSelf);
    #endregion

    private void Awake()
    {
        Instance = this;

        for (int i = 0; i < _generateCount; i++)
        {
            var trump = Instantiate(_croquetTrumpPrefab, _trumpsParent);
            trump.transform.localPosition = Vector3.zero;
            _trumpsList.Add(trump);
        }
    }

    IEnumerator Start()
    {
        yield return null;

        if (_debugMode)
        {
            SetTrumpSolder(_debugAlignmentType);
        }
    }

    public void SetTrumpSolder(AlignmentType type)
    {
        foreach (var t in _trumpsList)
        {
            t.gameObject.SetActive(true);
        }

        switch (type)
        {
            case AlignmentType.Straight:
                SetStraight();
                break;
            case AlignmentType.Crossing:
                SetCrossing();
                break;
            case AlignmentType.Slant:
                SetSlant();
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 直線でトランプ兵をセット
    /// </summary>
    void SetStraight()
    {
        var pos = _pointOfStraight.position;

        for (int index = 0; index < _trumpsList.Count; index++)
        {
            var trump = _trumpsList[index];

            if (index % 2 == 0)
            {
                // トランプの種類を指定
                trump.ChangeRandomPattern(TrumpColorType.Red);
                trump.gameObject.transform.position = pos;
            }
            else
            {
                // トランプの種類を指定
                trump.ChangeRandomPattern(TrumpColorType.Black);
                trump.gameObject.transform.position = pos;
            }

            pos.z += _depthDistance;
        }
    }

    /// <summary>
    /// 交差でトランプ兵をセット
    /// </summary>
    void SetCrossing()
    {
        var leftPoint = _pointOfXrossLeft.position;
        var rightPoint = _pointOfXrossRight.position;

        bool isLeftPoint_RightDir = true;
        bool isRightPoint_LeftDir = true;

        for (int i = 0; i < _trumpsList.Count; i++)
        {
            var trump = _trumpsList[i];

            if (i % 2 == 0)
            {
                trump.ChangeRandomPattern(TrumpColorType.Red);
                trump.gameObject.transform.position = leftPoint;

                if (isLeftPoint_RightDir)
                {
                    leftPoint.x += _sideMisalign;
                    trump.ChangeMoveState(MoveDir.Left);

                    if (leftPoint.x >= _rightWall.position.x - (_sideMisalign + 1f))
                    {
                        isLeftPoint_RightDir = false;
                    }
                }
                else
                {
                    leftPoint.x -= _sideMisalign;
                    trump.ChangeMoveState(MoveDir.Right);

                    if (leftPoint.x <= _leftWall.position.x + (_sideMisalign + 1f))
                    {
                        isLeftPoint_RightDir = true;
                    }
                }

                if (isLeftPoint_RightDir)
                {
                    trump.ChangeMoveState(MoveDir.Right);
                }
                else
                {
                    trump.ChangeMoveState(MoveDir.Left);
                }
            }
            else
            {
                trump.ChangeRandomPattern(TrumpColorType.Black);
                trump.gameObject.transform.position = rightPoint;

                if (isRightPoint_LeftDir)
                {
                    rightPoint.x += _sideMisalign;

                    if (rightPoint.x >= _rightWall.position.x - (_sideMisalign + 1f))
                    {
                        isRightPoint_LeftDir = false;
                    }
                }
                else
                {
                    rightPoint.x -= _sideMisalign;

                    if (rightPoint.x <= _leftWall.position.x + (_sideMisalign + 1f))
                    {
                        isRightPoint_LeftDir = true;
                    }
                }

                if (isRightPoint_LeftDir)
                {
                    trump.ChangeMoveState(MoveDir.Left);
                }
                else
                {
                    trump.ChangeMoveState(MoveDir.Right);
                }
            } 
            
            leftPoint.z += _depthDistance;

            rightPoint.z += _depthDistance;
        }
    }

    /// <summary>
    /// 斜めにトランプ兵をセット
    /// </summary>
    void SetSlant()
    {
        var pos = _pointOfXSlant.position;
        bool isRightDir = true;

        for (int index = 0; index < _trumpsList.Count; index++)
        {
            var trump = _trumpsList[index];

            if (index % 2 == 0)
            {
                // トランプの種類を指定
                trump.ChangeRandomPattern(TrumpColorType.Red);
                trump.gameObject.transform.position = pos;
            }
            else
            {
                // トランプの種類を指定
                trump.ChangeRandomPattern(TrumpColorType.Black);
                trump.gameObject.transform.position = pos;
            }

            if (pos.x >= _rightWall.position.x - (_sideMisalign + 1f))
            {
                isRightDir = false;
            }
            else if (pos.x <= _leftWall.position.x + (_sideMisalign + 1f))
            {
                isRightDir = true;
            }

            if (isRightDir)
            {
                pos.x += _sideMisalign;
                trump.ChangeMoveState(MoveDir.Right);
            }
            else
            {
                pos.x -= _sideMisalign;
                trump.ChangeMoveState(MoveDir.Left);
            }

            pos.z += _depthDistance;
        }
    }
}


public enum AlignmentType
{
    /// <summary>
    /// 直線
    /// </summary>
    Straight,
    /// <summary>
    /// 交差
    /// </summary>
    Crossing,
    /// <summary>
    /// 斜め
    /// </summary>
    Slant
}
