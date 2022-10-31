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

    private void Start()
    {
        TransitionManager.FadeOut(FadeType.Normal, () => 
        {
            StartCoroutine(StartIntro());
        });
        
    }
    IEnumerator StartIntro()
    {
        yield return StartCoroutine(_player.PlayAllMessageCoroutine());

        TransitionManager.SceneTransition(SceneType.Stage1_Fall);
    }
}
