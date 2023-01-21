using System;
using System.Linq;
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

    [Header("Components")]
    [SerializeField]
    Fade _fade = default;

    [SerializeField]
    Image _fadeImage = default;

    [SerializeField]
    FadeImage _fadeMaskImage = default;

    [Header("MaskData")]
    [SerializeField]
    MaskData _maskData = default;
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
        //FadeOut(_fadeType);
        Cursor.visible = false;
    }
    /// <summary>
    /// シーン遷移する
    /// </summary>
    /// <param name="scene"> 遷移先のScene </param>
    /// <param name="action">  </param>
    public static void SceneTransition(SceneType scene, FadeType fade = default, Action action = null)
    {
        FadeIn(fade, action: () => 
        {
            action?.Invoke();
            Instance.StartCoroutine(Instance.LoadScene(scene, Instance._loadTime));
        });
    }

    /// <summary>
    /// 画面を徐々に映す
    /// </summary>
    public static void FadeOut(FadeType fade, float fadeTime = 1.5f, Action action = null)
    {
        switch (fade)
        {
            case FadeType.Normal:
                Instance._fade.FadeOut(0f);
                Instance._fadeImage.enabled = true;
                Instance._fadeImage.DOFade(1f, 0f);
                Instance._fadeImage.DOFade(0f, fadeTime)
                          .OnComplete(() =>
                          {
                              action?.Invoke();
                          });
                break;
            case FadeType.White_default:
                Instance._fade.FadeOut(0f);
                Instance._fadeImage.DOColor(new Color(1, 1, 1, 0), 0);
                Instance._fadeImage.enabled = true;
                Instance._fadeImage.DOFade(1f, 0f);
                Instance._fadeImage.DOFade(0f, fadeTime)
                          .OnComplete(() =>
                          {
                              action?.Invoke();
                          });
                break;
            case FadeType.Black_default:
                Instance._fade.FadeOut(0f);
                Instance._fadeImage.DOColor(new Color(0, 0, 0, 0), 0);
                Instance._fadeImage.enabled = true;
                Instance._fadeImage.DOFade(1f, 0f);
                Instance._fadeImage.DOFade(0f, fadeTime)
                          .OnComplete(() =>
                          {
                              action?.Invoke();
                          });
                break;
            case FadeType.Mask_CheshireCat:
                Instance._fade.FadeIn(0f, () => 
                {
                    Instance._fadeImage.DOFade(0f, 0f);
                    var cheshireMaskData = Instance._maskData.Masks.FirstOrDefault(p => p.MaskType == MaskType.CheshireCat);
                    Instance._fadeImage.enabled = true;
                    Instance._fadeMaskImage.UpdateMaskTexture(cheshireMaskData.MaskTexture);
                    Instance._fadeMaskImage.material.color = cheshireMaskData.MaskColor;
                    Instance._fade.FadeOut(fadeTime, action);
                });
                break;
            case FadeType.Mask_Heart:
                Instance._fade.FadeIn(0f, () => 
                {
                    Instance._fadeImage.DOFade(0f, 0f);
                    var heartMaskData = Instance._maskData.Masks.FirstOrDefault(p => p.MaskType == MaskType.Heart);
                    Instance._fadeImage.enabled = true;
                    Instance._fadeMaskImage.UpdateMaskTexture(heartMaskData.MaskTexture);
                    Instance._fadeMaskImage.material.color = heartMaskData.MaskColor;
                    Instance._fade.FadeOut(fadeTime, action);
                });
                break;
            case FadeType.Mask_KeyHole:
                Instance._fade.FadeIn(0f, () => 
                {
                    Instance._fadeImage.DOFade(0f, 0f);
                    var keyholeMaskData = Instance._maskData.Masks.FirstOrDefault(p => p.MaskType == MaskType.KeyHole);
                    Instance._fadeImage.enabled = true;
                    Instance._fadeMaskImage.UpdateMaskTexture(keyholeMaskData.MaskTexture);
                    Instance._fadeMaskImage.material.color = keyholeMaskData.MaskColor;
                    Instance._fade.FadeOut(fadeTime, action);
                });
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 画面を徐々に隠す
    /// </summary>
    public static void FadeIn(FadeType fade, float fadeTime = 1.5f, Action action = null)
    {
        switch (fade)
        {
            case FadeType.Normal:
                Instance._fadeImage.enabled = true;
                Instance._fadeImage.DOFade(0f, 0f);
                Instance._fadeImage.DOFade(1f, fadeTime)
                          .OnComplete(() =>
                          {
                              action?.Invoke();
                          });
                break;
            case FadeType.White_default:
                Instance._fadeImage.DOColor(new Color(1, 1, 1, 1), fadeTime)
                          .OnComplete(() =>
                          {
                              action?.Invoke();
                          });
                break;
            case FadeType.Black_default:
                Instance._fadeImage.DOColor(new Color(0, 0, 0, 1), fadeTime)
                          .OnComplete(() =>
                          {
                              action?.Invoke();
                          });
                break;
            case FadeType.White_Transparent:
                Instance._fadeImage.DOColor(new Color(1, 1, 1, 0), fadeTime)
                                   .OnComplete(() =>
                                   {
                                       action?.Invoke();
                                   });
                break;
            case FadeType.Black_TransParent:
                Instance._fadeImage.DOColor(new Color(0, 0, 0, 0), fadeTime)
                                   .OnComplete(() =>
                                   {
                                       action?.Invoke();
                                   });
                break;
            case FadeType.Mask_CheshireCat:
                var cheshireMaskData = Instance._maskData.Masks.FirstOrDefault(p => p.MaskType == MaskType.CheshireCat);
                Instance._fadeImage.enabled = true;
                Instance._fadeMaskImage.UpdateMaskTexture(cheshireMaskData.MaskTexture);
                Instance._fadeMaskImage.material.color = cheshireMaskData.MaskColor;
                Instance._fadeImage.DOFade(0f, 0.05f)
                                   .OnComplete(() =>
                                   { 
                                       Instance._fade.FadeIn(fadeTime, action);
                                   });
                break;
            case FadeType.Mask_Heart:
                var heartMaskData = Instance._maskData.Masks.FirstOrDefault(p => p.MaskType == MaskType.Heart);
                Instance._fadeImage.enabled = true;
                Instance._fadeMaskImage.UpdateMaskTexture(heartMaskData.MaskTexture);
                Instance._fadeMaskImage.material.color = heartMaskData.MaskColor;
                Instance._fadeImage.DOFade(0f, 0.05f)
                                   .OnComplete(() =>
                                   {
                                       Instance._fade.FadeIn(fadeTime, action);
                                   });
                break;
            case FadeType.Mask_KeyHole:
                var keyholeMaskData = Instance._maskData.Masks.FirstOrDefault(p => p.MaskType == MaskType.KeyHole);
                Instance._fadeImage.enabled = true;
                Instance._fadeMaskImage.UpdateMaskTexture(keyholeMaskData.MaskTexture);
                Instance._fadeMaskImage.material.color = keyholeMaskData.MaskColor;
                Instance._fadeImage.DOFade(0f, 0.05f)
                                   .OnComplete(() =>
                                   {
                                       Instance._fade.FadeIn(fadeTime, action);
                                   });
                break;
            default:
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
    UnderLobby,
    Stage1_Fall,
    RE_Stage2,
    RE_Stage3,
    Stage4,
    Stage5,
    Stage6,
    Stage_Boss,
    Ending
}
public enum FadeType
{
    Normal,
    White_default,
    Black_default,
    White_Transparent,
    Black_TransParent,
    Mask_CheshireCat,
    Mask_Heart,
    Mask_KeyHole
}
