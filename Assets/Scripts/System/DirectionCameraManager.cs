using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using DG.Tweening;

/// <summary>
/// �J�������o���Ǘ�����}�l�[�W���[
/// </summary>
public class DirectionCameraManager : MonoBehaviour
{
    #region serialize
    [Tooltip("�J�������o�f�[�^")]
    [SerializeField]
    CameraDirection[] _CameraDirections = default;

    [Header("Debug")]
    [SerializeField]
    bool _DebugMode = false;

    [SerializeField]
    CameraDirectionType _debugType = default;
    #endregion

    #region private
    CinemachineBrain _brain;
    float _originBlendTime;
    #endregion

    #region public
    #endregion

    #region property
    #endregion

    private void Awake()
    {
        //���o�J�����̃Z�b�g�A�b�v
        foreach (var direction in _CameraDirections)
        {
            foreach (var camera in direction.DollyCameras)
            {
                camera.Setup();
            }
        }
        _brain = Camera.main.GetComponent<CinemachineBrain>();
        _originBlendTime = _brain.m_DefaultBlend.m_Time;
    }

    private void Start()
    {
        if (_DebugMode)
        {
            StartCoroutine(StartDirectionCoroutine(_debugType));
        }
    }

    /// <summary>
    /// �J�������o�̃R���[�`��
    /// </summary>
    /// <param name="type"> �J�������o�̎�� </param>
    /// <returns></returns>
    public IEnumerator StartDirectionCoroutine(CameraDirectionType type, int priority = 30)
    {
        yield return null;

        var direction = _CameraDirections.FirstOrDefault(d => d.DirectionType == type);

        print($"{direction.DirectionName}");

        for (int i = 0; i < direction.DollyCameras.Length; i++)
        {
            _brain.m_DefaultBlend.m_Time = direction.DollyCameras[i].NextCameraBlendTime;

            if (direction.DollyCameras[i].Dolly == null)
            {
                direction.DollyCameras[i].Setup();
            }
            direction.DollyCameras[i].Camera.Priority = priority;
            print($"���݂̃J�����̗D��x�F{priority}");

            if (direction.DollyCameras[i].MovementType == CameraMovementType.Dolly)
            {
                Debug.Log("OnDolly");
                yield return DOTween.To(() =>
                                    direction.DollyCameras[i].Dolly.m_PathPosition,
                                    x => direction.DollyCameras[i].Dolly.m_PathPosition = x,
                                    direction.DollyCameras[i].PathLength - 1,
                                    direction.DollyCameras[i].ViewDuration)
                                    .SetEase(direction.DollyCameras[i].CameraEaseType)
                                    .WaitForCompletion();
            }
            else
            {
                yield return new WaitForSeconds(direction.DollyCameras[i].ViewDuration);
            }

            if (i != 0 && i < direction.DollyCameras.Length - 1)
            {
                direction.DollyCameras[i].Camera.Priority = 0;
                print("�J�����D��x�����Z�b�g");
            }
        }
    }

    /// <summary>
    /// �S�ẴJ�������o�̗D��x�����Z�b�g����
    /// </summary>
    public void ResetCamera()
    {
        foreach (var d in _CameraDirections)
        {
            foreach (var c in d.DollyCameras)
            {
                c.Camera.Priority = 0;
            }
        }
    }

    /// <summary>
    /// �w�肵���J�������o�̗D��x�����Z�b�g����
    /// </summary>
    /// <param name="type"> �w�肵���J�������o </param>
    /// <param name="blendTime"> ���̃J�����ɖ߂鎞�̃u�����h���� </param>
    public void ResetCamera(CameraDirectionType type, float blendTime)
    {
        _brain.m_DefaultBlend.m_Time = blendTime;

        _CameraDirections.FirstOrDefault(d => d.DirectionType == type)
                         .DollyCameras.ToList()
                         .ForEach(c => c.Camera.Priority = 0);
    }

