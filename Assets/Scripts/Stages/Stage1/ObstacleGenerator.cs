using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �X�e�[�W1�̏�Q���𐶐�����N���X
/// </summary>
public class ObstacleGenerator : MonoBehaviour
{
    #region serialize
    [Tooltip("�Q�[���J�n���̏�Q���������Ԃ̊Ԋu")]
    [SerializeField]
    float _generateInterval = 3.0f;

    [Tooltip("��Q���𐶐�����ʒu")]
    [SerializeField]
    Transform[] _generateTrans = default;

    [Tooltip("���������Q����List")]
    [SerializeField]
    List<ObstacleController> _obstacleList = new List<ObstacleController>();
    #endregion
    #region private
    bool _isInGamed;
    #endregion

    void Start()
    {
        FallGameManager.Instance.GameStart +=() => StartCoroutine(OnGenerate());
        FallGameManager.Instance.GameEnd += StopGenerate;
    }

    void Generate(int index)
    {
        int randPos = Random.Range(0, _generateTrans.Length);
        _obstacleList[index].Use(_generateTrans[randPos].position);
    }

    /// <summary>
    /// �����J�n
    /// </summary>
    IEnumerator OnGenerate()
    {
        yield return null;
        _isInGamed = true;

        while (_isInGamed)
        {
            int rand = Random.Range(0, _obstacleList.Count);
            Generate(rand);

            yield return new WaitForSeconds(_generateInterval);
        }
    }
    /// <summary>
    /// �A�N�e�B�u�̃I�u�W�F�N�g��S�Ĕ�A�N�e�B�u�����A�������I������
    /// </summary>
    void StopGenerate()
    {
        foreach (var o in _obstacleList)
        {
            o.Return();
        }
        _isInGamed = false;
    }
}
public enum ObstacleType
{
    Chair,
    Clock,
    Table,
    Mirror
}
