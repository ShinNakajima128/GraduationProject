using UnityEngine;

public class Stage2GameManager : MonoBehaviour
{
    private enum State
    {
        None,
        ZoomOut,
        shuffle,
        Select
    }

    [SerializeField]
    private Stage2MugcapManager _mugcapManager;

    [SerializeField]
    private Stage2CameraController _cameraController;

    private State _state;

    private void Awake()
    {
        ChengeState(State.None);
    }

    private void Start()
    {
        ChengeState(State.ZoomOut);
    }

    /// <summary>
    /// �X�e�[�g�̕ύX
    /// </summary>
    private void ChengeState(State next)
    {
        switch (next)
        {
            case State.None:
                break;
            case State.ZoomOut:
                // �J�����̃Y�[���A�E�g
                _cameraController.MoveRequest();
                // �l�Y�~�������Ă�J�b�v��������
                _mugcapManager.Setup(() => ChengeState(State.shuffle));
                break;
            case State.shuffle:
                break;
            case State.Select:
                break;
            default:
                break;
        }

        _state = next;
    }
}
