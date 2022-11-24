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

    [Header("�Ó]�ɂ����鎞��")]
    [SerializeField]
    private float _durationOfBlackOut;

    private GameState _state = GameState.None;

    private void Start()
    {
        ChengeStage(GameState.CreateOrder);
        // �Z�{�^���ɏ����̓o�^
        _player.RegistToOnCircleButton(() => ChengeStage(GameState.MoveCamera));
    }

    /// <summary>
    /// �X�e�[�g�̕ύX
    /// </summary>
    private void ChengeStage(GameState next)
    {
        Debug.Log($"�X�e�[�g�ύX {_state} -> {next}");

        switch (next)
        {
            case GameState.None:
                break;
            case GameState.CreateOrder:
                // �I�[�_�[�̍쐬
                var order = _order.CreateOrder(() => ChengeStage(GameState.MoveCamera));
                _uiManager.DisplayOrder(order);
                break;
            case GameState.MoveCamera:
                // Order�̔�\��
                _uiManager.UIDisable(Stage3UIManager.Type.OrderUI);
                // �J�����̈ړ�
                _cameraCtrl.MoveRequest(() => ChengeStage(GameState.Throw));
                // �Ó]�̏���
                _uiManager.BeginBlackOut(_durationOfBlackOut);
                break;
            case GameState.Throw:
                // �Ó]�p�l���̔�\��
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