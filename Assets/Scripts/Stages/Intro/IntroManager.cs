using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �C���g��Scene�̊Ǘ����s���}�l�[�W���[�N���X
/// </summary>
public class IntroManager : MonoBehaviour
{
    [SerializeField]
    MessagePlayer _player = default;

    IEnumerator Start()
    {
        yield return StartCoroutine(_player.PlayAllMessageCoroutine());
        //yield return StartCoroutine(_player.PlayMessageCorountine(MessageType.Intro));

        TransitionManager.SceneTransition(SceneType.Stage1_Fall);
    }
}
