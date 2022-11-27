using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �X�e�[�W4�̃I�u�W�F�N�g���Ǘ�����}�l�[�W���[
/// </summary>
public class ObjectManager : MonoBehaviour
{
    #region serialize
    [Header("Objects")]
    [Tooltip("Scene��ɔz�u���Ă����")]
    [SerializeField]
    RoseTree[] _trees = default;

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
    public int CurrentRedTrumpCount => _trumpGenerator.CurrentRedTrumpCount;
    public int CurrentBlackTrumpCount => _trumpGenerator.CurrentBlackTrumpCount;
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
        _trumpGenerator.Return();
        _trumpGenerator.Generate();
    }
}