    /// <summary>
    /// �J�����̃u�����h���Ԃ����ɖ߂�
    /// </summary>
    public void ResetBlendTime()
    {
        _brain.m_DefaultBlend.m_Time = _originBlendTime;
    }

    public void SetBlendTime(float blendTime)
    {
        _brain.m_DefaultBlend.m_Time = blendTime;
    }
}

/// <summary>
/// �J�������o���܂Ƃ߂��\����
/// </summary>
[Serializable]
public class CameraDirection
{
    public string DirectionName;
    public CameraDirectionType DirectionType;
    public DirectionDollyCamera[] DollyCameras; 
}

/// <summary>
/// ���o�p�̃J�����̍\����
/// </summary>
[Serializable]
public struct DirectionDollyCamera
{
    #region public
    public CinemachineVirtualCamera Camera;
    [Tooltip("�J�����̕\������")]
    public float ViewDuration;
    public Ease CameraEaseType;
    public CameraMovementType MovementType;
    public float NextCameraBlendTime;
    #endregion

    #region private
    private CinemachineTrackedDolly _dolly;
    private CinemachineSmoothPath _SmoothPath;
    #endregion

    #region property
    public CinemachineTrackedDolly Dolly => _dolly;
    public int PathLength => _SmoothPath.m_Waypoints.Length;

    #endregion

    public void Setup()
    {
        try
        {
            if (MovementType != CameraMovementType.Dolly)
            {
                return;
            }
            _dolly = Camera.GetCinemachineComponent<CinemachineTrackedDolly>();
            if (_dolly != null)
            {
                _SmoothPath = _dolly.m_Path.GetComponent<CinemachineSmoothPath>();
            }
        }
        catch
        {
            Debug.LogError($"TrackedDollyComponent���擾�ł��܂���ł����B�J������:�u{Camera.Name}�v");
        }
        //Debug.Log($"{Camera.Name}��Path�̐��F{PathLength}");
    }
}

/// <summary>
/// �J�������o�̎��
/// </summary>
public enum CameraDirectionType
{
    Lobby_FirstVisit,
    Lobby_Introduction,
    Lobby_Alice_Front,
    Lobby_AliceAndCheshireTalking,
    BossStage_FrontAlice,
    BossStage_HeadingBossFeet,
    BossStage_SlowlyRise,
    BossStage_OnBossFace,
    BossStage_BehindBoss,
    BossStage_BehindAlice,
    BossStage_FrontBoss,
    BossStage_ZoomBossFace,
    BossStage_FrontCheshireCat,
    BossStage_End_Start,
    BossStage_End_OnSideQueen,
    BossStage_End_GoAroundFrontQueen,
    Bossstage_End_OnFace,
    BossStage_End_AliceFront, //�A���X����
    BossStage_End_ZoomAlice, //�A���X�ɃY�[��
    BossStage_End_FrontQueen, //��������
    BossStage_End_ShakeHead, //���������U��
    BossStage_End_AliceOblique, //�A���X���΂߂���
    BossStage_End_LeftToRightTrump, //�g�����v����������E�։f���Ȃ���ړ�
    BossStage_End_RightToLeftTrump, //�g�����v�����E���獶�։f���Ȃ���ړ�
    BossStage_End_AliceDiagonallyBack, //�A���X�̎΂ߌ��
    BossStage_End_QueenDepend, //�����Ɋ��
    BossStage_End_CheshireFront, //�`�F�V���L����
    BossStage_End_AliceFloat1, //�A���X�������n�߂�
    BossStage_End_AliceFloat2, //�A���X������낫��낷��
    BossStage_End_CheshireOverhead, //�`�F�V���L�̓���
    BossStage_End_AliceLookDown, //�A���X�������낷
    BossStage_End_CheshireLookUp, //�`�F�V���L���A���X�����グ��
    BossStage_End_AliceZoomUp, //�A���X��A�b�v
    BossStage_Phase3_FocusEatMe //EatMe�ɒ���
}

public enum CameraMovementType
{
    None,
    Dolly
}