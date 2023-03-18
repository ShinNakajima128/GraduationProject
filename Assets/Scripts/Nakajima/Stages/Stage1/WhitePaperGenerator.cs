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

    #region property
    public static WhitePaperGenerator Instance { get; private set; }
    #endregion

    private void Awake()
    {
        Instance = this;
    }

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

    /// <summary>
    /// �A�N�e�B�u�̃I�u�W�F�N�g��S�Ĕ�A�N�e�B�u�����A�������I������
    /// </summary>
    void StopGenerate()
    {
        _wpc.Return();
        _isInGamed = false;
    }

    /// <summary>
    /// �������Ԃ�ݒ肷��
    /// </summary>
    /// <param name="value"> ��������Ԋu </param>
    public void SetInterval(float value)
    {
        _generateInterval = value;
    }
}
