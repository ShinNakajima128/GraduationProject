using UnityEngine;
using UnityEngine.InputSystem;

public class Stage2GameManager : MonoBehaviour
{
    #region Define
    private enum GameState
    {
        None,
        AllView,
        ZoomIn,
        DownMugCap,
        ZoomOut,
        Shuffle,
        Select,
        Wait,
        GameEnd
    }
    #endregion

    [SerializeField]
    private Stage2CameraController _camera;

    [SerializeField]
    private Stage2MugcupManager _mugcupManager;

    [SerializeField]
    private Stage2UIController _uiCtrl;

    [SerializeField]
    private Stage2Selector _selector;

    [SerializeField]
    private DorMouse _mouse;

    [SerializeField]
    private PlayerInput _input;

    [SerializeField]
    private Stages _stages;

    private GameState _state;
    private ShuffleFase _currentShuffleFase;

    private bool IsClear { get; set; } = false;

    private void Start()
    {
        GameManager.UpdateCurrentStage(_stages);
        _input.actions["Enter"].started += OnEnter;
        ChengeState(GameState.ZoomIn);
        _currentShuffleFase = ShuffleFase.One;
    }

    // 〇ボタンor左クリックが押された時
    private void OnEnter(InputAction.CallbackContext context)
    {
        if (context.started)
            switch (_state)
            {
                case GameState.AllView:
                    _camera.ZoomRequest(Stage2CameraController.ZoomType.Out, () => ChengeState(GameState.ZoomIn));
                    break;
                case GameState.ZoomIn:
                    break;
                case GameState.DownMugCap:
                    break;
                case GameState.ZoomOut:
                    break;
                case GameState.Shuffle:
                    break;
                case GameState.Select:
                    break;
                case GameState.Wait:
                    if (_camera.IsZoomed is true)
                    {
                        _camera.ZoomRequest(Stage2CameraController.ZoomType.Out, () => ChengeState(GameState.Shuffle));
                    }
                    else
                    {
                        ChengeState(GameState.Shuffle);
                    }
                    break;
                case GameState.GameEnd:
                    if (IsClear)
                    {
                        // ロビーに移動
                        GameManager.SaveStageResult(true);
                        TransitionManager.SceneTransition(SceneType.Lobby);
                    }
                    break;
                default:
                    break;
            }
    }

    /// <summary>
    /// ステートの変更
    /// </summary>
    private void ChengeState(GameState next)
    {
        Debug.Log($"ステートを{next}に変更");

        switch (next)
        {
            case GameState.None:
                break;
            case GameState.ZoomIn:
                _camera.ZoomRequest(Stage2CameraController.ZoomType.In, () => ChengeState(GameState.DownMugCap));
                break;
            case GameState.DownMugCap:
                _mugcupManager.Initialise(() => ChengeState(GameState.ZoomOut));
                // アニメーショん
                _mouse.OnAnimation(MouseState.CloseEar, 2f);
                break;
            case GameState.ZoomOut:
                _camera.ZoomRequest(Stage2CameraController.ZoomType.Out, () => ChengeState(GameState.Wait));
                break;
            case GameState.Wait:
                // シャッフルスタート画面の表示
                _uiCtrl.ChengeActive(Stage2UIController.UIType.Play, true);
                // ズームしていたら、ズームアウトしてからフェーズを変える
                if (_camera.IsZoomed is true)
                {
                    _camera.ZoomRequest(Stage2CameraController.ZoomType.Out, () => ChengeState(GameState.Wait));
                }
                break;
            case GameState.Shuffle:
                // スタートパネルの非表示
                _uiCtrl.ChengeActive(Stage2UIController.UIType.Play, false);
                // UIの非表示
                _uiCtrl.SetDisable();
                // 選択機能の停止
                _selector.Stop();
                // シャッフルに移行の処理
                _mugcupManager.BeginShuffle(_currentShuffleFase, () =>
                {
                    ChengeState(GameState.Select);
                });
                break;
            case GameState.Select:
                _selector.Begin();
                break;
            case GameState.GameEnd:
                break;
            default:
                break;
        }

        _state = next;
    }

    /// <summary>
    /// 判定をし、ステートを切り替える
    /// </summary>
    public void Judge(int selectedNumber)
    {
        if (_state != GameState.Select) return;

        // マウスが入っているIndexを取得
        var num = _mugcupManager.GetInMouseCupNumber();

        // 選択アイコンの削除
        _selector.Stop();

        // 正解時
        if (num == selectedNumber)
        {
            Clear(selectedNumber, num);
        }
        // 不正解
        else
        {
            Miss();
        }
    }

    /// <summary>
    /// 正解時の処理
    /// </summary>
    private void Clear(int selectedNumber, int num)
    {
        Debug.Log("正解");
        _uiCtrl.ChengeActive(Stage2UIController.UIType.Clear, true);
        // マウスを起き上がる
        _mouse.OnAnimation(MouseState.WakeUp, 0.5f);

        if (_currentShuffleFase != ShuffleFase.Three)
        {
            // フェーズの切り替え
            _currentShuffleFase++;
            // 選択したカップをあげる
            _mugcupManager.OpenRequest(selectedNumber, () =>
            {
                // アニメーション
                _mouse.OnAnimation(MouseState.CloseEar, 0.5f);
                // UIの非表示
                _uiCtrl.ChengeActive(Stage2UIController.UIType.Clear, false);
                // 下げた後、ステートを切り替える
                _mugcupManager.CloseRequest(selectedNumber, () =>
                {
                    // ズームしていたら、ズームアウトしてからフェーズを変える
                    if (_camera.IsZoomed is true)
                    {
                        _camera.ZoomRequest(Stage2CameraController.ZoomType.Out, () => 
                        {
                            IsClear = true;
                            ChengeState(GameState.Wait);
                        });
                    }
                });
            });
        }
        else if (_currentShuffleFase == ShuffleFase.Three)
        {
            _mugcupManager.OpenRequest(num);
            // UIの表示
            _uiCtrl.ChengeActive(Stage2UIController.UIType.GameClear, true);

            // ズームしていたら、ズームアウトしてからフェーズを変える
            if (_camera.IsZoomed is true)
            {
                _camera.ZoomRequest(Stage2CameraController.ZoomType.Out, () => ChengeState(GameState.GameEnd));
            }
        }
    }

    /// <summary>
    /// 間違えた時
    /// </summary>
    private void Miss()
    {
        Debug.Log("失敗");

        _uiCtrl.ChengeActive(Stage2UIController.UIType.Miss, true);
        // ネズミが起き上がる
        _mouse.OnAnimation(MouseState.OpenEar, 1f);
        // 全てあげる
        _mugcupManager.OpenAllMugCup(() =>
        {
            // 寝る
            _mouse.OnAnimation(MouseState.CloseEar, 1f);

            // UIの表示
            _uiCtrl.ChengeActive(Stage2UIController.UIType.Miss, false);

            // ステートの変更
            ChengeState(GameState.Wait);
        });
    }
}
