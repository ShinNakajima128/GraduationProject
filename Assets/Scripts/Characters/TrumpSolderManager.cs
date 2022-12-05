using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TrumpSolderManager : MonoBehaviour
{
    #region serialize
    [Header("Objects")]
    [Tooltip("トランプ兵のPrefab")]
    [SerializeField]
    BattleAreaTrumpSolder _trumpPrefab = default;

    [SerializeField]
    Transform _trumpSolderParent = default;

    [SerializeField]
    BossAreaPositions[] _areaPositions = default;
    #endregion
    #region private
    List<BattleAreaTrumpSolder> _trumpSolderList = new List<BattleAreaTrumpSolder>();
    readonly List<Animator> _frontTrumpAnims = new List<Animator>();
    readonly List<Animator> _backTrumpAnims = new List<Animator>();
    readonly List<Animator> _leftTrumpAnims = new List<Animator>();
    readonly List<Animator> _rightTrumpAnims = new List<Animator>() ;
    #endregion
    #region public
    #endregion
    #region property
    public List<Animator> FrontTrumpAnims => _frontTrumpAnims;
    public List<Animator> BackTrumpAnims => _backTrumpAnims;
    public List<Animator> LeftTrumpAnims => _leftTrumpAnims;
    public List<Animator> RightTrumpAnims => _rightTrumpAnims;
    #endregion

    private void Awake()
    {
        int generateNum = 0;
        foreach (var p in _areaPositions)
        {
            generateNum += p.LineUpTrans.Length;
        }
        Debug.Log($"トランプ兵の生成数：{generateNum}");

        for (int i = 0; i < generateNum; i++)
        {
            var trump = Instantiate(_trumpPrefab, _trumpSolderParent);
            _trumpSolderList.Add(trump);
        }
    }

    private void Start()
    {
        int index = 0;
        for (int i = 0; i < _areaPositions.Length; i++)
        {
            for (int n = 0; n < _areaPositions[i].LineUpTrans.Length; n++)
            {
                _trumpSolderList[index].transform.localPosition = _areaPositions[i].LineUpTrans[n].position;
                _trumpSolderList[index].DirType = _areaPositions[i].DirectionType;
                Vector3 dir = default;

                var anim = _trumpSolderList[index].GetComponent<Animator>();
                
                switch (_trumpSolderList[index].DirType)
                {
                    case DirectionType.Front:
                        dir.y = 0f;
                        _frontTrumpAnims.Add(anim);
                        break;
                    case DirectionType.Back:
                        dir.y = 180f;
                        _backTrumpAnims.Add(anim);
                        break;
                    case DirectionType.Left:
                        _leftTrumpAnims.Add(anim);
                        dir.y = 90f;
                        break;
                    case DirectionType.Right:
                        _rightTrumpAnims.Add(anim);
                        dir.y = 270f;
                        break;
                }
                _trumpSolderList[index].transform.DOLocalRotate(dir, 0f);
                index++;
            }
        }
    }
}

[Serializable]
public struct BossAreaPositions
{
    public DirectionType DirectionType;
    public Transform[] LineUpTrans;
}
public enum DirectionType
{
    Front,
    Back,
    Left,
    Right
}
