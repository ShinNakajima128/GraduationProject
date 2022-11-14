using System;
using UnityChan;
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
        Select
    }
    #endregion

    [SerializeField]
    private Stage2CameraController _camera;

    [SerializeField]
    private Stage2MugcupManager _mugcupManager;

    private GameState _state;
    private ShuffleFase _currentShuffleFase;

    private void Start()
    {
        ChengeState(GameState.ZoomIn);
        ChengeFaze((ShuffleFase.One));
    }

    private void ChengeFaze(ShuffleFase next)
    {
        _currentShuffleFase = next;
    }

    /// <summary>
    /// ステートの変更
    /// </summary>
    private void ChengeState(GameState next)
    {
        Debug.Log(next.ToString());

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
                _mugcupManager.Shuffle(_currentShuffleFase, () =>
                {
                    ChengeState(GameState.Select);
                });
                break;
            case GameState.Select:
                break;
            default:
                break;
        }

        _state = next;
    }
}
