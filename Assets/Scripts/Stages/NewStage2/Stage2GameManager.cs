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
            // �S����������A�V���b�t���J�n
            _mugcupManager.CloseAllMugCup(() => ChengeState(GameState.Shuffle));
        }
    }

    /// <summary>
    /// �X�e�[�g�̕ύX
    /// </summary>
    private void ChengeState(GameState next)
    {
        Debug.Log($"�X�e�[�g��{next}�ɕύX");

        switch (next)
        {
            case GameState.None:
                break;
            case GameState.ZoomIn:
                _camera.ZoomRequest(Stage2CameraController.ZoomType.In, () => ChengeState(GameState.DownMugCap));
                break;
            case GameState.DownMugCap:
                _mugcupManager.Initialise(() => ChengeState(GameState.ZoomOut));
                // �A�j���[�V����
                _mouse.OnAnimation(MouseState.CloseEar, 2f);
                break;
            case GameState.ZoomOut:
                _camera.ZoomRequest(Stage2CameraController.ZoomType.Out, () => ChengeState(GameState.Shuffle));
                break;
            case GameState.Shuffle:
                // UI�̔�\��
                _uiCtrl.SetDisable();
                // �I���@�\�̒�~
                _selector.Stop();
                // �V���b�t���Ɉڍs�̏���
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
    /// ��������A�X�e�[�g��؂�ւ���
    /// </summary>
    public void Judge(int selectedNumber)
    {
        if (_state != GameState.Select) return;

        // �}�E�X�������Ă���Index���擾
        var num = _mugcupManager.GetInMouseCupNumber();

        // �A�C�R���̍폜
        _selector.Stop();

        // ������
        if (num == selectedNumber)
        {
            Clear(selectedNumber, num);
        }
        // �s����
        else
        {
            Miss();
        }
    }

    /// <summary>
    /// �������̏���
    /// </summary>
    private void Clear(int selectedNumber, int num)
    {
        Debug.Log("����");

        // �}�E�X���N���オ��
        _mouse.OnAnimation(MouseState.WakeUp, 0.5f);

        if (_currentShuffleFase != ShuffleFase.Three)
        {
            // �t�F�[�Y�̐؂�ւ�
            _currentShuffleFase++;

            // �I�������J�b�v��������
            _mugcupManager.OpenRequest(selectedNumber, () =>
            {
                // �}�E�X���˂�����
                _mouse.OnAnimation(MouseState.CloseEar, 0.5f);
                // UI�̕\��
                _uiCtrl.ChengeActivate(Stage2UIController.UIType.Clear, true);
                // ��������A�X�e�[�g��؂�ւ���
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
            // UI�̕\��
            _uiCtrl.ChengeActivate(Stage2UIController.UIType.GameClear, true);
            ChengeState(GameState.GameEnd);
        }
    }

    /// <summary>
    /// �ԈႦ����
    /// </summary>
    private void Miss()
    {
        Debug.Log("���s");

        // �l�Y�~���N���オ��
        _mouse.OnAnimation(MouseState.WakeUp, 1f);
        // �S�Ă�����
        _mugcupManager.OpenAllMugCup(() =>
        {
            // �Q��
            _mouse.OnAnimation(MouseState.CloseEar, 1f);

            // UI�̕\��
            _uiCtrl.ChengeActivate(Stage2UIController.UIType.Miss, true);

            Debug.Log("All Open");
            // �S�Ă�������ɕ���
            _mugcupManager.CloseAllMugCup(() =>
            {
                Debug.Log("All Close");

                ChengeState(GameState.Wait);
            });
        });
    }
}
