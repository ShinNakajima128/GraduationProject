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

    [Header("�Ó]�ɂ����鎞��")]
    [SerializeField]
    private float _blackOutDuration;

    [SerializeField]
    private int _needClearCount;

    private GameState _state = GameState.None;

    /// <summary>
    /// �Q�[���N���A�̉�
    /// </summary>
    private int _currentClearCount;

    private void Start()
    {
        ChengeStage(GameState.CreateOrder);
        // �Z�{�^���ɏ����̓o�^
        _player.RegistToOnCircleButton(() => ChengeStage(GameState.MoveCamera));
        // �S�[�����̋����̒ǉ�
        _ball.AddCallBack(() => ChengeStage(GameState.Result));
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
                var order = _orderMana.CreateOrder();
                // �����UI�\��
                _uiManager.DisplayOrder(order);
                break;
            case GameState.MoveCamera:
                // Order�̔�\��
                _uiManager.ChengeUIActivete(Stage3UIManager.Type.OrderUI, false);
                // �J�����̈ړ��@�ړ���X�e�[�g�̕ύX
                _cameraCtrl.MoveRequest(() => ChengeStage(GameState.Throw));
                // �Ó]����
                _uiManager.BeginBlackOut(_blackOutDuration);
                break;
            case GameState.Throw:
                // InGameUI�̕\��
                _uiManager.ChengeUIActivete(Stage3UIManager.Type.IngameUI, true);
                // �Ó]�p�l���̔�\��
                _uiManager.ChengeUIActivete(Stage3UIManager.Type.BlackOutImage, false);
                // ������J�n
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
    /// �X�e�[�W�̃N���A��������
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