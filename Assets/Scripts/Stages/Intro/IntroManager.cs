using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

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
        if (_nextSceneType == SceneType.Stage1_Fall)
        {
            AudioManager.PlayBGM(BGMType.Intro);
            
        }
        else
        {
            TransitionManager.FadeIn(FadeType.White_Transparent, 0f);
            AudioManager.PlayBGM(BGMType.Ending);
        }

        TransitionManager.FadeOut(FadeType.Normal, action: () =>
         {
             StartCoroutine(StartScenario());
         });
    
        SkipButton.Instance.Isrespond += SkipResponse;
        SkipButton.Instance.OnSkip.Subscribe(_ =>
        {
            TransitionManager.FadeIn(FadeType.Black_Transparent, 0f);
            TransitionManager.SceneTransition(_nextSceneType);
        });
    }

    IEnumerator StartScenario()
    {
        yield return StartCoroutine(_player.PlayAllMessageCoroutine());

        TransitionManager.FadeIn(FadeType.Black_Transparent, 0f);
        TransitionManager.SceneTransition(_nextSceneType);
    }

    bool SkipResponse()
    {
        return true;
    }
}
