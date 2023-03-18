using DG.Tweening;
using System;
using UnityEngine;

/// <summary>
/// カメラコントローラー
/// </summary>
public class Stage2CameraController : MonoBehaviour
{
    public enum ZoomType
    {
        In,
        Out
    }


    [SerializeField]
    private Camera _camera;

    [SerializeField]
    private float _duration;

    [SerializeField]
    private Transform _allViewTran;

    [SerializeField]
    private Transform _zoomOutTrans;

    [SerializeField]
    private Transform _zoomInTrans;

    // カップに寄る座標     
    [SerializeField]
    private Transform[] _zoonPositions;

    public bool IsZoomed { get; set; } = false;

    public void SelectZoom(int index, float duration)
    {
        Debug.Log($"カメラを{index}に移動");
        IsZoomed = true;
        _camera.transform.DOMove(_zoonPositions[index].position, duration);
    }

    public void ZoomRequest(ZoomType type, Action action = null)
    {
        switch (type)
        {
            case ZoomType.In:
                IsZoomed = true;
                _camera.transform.DOLocalMove(_zoomInTrans.localPosition, _duration).SetEase(Ease.Linear).OnComplete(() => action());
                break;
            case ZoomType.Out:
                IsZoomed = false;
                _camera.transform.DOLocalMove(_zoomOutTrans.localPosition, _duration).SetEase(Ease.Linear).OnComplete(() => action());
                break;
            default:
                break;
        }
    }
}
