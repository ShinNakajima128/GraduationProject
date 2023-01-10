using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebrisGenerator : MonoBehaviour
{
    #region serialize
    [Header("Variables")]
    [Tooltip("���ꂫ�𐶐����鐔")]
    [SerializeField]
    int _generateCount = 3;

    [Tooltip("�����^�C�~���O�Ő����������ꂫ�̐����Ԋu")]
    [SerializeField]
    float _sometimeGenerateInterval = 0.2f;

    [Header("Objects")]
    [Tooltip("��Q���𐶐�����ʒu")]
    [SerializeField]
    protected Transform[] _generateTrans = default;

    [Header("Components")]
    [Tooltip("���ꂫ��Controller")]
    [SerializeField]
    DebrisController _debrisCtrl = default;

    [Tooltip("���ꂫ�̉e��Controller")]
    [SerializeField]
    DebrisShadowController _debrisShadowCtrl = default;

    [Header("Debug")]
    [SerializeField]
    bool _debugMode = false;

    [SerializeField]
    float _debugGenerateInterval = 3.0f;
    #endregion

    #region private
    int[] _generateTransIndex;
    Coroutine _generateCoroutine;
    #endregion

    #region public
    #endregion

    #region property
    #endregion

    private void Awake()
    {
        _generateTransIndex = new int[_generateTrans.Length];
    }

    private void Start()
    {
        if (_debugMode)
        {
            StartCoroutine(GenerateCoroutine());
        }
    }
    /// <summary>
    /// ���ꂫ�𐶐�����
    /// </summary>
    /// <param name="generateCount"> �������鐔 </param>
    public void Generate(int generateCount)
    {
        bool isSet;
        int randomIndex;

        ResetGenerateIndex();

        for (int i = 0; i < _generateTransIndex.Length; i++)
        {
            isSet = false;

            //�ԍ����Z�b�g��������܂Ń��[�v
            while (!isSet)
            {
                randomIndex = Random.Range(0, _generateTrans.Length); //�����_���ȗv�f�ԍ����擾

                bool result = _generateTransIndex.Any(i => randomIndex == i); //���ɓ����ԍ����Ȃ����m�F

                //�����ԍ����Ȃ��ꍇ�͍X�V
                if (!result)
                {
                    _generateTransIndex[i] = randomIndex;
                    isSet = true;
                }
            }
        }

        StartCoroutine(GenerateIntervalCoroutine(generateCount));
    }

    /// <summary>
    /// �g�p���̂��ꂫ��S�Ĕ�A�N�e�B�u�ɂ���
    /// </summary>
    public void Return()
    {
        _debrisCtrl.Return();
    }

    /// <summary>
    /// ���ꂫ�𐶐��J�n����
    /// </summary>
    public void StartGenerate()
    {
        _generateCoroutine = StartCoroutine(GenerateCoroutine());
    }

    /// <summary>
    /// ���ꂫ�̐������~����
    /// </summary>
    public void StopGenerate()
    {
        if (_generateCoroutine != null)
        {
            StopCoroutine(_generateCoroutine);
            _generateCoroutine = null;
        }

        _debrisCtrl.Return();
        _debrisShadowCtrl.Return();
    }

    /// <summary>
    /// ��������ʒu��������
    /// </summary>
    private void ResetGenerateIndex()
    {
        for (int i = 0; i < _generateTransIndex.Length; i++)
        {
            _generateTransIndex[i] = -1;
        }
    }

    IEnumerator GenerateCoroutine()
    {
        yield return new WaitForSeconds(0.5f);

        var interval = new WaitForSeconds(_debugGenerateInterval);

        while (true)
        {
            Generate(_generateCount);
            yield return interval;
        }
    }
    IEnumerator GenerateIntervalCoroutine(int generateCount)
    {
        var interval = new WaitForSeconds(_sometimeGenerateInterval);

        for (int i = 0; i < generateCount; i++)
        {
            GameObject shadow = default;
            Vector3 shadowGeneratePoint = new Vector3(_generateTrans[_generateTransIndex[i]].position.x,
                                              0.01f, //�e��\������Y���W�͂��̐��l�łȂ��ƃX�e�[�W�ɖ��܂邽�� 
                                              _generateTrans[_generateTransIndex[i]].position.z);
            
            var point = _generateTrans[_generateTransIndex[i]].position;

            _debrisShadowCtrl.Use(shadowGeneratePoint, s =>
            {
                s.OnAnimation(2.5f, () => 
                {
                    _debrisCtrl.Use(point, d =>
                    {
                        d.OnAnimation(() => 
                        {
                            s.gameObject.SetActive(false);
                        });

                        if (shadow != null)
                        {
                            d.SetShadow(shadow);
                        }
                    });
                });
                shadow = s.gameObject;
            });

            yield return interval;
        }
    }
}
