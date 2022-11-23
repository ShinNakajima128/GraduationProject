using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ステージ4のオブジェクトを管理するマネージャー
/// </summary>
public class ObjectManager : MonoBehaviour
{
    #region serialize
    [Header("Objects")]
    [Tooltip("Scene上に配置してある木")]
    [SerializeField]
    RoseTree[] _trees = default;
    #endregion
    #region private
    #endregion
    #region property
    public int CurrentRedRoseCount 
    { 
        get 
        {
            var count = 0;

            foreach (var t in _trees)
            {
                count += t.RedRoseCount;
            }
            return count;
        } 
    }

    public int CurrentWhiteRoseCount
    {
        get
        {
            var count = 0;

            foreach (var t in _trees)
            {
                count += t.WhiteRoseCount;
            }
            return count;
        }
    }
    #endregion

    private void Start()
    {
        QuizGameManager.Instance.GameSetUp += ObjectSetUp;
    }
    void ObjectSetUp()
    {
        foreach (var t in _trees)
        {
            t.Deploy();
        }
    }
}
