using UnityEngine;

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
        GameEnd
    }
    #endregion

    [SerializeField]
    private Stage2CameraController _camera;

    [SerializeField]
    private Stage2MugcupManager _mugcupManager;

    [SerializeField]
    private Stage2Selector _selector;

    private GameState _state;
    private ShuffleFase _currentShuffleFase;

    private void Start()
    {
        ChengeState(GameState.ZoomIn);
        _currentShuffleFase = ShuffleFase.One;
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
                break;
            case GameState.ZoomOut:
                _camera.ZoomRequest(Stage2CameraController.ZoomType.Out, () => ChengeState(GameState.Shuffle));
                break;
            case GameState.Shuffle:
                _selector.Stop();
                _mugcupManager.Shuffle(_currentShuffleFase, () =>
                {
                    ChengeState(GameState.Select);
                });
                break;
            case GameState.Select:
                _selector.Begin();
                break;
            case GameState.GameEnd:
                Debug.Log("Game Clear");
                break;
            default:
                break;
        }

        _state = next;
    }

    /// <summary>
    /// 判定をし、ステートを切り替える
    /// </summary>
    internal void Judge(int selectedNumber)
    {
        if (_state != GameState.Select) return;

        // マウスが入っているIndexを取得
        var num = _mugcupManager.GetInMouseCupNumber();

        // アイコンの削除
        _selector.Stop();

        // 正解時
        if (num == selectedNumber)
        {
            Debug.Log("成功");
            // 選んだカップをあげる

            if (_currentShuffleFase != ShuffleFase.Three)
            {
                _currentShuffleFase++;
                // 選択したカップをあげる
                _mugcupManager.OpenRequest(selectedNumber,() => 
                {
                    // 下げた後、ステートを切り替える
                    _mugcupManager.CloseRequest(selectedNumber,() => ChengeState(GameState.Shuffle));
                });
            }
            else if (_currentShuffleFase == ShuffleFase.Three)
            {
                ChengeState(GameState.GameEnd);
            }
        }
        // 不正解
        else
        {
            Debug.Log("失敗");
            // 全てあげる
            _mugcupManager.OpenAllMugCup(() =>
            {
                Debug.Log("All Open");
                // 全てあげた後に閉じる
                _mugcupManager.CloseAllMugCup(() =>
                {
                    Debug.Log("All Close");
                    // 配列のリセット
                    _mugcupManager.ResetForArray(() =>
                    {
                        // ステートの変更
                        ChengeState(GameState.Shuffle);
                    });
                });
            });
        }
    }
}
