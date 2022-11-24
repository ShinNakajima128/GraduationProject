using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

/// <summary>
/// Scene遷移、画面フェードの機能をまとめたコンポーネント
/// </summary>
public class TransitionManager : MonoBehaviour
{
    #region serialize
    [SerializeField]
    float _fadeTime = 1.0f;

    [SerializeField]
    float _loadTime = 2.0f;

    [SerializeField]
    FadeType _fadeType = FadeType.Normal;

    [Header("Components")]
    [SerializeField]
    Fade _fade = default;

    [SerializeField]
    Image _fadeImage = default;
    #endregion

    #region property
    public static TransitionManager Instance { get; private set; }
    #endregion

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        FadeOut(_fadeType);
    }
    /// <summary>
    /// シーン遷移する
    /// </summary>
    /// <param name="scene"> 遷移先のScene </param>
    /// <param name="action">  </param>
    public static void SceneTransition(SceneType scene, FadeType fade = default, Action action = null)
    {
        FadeIn(fade, () => 
        {
            action?.Invoke();
            Instance.StartCoroutine(Instance.LoadScene(scene, Instance._loadTime));
        });
    }

    /// <summary>
    /// 画面を徐々に映す
    /// </summary>
    public static void FadeOut(FadeType fade, Action action = null)
    {
        switch (fade)
        {
            case FadeType.Normal:
                Instance._fadeImage.enabled = true;
                Instance._fadeImage.DOFade(1f, 0f);
                Instance._fadeImage.DOFade(0f, Instance._fadeTime)
                          .OnComplete(() =>
                          {
                              action?.Invoke();
                          });
                break;
            default:
                Instance._fade.FadeOut(Instance._fadeTime, action);
                break;
        }
    }

    /// <summary>
    /// 画面を徐々に隠す
    /// </summary>
    public static void FadeIn(FadeType fade, Action action = null)
    {
        switch (fade)
        {
            case FadeType.Normal:
                Instance._fadeImage.enabled = true;
                Instance._fadeImage.DOFade(0f, 0f);
                Instance._fadeImage.DOFade(1f, Instance._fadeTime)
                          .OnComplete(() =>
                          {
                              action?.Invoke();
                          });
                break;
            default:
                Instance._fade.FadeIn(Instance._fadeTime, action);
                break;
        }
    }

    IEnumerator LoadScene(SceneType scene,float loadTime)
    {
        yield return new WaitForSeconds(loadTime);

        SceneManager.LoadScene(scene.ToString());
    }
}

/// <summary>
/// Sceneの種類
/// </summary>
public enum SceneType
{
    Title,
    Intro,
    Lobby,
    Stage1_Fall,
    Stage2,
    Stage3,
    Stage4,
    Stage5,
    Stage6,
    BossScene
}
public enum FadeType
{
    Normal,
    Mask1
}
