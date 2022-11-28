using System;
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

    [Tooltip("トランプ兵を生成するGenerator")]
    [SerializeField]
    TrumpSolderGenerator _trumpGenerator = default;
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
    public int CurrentRedTrumpCount => _trumpGenerator.CurrentRedTrumpCount;
    public int CurrentBlackTrumpCount => _trumpGenerator.CurrentBlackTrumpCount;
    #endregion

    private void OnDisable()
    {
        QuizGameManager.Instance.QuizSetUp -= ObjectSetUp;
    }

    private void Start()
    {
        //QuizGameManager.Instance.GameSetUp += ObjectSetUp;
        QuizGameManager.Instance.QuizSetUp += ObjectSetUp;
    }
    void ObjectSetUp(QuizType quizType)
    {
        Debug.Log($"現在のクイズ：{quizType}");
        foreach (var t in _trees)
        {
            t.Deploy();
        }

        if (quizType != QuizType.TrumpSolder)
        {
            return;
        }

        _trumpGenerator.Return();
        _trumpGenerator.Generate();
    }
}
