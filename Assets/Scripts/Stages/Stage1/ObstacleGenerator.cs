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

    void Start()
    {
        StartCoroutine(OnGenerate());
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
        while (true)
        {
            int rand = Random.Range(0, _obstacleList.Count);
            Generate(rand);

            yield return new WaitForSeconds(_generateInterval);
        }
    }
}
public enum ObstacleType
{
    Chair,
    Clock,
    Table,
    Mirror
}
