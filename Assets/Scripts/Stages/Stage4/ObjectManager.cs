using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �X�e�[�W4�̃I�u�W�F�N�g���Ǘ�����}�l�[�W���[
/// </summary>
public class ObjectManager : MonoBehaviour
{
    #region serialize
    [SerializeField]
    int _groundNum = 20;

    [SerializeField]
    float _generateInterval = 6.05f;

    [Header("Objects")]
    [Tooltip("�n�ʃI�u�W�F�N�g")]
    [SerializeField]
    GameObject[] _generateModels = default;

    [SerializeField]
    Transform _groundsParent = default;

    [Tooltip("Scene��ɔz�u���Ă����")]
    [SerializeField]
    RoseTree[] _trees = default;

    [Header("Components")]
    [Tooltip("�g�����v���𐶐�����Generator")]
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
    //public int CurrentRedTrumpCount => _trumpGenerator.CurrentRedTrumpCount;
    //public int CurrentBlackTrumpCount => _trumpGenerator.CurrentBlackTrumpCount;
    #endregion

    private void OnDisable()
    {
        QuizGameManager.Instance.QuizSetUp -= ObjectSetUp;
    }

    private void Start()
    {
        QuizGameManager.Instance.QuizSetUp += ObjectSetUp;
        //GroundSetup();
    }
    void ObjectSetUp(QuizType quizType)
    {
        Debug.Log($"���݂̃N�C�Y�F{quizType}");
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

    void GroundSetup()
    {
        for (int i = 0; i < _groundNum; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, _generateModels.Length);

            var go = Instantiate(_generateModels[randomIndex], _groundsParent);

            go.transform.localPosition = new Vector3(go.transform.localPosition.x + i * _generateInterval, go.transform.localPosition.y, go.transform.localPosition.z);
        }
    }
}
