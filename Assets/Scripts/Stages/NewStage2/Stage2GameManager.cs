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

    // �Z�{�^��or���N���b�N�������ꂽ��
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
                        // ���r�[�Ɉړ�
                        GameManager.SaveStageResult(true);
                        TransitionManager.SceneTransition(SceneType.Lobby);
                    }
                    break;
                default:
                    break;
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
                _camera.ZoomRequest(Stage2CameraController.ZoomType.Out, () => ChengeState(GameState.Wait));
                break;
            case GameState.Wait:
                // �V���b�t���X�^�[�g��ʂ̕\��
                _uiCtrl.ChengeActive(Stage2UIController.UIType.Play, true);
                // �Y�[�����Ă�����A�Y�[���A�E�g���Ă���t�F�[�Y��ς���
                if (_camera.IsZoomed is true)
                {
                    _camera.ZoomRequest(Stage2CameraController.ZoomType.Out, () => ChengeState(GameState.Wait));
                }
                break;
            case GameState.Shuffle:
                // �X�^�[�g�p�l���̔�\��
                _uiCtrl.ChengeActive(Stage2UIController.UIType.Play, false);
                // UI�̔�\��
                _uiCtrl.SetDisable();
                // �I���@�\�̒�~
                _selector.Stop();
                // �V���b�t���Ɉڍs�̏���
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
    /// ��������A�X�e�[�g��؂�ւ���
    /// </summary>
    public void Judge(int selectedNumber)
    {
        if (_state != GameState.Select) return;

        // �}�E�X�������Ă���Index���擾
        var num = _mugcupManager.GetInMouseCupNumber();

        // �I���A�C�R���̍폜
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
        _uiCtrl.ChengeActive(Stage2UIController.UIType.Clear, true);
        // �}�E�X���N���オ��
        _mouse.OnAnimation(MouseState.WakeUp, 0.5f);

        if (_currentShuffleFase != ShuffleFase.Three)
        {
            // �t�F�[�Y�̐؂�ւ�
            _currentShuffleFase++;
            // �I�������J�b�v��������
            _mugcupManager.OpenRequest(selectedNumber, () =>
            {
                // �A�j���[�V����
                _mouse.OnAnimation(MouseState.CloseEar, 0.5f);
                // UI�̔�\��
                _uiCtrl.ChengeActive(Stage2UIController.UIType.Clear, false);
                // ��������A�X�e�[�g��؂�ւ���
                _mugcupManager.CloseRequest(selectedNumber, () =>
                {
                    // �Y�[�����Ă�����A�Y�[���A�E�g���Ă���t�F�[�Y��ς���
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
            // UI�̕\��
            _uiCtrl.ChengeActive(Stage2UIController.UIType.GameClear, true);

            // �Y�[�����Ă�����A�Y�[���A�E�g���Ă���t�F�[�Y��ς���
            if (_camera.IsZoomed is true)
            {
                _camera.ZoomRequest(Stage2CameraController.ZoomType.Out, () => ChengeState(GameState.GameEnd));
            }
        }
    }

    /// <summary>
    /// �ԈႦ����
    /// </summary>
    private void Miss()
    {
        Debug.Log("���s");

        _uiCtrl.ChengeActive(Stage2UIController.UIType.Miss, true);
        // �l�Y�~���N���オ��
        _mouse.OnAnimation(MouseState.OpenEar, 1f);
        // �S�Ă�����
        _mugcupManager.OpenAllMugCup(() =>
        {
            // �Q��
            _mouse.OnAnimation(MouseState.CloseEar, 1f);

            // UI�̕\��
            _uiCtrl.ChengeActive(Stage2UIController.UIType.Miss, false);

            // �X�e�[�g�̕ύX
            ChengeState(GameState.Wait);
        });
    }
}
