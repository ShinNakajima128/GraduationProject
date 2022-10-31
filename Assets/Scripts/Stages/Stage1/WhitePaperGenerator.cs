using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhitePaperGenerator : MonoBehaviour
{
    #region serialize
    [Tooltip("�Q�[���J�n���̔����������Ԃ̊Ԋu")]
    [SerializeField]
    float _generateInterval = 3.0f;

    [Tooltip("�����𐶐�����ʒu")]
    [SerializeField]
    Transform[] _generateTrans = default;

    [Tooltip("�������锒����Controller")]
    [SerializeField]
    WhitePaperController _wpc = default;
    #endregion

    #region private
    bool _isInGamed;
    #endregion

    void Start()
    {
        FallGameManager.Instance.GameStart += () => StartCoroutine(OnGenerate());
        FallGameManager.Instance.GameEnd += StopGenerate;
    }
    void Generate()
    {
        int randPos = Random.Range(0, _generateTrans.Length);
        _wpc.Use(_generateTrans[randPos].position);
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
            Generate();

            yield return new WaitForSeconds(_generateInterval);
        }
    }

    void StopGenerate()
    {
        _isInGamed = false;
    }
}
