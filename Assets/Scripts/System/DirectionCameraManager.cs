using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using DG.Tweening;

/// <summary>
/// カメラ演出を管理するマネージャー
/// </summary>
public class DirectionCameraManager : MonoBehaviour
{
    #region serialize
    [Tooltip("カメラ演出データ")]
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
        //演出カメラのセットアップ
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
    /// カメラ演出のコルーチン
    /// </summary>
    /// <param name="type"> カメラ演出の種類 </param>
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
            print($"現在のカメラの優先度：{priority}");

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
                print("カメラ優先度をリセット");
            }
        }
    }

    /// <summary>
    /// 全てのカメラ演出の優先度をリセットする
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
    /// 指定したカメラ演出の優先度をリセットする
    /// </summary>
    /// <param name="type"> 指定したカメラ演出 </param>
    /// <param name="blendTime"> 元のカメラに戻る時のブレンド時間 </param>
    public void ResetCamera(CameraDirectionType type, float blendTime)
    {
        _brain.m_DefaultBlend.m_Time = blendTime;

        _CameraDirections.FirstOrDefault(d => d.DirectionType == type)
                         .DollyCameras.ToList()
                         .ForEach(c => c.Camera.Priority = 0);
    }

    /// <summary>
    /// カメラのブレンド時間を元に戻す
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
/// カメラ演出をまとめた構造体
/// </summary>
[Serializable]
public class CameraDirection
{
    public string DirectionName;
    public CameraDirectionType DirectionType;
    public DirectionDollyCamera[] DollyCameras; 
}

/// <summary>
/// 演出用のカメラの構造体
/// </summary>
[Serializable]
public struct DirectionDollyCamera
{
    #region public
    public CinemachineVirtualCamera Camera;
    [Tooltip("カメラの表示時間")]
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
            Debug.LogError($"TrackedDollyComponentを取得できませんでした。カメラ名:「{Camera.Name}」");
        }
        //Debug.Log($"{Camera.Name}のPathの数：{PathLength}");
    }
}

/// <summary>
/// カメラ演出の種類
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
    BossStage_End_AliceFront, //アリス正面
    BossStage_End_ZoomAlice, //アリスにズーム
    BossStage_End_FrontQueen, //女王正面
    BossStage_End_ShakeHead, //女王が首を振る
    BossStage_End_AliceOblique, //アリスを斜めから
    BossStage_End_LeftToRightTrump, //トランプ兵を左から右へ映しながら移動
    BossStage_End_RightToLeftTrump, //トランプ兵を右から左へ映しながら移動
    BossStage_End_AliceDiagonallyBack, //アリスの斜め後ろ
    BossStage_End_QueenDepend, //女王に寄る
    BossStage_End_CheshireFront, //チェシャ猫正面
    BossStage_End_AliceFloat1, //アリスが浮き始める
    BossStage_End_AliceFloat2, //アリスがきょろきょろする
    BossStage_End_CheshireOverhead, //チェシャ猫の頭上
    BossStage_End_AliceLookDown, //アリスが見下ろす
    BossStage_End_CheshireLookUp, //チェシャ猫がアリスを見上げる
    BossStage_End_AliceZoomUp, //アリス顔アップ
    BossStage_Phase3_FocusEatMe //EatMeに注目
}

public enum CameraMovementType
{
    None,
    Dolly
}