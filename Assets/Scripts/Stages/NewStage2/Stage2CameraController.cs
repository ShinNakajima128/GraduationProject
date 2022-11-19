using DG.Tweening;
using System;
using UnityEngine;

/// <summary>
/// カメラコントローラー
/// </summary>
public class Stage2CameraController : MonoBehaviour
{
    [SerializeField]
    private Camera _camera;

    [SerializeField]
    private float _duration;

    [SerializeField]
    private Transform _zoomOutTrans;

    [SerializeField]
    private Transform _zoomInTrans;

    public enum ZoomType
    {
        In,
        Out
    }

    public void ZoomRequest(ZoomType type, Action action = null)
    {
        switch (type)
        {
            case ZoomType.In:
                _camera.transform.DOLocalMove(_zoomInTrans.localPosition, _duration).SetEase(Ease.Linear).OnComplete(() => action());
                break;
            case ZoomType.Out:
                _camera.transform.DOLocalMove(_zoomOutTrans.localPosition, _duration).SetEase(Ease.Linear).OnComplete(() => action());
                break;
            default:
                break;
        }
    }
}
