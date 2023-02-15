using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TeacupShuffler : MonoBehaviour
{
    #region serialize
    [Header("Objects")]
    [Tooltip("�e�e�B�[�|�b�g��Transform")]
    [SerializeField]
    Transform[] _teapots = default;

    [Tooltip("�e�B�[�|�b�g�̃A�j���[�V����Path")]
    [SerializeField]
    ShufflePath[] _shufflePaths = default;

    [Header("Debug")]
    [SerializeField]
    bool _debugMode = false;
    #endregion
    #region private
    #endregion
    #region public
    #endregion
    #region property
    #endregion

    //����
    //�V���b�t������֐��Ɏw�肷�鐔����0�`5�܂�

    IEnumerator Start()
    {
        if (_debugMode)
        {
            while (true)
            {
                int rand = UnityEngine.Random.Range(0, 6);

                yield return ShuffleTeacup(null, rand);
            }
        }
    }

    /// <summary>
    /// �e�B�[�J�b�v���V���b�t������
    /// </summary>
    /// <param name="index"> �V���b�t������J�b�v�̔ԍ� </param>
    public IEnumerator ShuffleTeacup(Teacup[] cups,int index, float shuffleTime = 1.5f)
    {
        var mainPositionType = (TeapotPositionType)index;  //���C����Index���ʒu�̎�ނ֕ϊ�
        var mainRandomTarget = (ReplaceTargetType)UnityEngine.Random.Range(0, 2); //������̃J�b�v��O�ɂ��邩���ɂ��邩�����_���Ō��߂�

        //���C������^�[�Q�b�g�ւ�Path���擾
        var mainToTargetPath = _shufflePaths.Where(t => t.PositionType == mainPositionType)
                                .FirstOrDefault(p => p.TargetType == mainRandomTarget)
                                .PathTrans.Select(x => x.transform.position).ToArray();

        int targetIndex = default;
        ReplaceTargetType targetToMainType = default;
        Vector3[] targetToMainPath = default;

        //���C���̌����悪�uNext�v�̏ꍇ��1���TeaCup�́uPrev�v��Path���擾
        if (mainRandomTarget == ReplaceTargetType.Next)
        {
            //�Ō�̗v�f�����C���������ꍇ�́u0�v�ɖ߂�
            if (index == 5)
            {
                targetIndex = 0;
            }
            else
            {
                targetIndex = index + 1;
            }
            targetToMainType = ReplaceTargetType.Previous;
        }
        else
        {
            if (index == 0)
            {
                targetIndex = 5;
            }
            else
            {
                targetIndex = index - 1;
            }
            targetToMainType = ReplaceTargetType.Next;
        }

        //�^�[�Q�b�g���烁�C���ւ�Path���擾
        targetToMainPath = _shufflePaths.Where(t => t.PositionType == (TeapotPositionType)targetIndex)
                                .FirstOrDefault(p => p.TargetType == targetToMainType)
                                .PathTrans.Select(x => x.transform.position).ToArray();

        AudioManager.PlaySE(SEType.Stage2_Shuffle);
        //VibrationController.OnVibration(Strength.Low, 0.2f);

        //�Q�̃J�b�v�����ւ���A�j���[�V�������Đ��B�����̓J�b�v�̃A�j���[�V�������I���܂őҋ@
        _teapots[index].DOPath(mainToTargetPath, shuffleTime, PathType.CatmullRom);
        yield return _teapots[targetIndex].DOPath(targetToMainPath, shuffleTime, PathType.CatmullRom)
                                          .WaitForCompletion();

        //�^�v���Ō��������J�b�v�̔z��ʒu�����ւ���
        (_teapots[index], _teapots[targetIndex]) = (_teapots[targetIndex], _teapots[index]);

        if (!_debugMode && cups != null)
        {
            (cups[index], cups[targetIndex]) = (cups[targetIndex], cups[index]);
        }

        yield return null;
    }
    public IEnumerator WarpTeacup(Teacup[] cups, int index, float warpTime = 0.5f)
    {
        var randomTarget = index;

        //���[�v��̃^�[�Q�b�g�����܂�܂Ń��[�v
        while (index == randomTarget)
        {
            randomTarget = UnityEngine.Random.Range(0, 6);
        }

        EffectManager.PlayEffect(EffectType.Stage2_WarpStart, _teapots[index].transform.position);
        EffectManager.PlayEffect(EffectType.Stage2_WarpStart, _teapots[randomTarget].transform.position);
        AudioManager.PlaySE(SEType.Stage2_Warp);
        VibrationController.OnVibration(Strength.Middle, 0.3f);

        yield return new WaitForSeconds(0.4f);

        _teapots[index].gameObject.SetActive(false);
        _teapots[randomTarget].gameObject.SetActive(false);

        yield return new WaitForSeconds(warpTime);

        EffectManager.PlayEffect(EffectType.Stage2_WarpEnd, _teapots[index].transform.position);
        EffectManager.PlayEffect(EffectType.Stage2_WarpEnd, _teapots[randomTarget].transform.position);
        AudioManager.PlaySE(SEType.Stage2_Warp);
        VibrationController.OnVibration(Strength.Middle, 0.3f);

        yield return new WaitForSeconds(0.4f);

        _teapots[index].gameObject.SetActive(true);
        _teapots[randomTarget].gameObject.SetActive(true);

        var mainPos = _teapots[index].transform.position;
        var targetPos = _teapots[randomTarget].transform.position;

        _teapots[index].DOMove(targetPos, 0f);
        _teapots[randomTarget].DOMove(mainPos, 0f);

        //�^�v���Ō��������J�b�v�̔z��ʒu�����ւ���
        (_teapots[index], _teapots[randomTarget]) = (_teapots[randomTarget], _teapots[index]);

        if (!_debugMode && cups != null)
        {
            (cups[index], cups[randomTarget]) = (cups[randomTarget], cups[index]);
        }

        yield return new WaitForSeconds(warpTime);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        //�������d���Ȃ邽�߁A�f�o�b�O���[�h���ȊO�͕`�ʂ��Ȃ�
        if (!_debugMode)
        {
            return;
        }

        Gizmos.color = Color.red;

        for (int i = 0; i < _shufflePaths.Length; i++)
        {
            for (int n = 0; n < _shufflePaths[i].PathTrans.Length - 1; n++)
            {
                Gizmos.DrawLine(_shufflePaths[i].PathTrans[n].position, _shufflePaths[i].PathTrans[n + 1].position);
            }
        }
    }
#endif
}

/// <summary>
/// �e�B�[�J�b�v�̈ʒu�̎��
/// </summary>
public enum TeapotPositionType
{
    UpperRight,
    Right,
    LowerRight,
    LowerLeft,
    Left,
    UpperLeft
}

/// <summary>
/// �^�[�Q�b�g���ǂ���ɂ��邩�̎��
/// </summary>
public enum ReplaceTargetType
{
    Previous, //�O
    Next      //��
}

/// <summary>
/// �V���b�t�����Ɉ���Path�f�[�^
/// </summary>
[Serializable]
public struct ShufflePath
{
    public string PathName;
    public TeapotPositionType PositionType;
    public ReplaceTargetType TargetType;
    public Transform[] PathTrans;
}