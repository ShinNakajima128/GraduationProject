using UnityEngine;

public class Stage3GameManager : MonoBehaviour
{
    #region Define
    public enum GameState
    {
        None,
        CreateOrder,
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
    private Stage3UIManager _uiManager;

    [SerializeField]
    private Stage3PlayerController _player;

    [SerializeField]
    private Stage3ScoreConter _scoreConter;

    [Header("暗転にかける時間")]
    [SerializeField]
    private float _durationOfBlackOut;

    private GameState _state = GameState.None;

    private void Start()
    {
        ChengeStage(GameState.CreateOrder);
        // 〇ボタンに処理の登録
        _player.RegistToOnCircleButton(() => ChengeStage(GameState.MoveCamera));
    }

    /// <summary>
    /// ステートの変更
    /// </summary>
    private void ChengeStage(GameState next)
    {
        Debug.Log($"ステート変更 {_state} -> {next}");

        switch (next)
        {
            case GameState.None:
                break;
            case GameState.CreateOrder:
                // オーダーの作成
                var order = _order.CreateOrder(() => ChengeStage(GameState.MoveCamera));
                _uiManager.DisplayOrder(order);
                break;
            case GameState.MoveCamera:
                // Orderの非表示
                _uiManager.UIDisable(Stage3UIManager.Type.OrderUI);
                // カメラの移動
                _cameraCtrl.MoveRequest(() => ChengeStage(GameState.Throw));
                // 暗転の処理
                _uiManager.BeginBlackOut(_durationOfBlackOut);
                break;
            case GameState.Throw:
                // 暗転パネルの非表示
                _uiManager.UIDisable(Stage3UIManager.Type.BlackOutImage);
                _player.StartControl();
                break;
            case GameState.Result:
                break;
            default:
                break;
        }

        _state = next;
    }
}