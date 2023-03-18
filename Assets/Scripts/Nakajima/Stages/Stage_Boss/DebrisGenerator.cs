using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���ꂫ�𐶐��A��������@�\������Component
/// </summary>
public class DebrisGenerator : MonoBehaviour
{
    #region serialize
    [Header("Variables")]
    [Tooltip("���ꂫ�𐶐����鐔")]
    [SerializeField]
    int _generateCount = 3;

    [Tooltip("�t�F�[�Y2�̐����͈͂̍��W")]
    [SerializeField]
    Transform[] _generateRangeTrans = default;

    [Tooltip("�����^�C�~���O�Ő����������ꂫ�̐����Ԋu")]
    [SerializeField]
    float _sometimeGenerateInterval = 0.2f;

    [Tooltip("�񕜃A�C�e������������m��")]
    [SerializeField, Range(0f, 1f)]
    float _itemGeneratePercent = 0.3f;

    [SerializeField]
    float _nextGenerateInterval = 5.0f;

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

    [Tooltip("�񕜃A�C�e����Generator")]
    [SerializeField]
    ItemGenerator _itemGenerator = default;

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
    public bool IsGenerating { get; private set; } = false;
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
    /// ���ꂫ��ݒ肳�ꂽ�u�͈́v�Ƀ����_���Ő�������
    /// </summary>
    /// <param name="generateCount"> �������鐔 </param>
    public void RandomGenerate(int generateCount)
    {
        StartCoroutine(RandomIntervalCoroutine(generateCount));
    }

    /// <summary>
    /// ���ꂫ��ݒ肳�ꂽ�u���W�v�Ƀ����_���Ő�������
    /// </summary>
    /// <param name="generateCount"> �������鐔 </param>
    public void DefiniteGenerate(int generateCount)
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
        if (_generateCoroutine != null)
        {
            StopCoroutine(_generateCoroutine);
            _generateCoroutine = null;
        }
        _debrisCtrl.Return();
    }

    /// <summary>
    /// ���ꂫ�𐶐��J�n����
    /// </summary>
    public void StartGenerate(GenerateType type)
    {
        switch (type)
        {
            case GenerateType.Random:
                _generateCoroutine = StartCoroutine(RandomGenerateCoroutine());
                break;
            case GenerateType.Definite:
                _generateCoroutine = StartCoroutine(GenerateCoroutine());
                break;
            default:
                break;
        }
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

        IsGenerating = false;
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

    IEnumerator RandomGenerateCoroutine()
    {
        yield return new WaitForSeconds(0.5f);

        WaitForSeconds interval;

        if (_debugMode)
        {
            interval = new WaitForSeconds(_debugGenerateInterval);
        }
        else
        {
            interval = new WaitForSeconds(_nextGenerateInterval);
        }

        while (true)
        {
            RandomGenerate(_generateCount);
            yield return interval;
        }
    }

    IEnumerator GenerateCoroutine()
    {
        yield return new WaitForSeconds(0.5f);

        WaitForSeconds interval;

        if (_debugMode)
        {
            interval = new WaitForSeconds(_debugGenerateInterval);
        }
        else
        {
            interval = new WaitForSeconds(_nextGenerateInterval); 
        }

        while (true)
        {
            DefiniteGenerate(_generateCount);
            yield return interval;
        }
    }

    IEnumerator RandomIntervalCoroutine(int generateCount)
    {
        var interval = new WaitForSeconds(_sometimeGenerateInterval);

        IsGenerating = true;

        for (int i = 0; i < generateCount; i++)
        {
            GameObject shadow = default;

            float randomX = Random.Range(_generateRangeTrans[0].position.x, _generateRangeTrans[1].position.x);
            float randomZ = Random.Range(_generateRangeTrans[1].position.z, _generateRangeTrans[0].position.z);

            Vector3 randomPoint = new Vector3(randomX, 10f, randomZ);

            Vector3 shadowGeneratePoint = new Vector3(randomPoint.x,
                                              0.01f, //�e��\������Y���W�͂��̐��l�łȂ��ƃX�e�[�W�ɖ��܂邽�� 
                                              randomPoint.z);

            _debrisShadowCtrl.Use(shadowGeneratePoint, s =>
            {
                s.OnAnimation(2.5f, () =>
                {
                    _debrisCtrl.Use(randomPoint, d =>
                    {
                        d.OnAnimation(() =>
                        {
                            s.gameObject.SetActive(false);
                        });

                        if (shadow != null)
                        {
                            d.SetShadow(shadow);
                        }

                        int percent = Random.Range(0, 10);

                        //�w�肵���A�C�e�������m���𒴂�����A�C�e���𔭐�������
                        if (_itemGeneratePercent >= percent / 10.0f)
                        {
                            d.IsItemGenerate = true;
                        }
                    });
                });
                shadow = s.gameObject;
            });

            yield return interval;
        }
    }

    IEnumerator GenerateIntervalCoroutine(int generateCount)
    {
        var interval = new WaitForSeconds(_sometimeGenerateInterval);

        IsGenerating = true;

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

                        int percent = Random.Range(0, 10);

                        //�w�肵���A�C�e�������m���𒴂�����A�C�e���𔭐�������
                        if (_itemGeneratePercent >= percent / 10.0f)
                        {
                            d.IsItemGenerate = true;
                        }
                    });
                });
                shadow = s.gameObject;
            });

            yield return interval;
        }
    }
}

/// <summary>
/// �����̎��
/// </summary>
public enum GenerateType
{
    Random, //�w�肵���͈͂������_��
    Definite //��ʒu
}
