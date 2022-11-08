using DG.Tweening;
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
    private Transform _target;

    public void MoveRequest()
    {
        transform.DOLocalMove(_target.localPosition, _duration);
    }
}
