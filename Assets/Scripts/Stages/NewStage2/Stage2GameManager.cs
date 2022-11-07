using UnityEngine;

public class Stage2GameManager : MonoBehaviour
{
    public enum GameState
    {
        None,
        CameraDown,
        Shuffle,
        Select,
    }

    [SerializeField]
    private MugcapManager _mugcapManager;

    [SerializeField]
    private Stage2CameraController _cameraCtrl;

    private GameState _state;

    private void Start()
    {
        ChengeState(GameState.CameraDown);
    }

    private void ChengeState(GameState next)
    {
        switch (next)
        {
            case GameState.None:
                break;
            case GameState.CameraDown:
                _mugcapManager.DownRequest(() =>
                {
                    ChengeState(GameState.Shuffle);
                });
                break;
            case GameState.Shuffle:
                _mugcapManager.Shuffle();
                break;
            case GameState.Select:
                break;
            default:
                break;
        }
        _state = next;
    }
}
