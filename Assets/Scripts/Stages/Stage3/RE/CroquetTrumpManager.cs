using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CroquetTrumpManager : MonoBehaviour
{
    #region serialize
    [Header("Variables")]
    [Tooltip("gvΊΜΆ¬")]
    [SerializeField]
    int _generateCount = 15;

    [Tooltip("gvΊΜcΜΤ")]
    [SerializeField]
    float _depthDistance = 1.0f;

    [Tooltip("gvΊΜ‘ϋΚΜ·")]
    [SerializeField]
    float _sideMisalign = 0.1f;

    [Tooltip("ΗΜS©ηΜ£")]
    [SerializeField]
    float _centerWallDist = 0.5f;

    [Tooltip("ΆΜΗΜTransform")]
    [SerializeField]
    Transform _leftWall = default;

    [Tooltip("EΜΗΜTransform")]
    [SerializeField]
    Transform _rightWall = default;

    [Tooltip("ΞίΙΐΤΜξ")]
    [SerializeField]
    private Transform _pointOfXSlant;

    [Tooltip("ΌόγΙΐΤΜξ")]
    [SerializeField]
    private Transform _pointOfStraight;

    [Tooltip("π·ΙΐΤΜξ")]
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

    public void SetTrumpSolder(AlignmentType type)
    {
        if (_debugMode)
        {
            type = _debugAlignmentType;
        }
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
    /// ΌόΕgvΊπZbg
    /// </summary>
    void SetStraight()
    {
        var pos = _pointOfStraight.position;

        for (int index = 0; index < _trumpsList.Count; index++)
        {
            var trump = _trumpsList[index];

            if (index % 2 == 0)
            {
                // gvΜνήπwθ
                trump.ChangeRandomPattern(TrumpColorType.Red);
                trump.gameObject.transform.position = pos;
            }
            else
            {
                // gvΜνήπwθ
                trump.ChangeRandomPattern(TrumpColorType.Black);
                trump.gameObject.transform.position = pos;
            }

            pos.z += _depthDistance;
        }
    }

    /// <summary>
    /// π·ΕgvΊπZbg
    /// </summary>
    void SetCrossing()
    {
        var leftPoint = _pointOfXrossLeft.position;
        var rightPoint = _pointOfXrossRight.position;

        bool isLeftPoint_RightDir = true;
        bool isRightPoint_LeftDir = true;

        for (int index = 0; index < _trumpsList.Count; index++)
        {
            var trump = _trumpsList[index];

            // τΤΪ
            if (index % 2 == 0)
            {
                // ΤΙΟX
                trump.ChangeRandomPattern(TrumpColorType.Red);
                trump.gameObject.transform.position = leftPoint;

                if (isLeftPoint_RightDir)
                {
                    leftPoint.x += _sideMisalign;
                    //trump.ChangeMoveState(MoveDir.Right);

                    if (leftPoint.x >= _rightWall.position.x - _centerWallDist)
                    {
                        isLeftPoint_RightDir = false;
                    }
                }
                else
                {
                    leftPoint.x -= _sideMisalign;
                    //trump.ChangeMoveState(MoveDir.Left);

                    if (leftPoint.x <= _leftWall.position.x + _centerWallDist)
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
            // οΤΪ
            else
            {
                // ΙΟX
                trump.ChangeRandomPattern(TrumpColorType.Black);
                trump.gameObject.transform.position = rightPoint;

                if (isRightPoint_LeftDir)
                {
                    rightPoint.x -= _sideMisalign;
                    //trump.ChangeMoveState(MoveDir.Left);

                    if (rightPoint.x <= _leftWall.position.x + _centerWallDist)
                    {
                        isRightPoint_LeftDir = false;
                    }
                }
                else
                {
                    rightPoint.x += _sideMisalign;
                    //trump.ChangeMoveState(MoveDir.Right);

                    if (rightPoint.x >= _rightWall.position.x - _centerWallDist)
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
    /// ΞίΙgvΊπZbg
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
                // gvΜνήπwθ
                trump.ChangeRandomPattern(TrumpColorType.Red);
                trump.gameObject.transform.position = pos;
            }
            else
            {
                // gvΜνήπwθ
                trump.ChangeRandomPattern(TrumpColorType.Black);
                trump.gameObject.transform.position = pos;
            }

            if (pos.x >= _rightWall.position.x - (_sideMisalign + _centerWallDist))
            {
                isRightDir = false;
            }
            else if (pos.x <= _leftWall.position.x + (_sideMisalign + _centerWallDist))
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
    /// Όό
    /// </summary>
    Straight,
    /// <summary>
    /// π·
    /// </summary>
    Crossing,
    /// <summary>
    /// Ξί
    /// </summary>
    Slant
}
