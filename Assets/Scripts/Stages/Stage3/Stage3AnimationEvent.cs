using UnityEngine;

/// <summary>
/// �A�j���[�V�����C�x���g�p�N���X
/// </summary>
public class Stage3AnimationEvent : MonoBehaviour
{
    [SerializeField]
    private Stage3PlayerController _player;

    public void OnThrow()
    {
        _player.Throw();
    }

    public void OnCheckState()
    {
        _player.LookForForward();
    }

    public void OnStandby()
    {
        _player.EndStandby();
    }
}
