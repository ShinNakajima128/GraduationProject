using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// イントロSceneの管理を行うマネージャークラス
/// </summary>
public class IntroManager : MonoBehaviour
{
    [SerializeField]
    MessagePlayer _player = default;

    [SerializeField]
    SceneType _nextSceneType = default;

    private void Start()
    {
        TransitionManager.FadeOut(FadeType.Normal, action: () =>
         {
             StartCoroutine(StartScenario());
         });

    }
    IEnumerator StartScenario()
    {
        yield return StartCoroutine(_player.PlayAllMessageCoroutine());

        TransitionManager.SceneTransition(_nextSceneType);
    }
}
