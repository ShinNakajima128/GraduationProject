using UnityEngine;
using UnityEngine.InputSystem;

public class Stage2GameManager : MonoBehaviour
{
    #region Define
    private enum GameState
    {
        None,
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

    private GameState _state;
    private ShuffleFase _currentShuffleFase;

    private void Start()
    {
        _input.actions["Enter"].started += GoToShuffle;
        ChengeState(GameState.ZoomIn);
        _currentShuffleFase = ShuffleFase.One;
    }

    private void GoToShuffle(InputAction.CallbackContext context)
    {
        if (_state == GameState.Wait)
        {
            // 全部下げた後、シャッフル開始
            _mugcupManager.CloseAllMugCup(() => ChengeState(GameState.Shuffle));
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
                _camera.ZoomRequest(Stage2CameraController.ZoomType.Out, () => ChengeState(GameState.Shuffle));
                break;
            case GameState.Shuffle:
                // UIの非表示
                _uiCtrl.SetDisable();
                // 選択機能の停止
                _selector.Stop();
                // シャッフルに移行の処理
                _mugcupManager.Shuffle(_currentShuffleFase, () =>
                {
                    ChengeState(GameState.Select);
                });
                break;
            case GameState.Select:
                _selector.Begin();
                break;
            case GameState.Wait:
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

        // アイコンの削除
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

        // マウスを起き上がる
        _mouse.OnAnimation(MouseState.WakeUp, 0.5f);

        if (_currentShuffleFase != ShuffleFase.Three)
        {
            // フェーズの切り替え
            _currentShuffleFase++;

            // 選択したカップをあげる
            _mugcupManager.OpenRequest(selectedNumber, () =>
            {
                // マウスをねかせる
                _mouse.OnAnimation(MouseState.CloseEar, 0.5f);
                // UIの表示
                _uiCtrl.ChengeActivate(Stage2UIController.UIType.Clear, true);
                // 下げた後、ステートを切り替える
                _mugcupManager.CloseRequest(selectedNumber, () =>
                {
                    ChengeState(GameState.Wait);
                }
                );
            });
        }
        else if (_currentShuffleFase == ShuffleFase.Three)
        {
            _mugcupManager.OpenRequest(num);
            // UIの表示
            _uiCtrl.ChengeActivate(Stage2UIController.UIType.GameClear, true);
            ChengeState(GameState.GameEnd);
        }
    }

    /// <summary>
    /// 間違えた時
    /// </summary>
    private void Miss()
    {
        Debug.Log("失敗");

        // ネズミが起き上がる
        _mouse.OnAnimation(MouseState.WakeUp, 1f);
        // 全てあげる
        _mugcupManager.OpenAllMugCup(() =>
        {
            // 寝る
            _mouse.OnAnimation(MouseState.CloseEar, 1f);

            // UIの表示
            _uiCtrl.ChengeActivate(Stage2UIController.UIType.Miss, true);

            Debug.Log("All Open");
            // 全てあげた後に閉じる
            _mugcupManager.CloseAllMugCup(() =>
            {
                Debug.Log("All Close");

                ChengeState(GameState.Wait);
            });
        });
    }
}
