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
    [Tooltip("ステージ3で使用するカメラ")]
    [SerializeField]
    Stage3Camera[] _cameras = default;

    [Tooltip("Cinemachineを管理するComponent")]
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
    /// カメラを切り替える
    /// </summary>
    /// <param name="type"> 切り替えるカメラ </param>
    /// <param name="blendTime"> 切り替え時間 </param>
    /// <param name="waitTimeCallback"> 切り替え時間を呼び先に返すCallback </param>
    public void ChangeCamera(CroquetCameraType type, float blendTime = 1.5f)
    {
        _brain.m_DefaultBlend.m_Time = blendTime;

        //現在アクティブのカメラを取得し、優先度を下げる
        _cameras.FirstOrDefault(c => c.Camera.Priority == 15).Camera.Priority = 10;

        //指定した種類と一致したカメラの優先度を上げる
        _cameras.FirstOrDefault(c => c.CameraType == type).Camera.Priority = 15;
    }
}

/// <summary>
/// ステージ3のカメラデータ
/// </summary>
[Serializable]
public struct Stage3Camera
{
    public string CameraName;
    public CroquetCameraType CameraType;
    public CinemachineVirtualCamera Camera;
}

/// <summary>
/// ステージ3のカメラの種類
/// </summary>
public enum CroquetCameraType
{
    /// <summary>
    /// 城の上にあるカメラ。ゲーム開始時視点
    /// </summary>
    Start,
    /// <summary> お題 </summary>
    Order,
    /// <summary> ステージ全体を見る </summary>
    View,
    /// <summary> プレイヤー操作画面 </summary>
    InGame,
    /// <summary> ハリネズミを追いかける </summary>
    Strike,
    /// <summary> ゴール演出 </summary>
    Goal
}

