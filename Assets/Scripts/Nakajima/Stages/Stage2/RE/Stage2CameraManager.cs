using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using DG.Tweening;

public class Stage2CameraManager : MonoBehaviour
{
    #region serialize
    [Header("Variables")]
    [SerializeField]
    float _directionTime = 10;

    [SerializeField]
    int _pathLength = 9;

    [SerializeField]
    Ease _directionCameraEase = default;

    [Header("Cameras")]
    [SerializeField]
    CinemachineVirtualCamera _startDirectionCamera = default;

    [SerializeField]
    CinemachineVirtualCamera _closeupMouseCamera = default;

    [SerializeField]
    CinemachineVirtualCamera _inGameCamera = default;

    [SerializeField]
    CinemachineBrain _brain = default;
    #endregion

    #region private
    CinemachineTrackedDolly _dolly;
    #endregion

    #region public
    #endregion

    #region property
    public static Stage2CameraManager Instance { get; private set; }
    #endregion

    private void Awake()
    {
        _dolly = _startDirectionCamera.GetCinemachineComponent<CinemachineTrackedDolly>();
    }

    public void ChangeCamera(Stage2CameraType type, float blendTime = 2.0f)
    {
        _brain.m_DefaultBlend.m_Time = blendTime;
        switch (type)
        {
            case Stage2CameraType.Main:
                _startDirectionCamera.Priority = 0;
                _closeupMouseCamera.Priority = 0;
                _inGameCamera.Priority = 0;
                break;
            case Stage2CameraType.StartDirection:
                _startDirectionCamera.Priority = 15;
                _closeupMouseCamera.Priority = 0;
                _inGameCamera.Priority = 0;
                break;
            case Stage2CameraType.CloseupMouse:
                _startDirectionCamera.Priority = 0;
                _closeupMouseCamera.Priority = 15;
                _inGameCamera.Priority = 0;
                break;
            case Stage2CameraType.Ingame:
                _startDirectionCamera.Priority = 0;
                _closeupMouseCamera.Priority = 0;
                _inGameCamera.Priority = 15;
                break;
            default:
                break;
        }
    }

    public IEnumerator StartDirectionCoroutine()
    {
        ChangeCamera(Stage2CameraType.StartDirection);

        yield return DOTween.To(() => _dolly.m_PathPosition,
                            x => _dolly.m_PathPosition = x,
                            _pathLength,
                            _directionTime)
                            .SetEase(_directionCameraEase)
                            .WaitForCompletion();
    }
}

public enum Stage2CameraType
{
    Main,
    StartDirection,
    CloseupMouse,
    Ingame
}
