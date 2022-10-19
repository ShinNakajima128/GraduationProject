using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// �X�e�[�W2�̃e�B�[�J�b�v���V���b�t������Controller
/// </summary>
public class ShuffuleController : MonoBehaviour
{
    [Tooltip("�V���b�t�������")]
    [SerializeField]
    int _shuffuleCount = 10;

    [Tooltip("�V���b�t���̏����X�s�[�h")]
    [SerializeField]
    float _initualShuffuleSpeed = 2.0f;

    [Tooltip("�V���b�t�����Ԃ�ω�������l")]
    [SerializeField]
    float _changeshuffuleTimeValue = 2f;

    #region serialize
    [Tooltip("�e�B�[�J�b�v")]
    [SerializeField]
    List<Transform> _teacups = new List<Transform>();

    [Tooltip("�e�B�[�J�b�v���܂Ƃ߂��e�I�u�W�F�N�g��Transform")]
    [SerializeField]
    Transform _teacupsTrans = default;

    [Tooltip("�e�B�[�J�b�v�ɉB���L�����N�^�[")]
    [SerializeField]
    Transform _hidingCharacter = default;

    [Tooltip("�B���ʒu")]
    [SerializeField]
    Transform[] _hidingPositions = default;
    #endregion

    #region private
    float _currentShuffuleSpeed;
    #endregion

    private void Start()
    {
        TeapotGameManager.Instance.GameStart += () => StartCoroutine(StartShuffule());
        TeapotGameManager.Instance.OpenTeacupPhase += OpenTeacup;
        TeapotGameManager.Instance.Initialize += InitializeObject;

        _currentShuffuleSpeed = _initualShuffuleSpeed;
    }

    void Shuffule(float speed)
    {
        bool isSet = false;
        int rand1 = 0;
        int rand2 = 0;

        while (!isSet)
        {
            rand1 = Random.Range(0, _teacups.Count); //�V���b�t������1�ڂ̃^�[�Q�b�g
            rand2 = Random.Range(0, _teacups.Count); //�V���b�t������2�ڂ̃^�[�Q�b�g

            if (rand1 != rand2)�@//�e�l���Ⴄ���̂ɂȂ����烋�[�v���甲����
            {
                isSet = true;
            }
        }
        var pos1 = _teacups[rand1].position;
        var pos2 = _teacups[rand2].position;

        _teacups[rand1].DOMove(pos2, speed);
        _teacups[rand2].DOMove(pos1, speed);
        (_teacups[rand1], _teacups[rand2]) = (_teacups[rand2], _teacups[rand1]); //Tuple�𗘗p���ē���ւ���
    }

    /// <summary>
    /// �V���b�t���J�n
    /// </summary>
    /// <returns></returns>
    IEnumerator StartShuffule()
    {
        int pos = Random.Range(0, _hidingPositions.Length); //�����ʒu�������_���Ō��߂�
        _hidingCharacter.position = _hidingPositions[pos].position;

        var corect = _teacups[pos];

        yield return new WaitForSeconds(1.0f);
        
        _teacupsTrans.DOMoveY(0.5f, 1.0f)
                     .OnComplete(() =>
                     {
                         _hidingCharacter.SetParent(_teacups[pos]);
                     });

        yield return new WaitForSeconds(2.0f);

        //�w�肵���񐔕��V���b�t��
        for (int i = 0; i < _shuffuleCount; i++)
        {
            Shuffule(_currentShuffuleSpeed);

            yield return new WaitForSeconds(_currentShuffuleSpeed);
            

            if (i < _shuffuleCount / 2)
            {
                _currentShuffuleSpeed /= _changeshuffuleTimeValue; //���X�ɑ�������
            }
            else
            {
                _currentShuffuleSpeed *= _changeshuffuleTimeValue; //���X�ɒx������
            }
        }

        //�����̃J�b�v�𒲂ׂ�
        for (int i = 0; i < _teacups.Count; i++)
        {
            if (_teacups[i] == corect)
            {
                TeapotGameManager.Instance.OnSelectTeacup(i); 
                break;
            }
        }
        Debug.Log("�V���b�t������");
    }
    void InitializeObject()
    {
        _teacupsTrans.DOMoveY(1.5f, 0f);

        foreach (var t in _teacups)
        {
            t.DOLocalMoveY(0f, 0f);
        }
    }
    void OpenTeacup(int selectNum, bool result)
    {
        _hidingCharacter.parent = null;
        
        //�͂��ꂽ�ꍇ�͂�����̈ʒu��������
        if (!result)
        {
            _teacups[selectNum].DOMoveY(2f, 1f)
                               .OnComplete(() => 
                               {
                                   for (int i = 0; i < _teacups.Count; i++)
                                   {
                                       if (i == selectNum)
                                       {
                                           continue;
                                       }
                                       _teacups[i].DOMoveY(2f, 0.5f);
                                   }
                               });
        }
        else
        {
            _teacups[selectNum].DOMoveY(2f, 1f);
        }
    }
}
