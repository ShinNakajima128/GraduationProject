using UnityEngine;

public class Stage3GameManager : MonoBehaviour
{
    #region Define
    public enum GameState
    {
        None,
        Order,
        MoveCamera,
        Throw,
        Result
    }
    #endregion

    [SerializeField]
    private CameraController _cameraCtrl;

    [SerializeField]
    private OrderManager _order;

    [SerializeField]
    private Stage3PlayerController _player;

    [SerializeField]
    private Stage3ScoreConter _scoreConter;

    private GameState _state = GameState.None;

    private void Start()
    {
        ChengeStage(GameState.Order);
    }

    private void ChengeStage(GameState next)
    {
        switch (next)
        {
            case GameState.None:
                break;
            case GameState.Order:
                _order.CreateOrder(() => ChengeStage(GameState.MoveCamera));
                break;
            case GameState.MoveCamera:
                _cameraCtrl.MoveRequest(() => ChengeStage(GameState.Throw));
                break;
            case GameState.Throw:
                _player.Send();
                break;
            case GameState.Result:
                break;
            default:
                break;
        }

        _state = next;
    }
}