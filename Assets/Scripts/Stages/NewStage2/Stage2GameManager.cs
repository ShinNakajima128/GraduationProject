using UnityEngine;

public class Stage2GameManager : MonoBehaviour
{
    private enum State
    {
        None,
        ZoomIn,
        DownMugCap,
        ZoomOut,
        shuffle,
        Select
    }

    [SerializeField]
    private Stage2MugcapManager _mugcapManager;

    [SerializeField]
    private Stage2CameraController _cameraController;

    private State _state;

    private void Awake()
    {
        ChengeState(State.None);
    }

    private void Start()
    {
        ChengeState(State.ZoomIn);
    }

    /// <summary>
    /// ステートの変更
    /// </summary>
    private void ChengeState(State next)
    {
        Debug.Log(next.ToString());

        switch (next)
        {
            case State.None:
                break;
            case State.ZoomIn:
                _cameraController.ZoomInRequest(() => ChengeState(State.DownMugCap));
                break;
            case State.DownMugCap:
                _mugcapManager.Setup(() => ChengeState(State.ZoomOut));
                break;
            case State.ZoomOut:
                _cameraController.ZoomOutRequest(() => ChengeState(State.shuffle));
                break;
            case State.shuffle:
                break;
            case State.Select:
                break;
            default:
                break;
        }

        _state = next;
    }
}
