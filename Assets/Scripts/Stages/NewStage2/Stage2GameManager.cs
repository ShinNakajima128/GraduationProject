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
    /// �X�e�[�g�̕ύX
    /// </summary>
    private void ChengeState(GameState next)
    {
        Debug.Log($"�X�e�[�g��{ next}�ɕύX");

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
                break;
            default:
                break;
        }

        _state = next;
    }

    /// <summary>
    /// ��������A�X�e�[�g��؂�ւ���
    /// </summary>
    internal void Judge(int currentSelectNum)
    {
        if (_state != GameState.Select) return;

        var num = _mugcupManager.GetInMouseCupNumber();

        if (num == currentSelectNum)
        {
            Debug.Log("����");
            if (_currentShuffleFase != ShuffleFase.Three)
            {
                // �V���b�t���X�e�[�g�̐؂�ւ�
                _currentShuffleFase++;
                ChengeState(GameState.Shuffle);
            }
        }
        else
        {
            ChengeState(GameState.Shuffle);
            Debug.Log("���s");
        }
    }
}
