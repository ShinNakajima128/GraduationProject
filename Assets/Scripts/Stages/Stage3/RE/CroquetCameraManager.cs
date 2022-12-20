using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CroquetCameraManager : MonoBehaviour
{
    #region serialize
    [Header("Cameras")]
    [Tooltip("�X�e�[�W3�Ŏg�p����J����")]
    [SerializeField]
    Stage3Camera[] _cameras = default;

    [Tooltip("Cinemachine���Ǘ�����Component")]
    [SerializeField]
    CinemachineBrain _brain = default;
    #endregion

    #region private
    #endregion

    #region public
    #endregion

    #region property
    #endregion

    /// <summary>
    /// �J������؂�ւ���
    /// </summary>
    /// <param name="type"> �؂�ւ���J���� </param>
    /// <param name="blendTime"> �؂�ւ����� </param>
    /// <param name="waitTimeCallback"> �؂�ւ����Ԃ��Ăѐ�ɕԂ�Callback </param>
    public void ChangeCamera(CroquetCameraType type, float blendTime = 1.5f)
    {
        _brain.m_DefaultBlend.m_Time = blendTime;

        //���݃A�N�e�B�u�̃J�������擾���A�D��x��������
        _cameras.FirstOrDefault(c => c.Camera.Priority == 15).Camera.Priority = 10;

        //�w�肵����ނƈ�v�����J�����̗D��x���グ��
        _cameras.FirstOrDefault(c => c.CameraType == type).Camera.Priority = 15;
    }
}

/// <summary>
/// �X�e�[�W3�̃J�����f�[�^
/// </summary>
[Serializable]
public struct Stage3Camera
{
    public string CameraName;
    public CroquetCameraType CameraType;
    public CinemachineVirtualCamera Camera;
}

/// <summary>
/// �X�e�[�W3�̃J�����̎��
/// </summary>
public enum CroquetCameraType
{
    /// <summary>
    /// ��̏�ɂ���J�����B�Q�[���J�n�����_
    /// </summary>
    Start,
    /// <summary> ���� </summary>
    Order,
    /// <summary> �X�e�[�W�S�̂����� </summary>
    View,
    /// <summary> �v���C���[������ </summary>
    InGame,
    /// <summary> �n���l�Y�~��ǂ������� </summary>
    Strike,
    /// <summary> �S�[�����o </summary>
    Goal
}

