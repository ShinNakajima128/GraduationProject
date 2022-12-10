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
    private OrderManager _orderMana;

    [SerializeField]
    private Stage3UIManager _uiManager;

    [SerializeField]
    private Stage3PlayerController _player;

    [SerializeField]
    private BallController _ball;

    [SerializeField]
    private Stage3ScoreConter _scoreConter;

    [Header("暗転にかける時間")]
    [SerializeField]
    private float _blackOutDuration;

    [SerializeField]
    private int _needClearCount;

    private GameState _state = GameState.None;

    /// <summary>
    /// ゲームクリアの回数
    /// </summary>
    private int _currentClearCount;

    private void Start()
    {
        ChengeStage(GameState.CreateOrder);
        // 〇ボタンに処理の登録
        _player.RegistToOnCircleButton(() => ChengeStage(GameState.MoveCamera));
        // ゴール時の挙動の追加
        _ball.AddCallBack(() => ChengeStage(GameState.Result));
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
                var order = _orderMana.CreateOrder();
                // お題のUI表示
                _uiManager.DisplayOrder(order);
                break;
            case GameState.MoveCamera:
                // Orderの非表示
                _uiManager.ChengeUIActivete(Stage3UIManager.Type.OrderUI, false);
                // カメラの移動　移動後ステートの変更
                _cameraCtrl.MoveRequest(() => ChengeStage(GameState.Throw));
                // 暗転処理
                _uiManager.BeginBlackOut(_blackOutDuration);
                break;
            case GameState.Throw:
                // InGameUIの表示
                _uiManager.ChengeUIActivete(Stage3UIManager.Type.IngameUI, true);
                // 暗転パネルの非表示
                _uiManager.ChengeUIActivete(Stage3UIManager.Type.BlackOutImage, false);
                // 操作を開始
                _player.BeginControl();
                break;
            case GameState.Result:
                if (_orderMana.IsCameClear())
                {
                    _currentClearCount++;
                    CheckGameClear();
                }
                else
                {
                    // GameManager.SaveStageResult(false);
                }
                break;
            default:
                break;
        }

        _state = next;
    }

    /// <summary>
    /// ステージのクリアをしたか
    /// </summary>
    private void CheckGameClear()
    {
        if (_currentClearCount > _needClearCount)
        {
            _uiManager.ChengeUIActivete(Stage3UIManager.Type.GameClear, true);
            GameManager.SaveStageResult(true);
            TransitionManager.SceneTransition(SceneType.Lobby);
        }
        else
        {
        }
    }
}