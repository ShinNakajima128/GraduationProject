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
    public int CurrentRedTrumpCount { get; set; }
    public int CurrentBlackTrumpCount { get; set; }
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
