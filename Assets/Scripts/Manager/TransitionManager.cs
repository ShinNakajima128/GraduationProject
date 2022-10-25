using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    #endregion

    #region property
    public static TransitionManager Instance { get; private set; }
    #endregion

    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// シーン遷移する
    /// </summary>
    /// <param name="scene"> 遷移先のScene </param>
    /// <param name="action">  </param>
    public static void SceneTransition(SceneType scene, FadeType fade = default, Action action = null)
    {

    }

    /// <summary>
    /// 画面を徐々に映す
    /// </summary>
    void FadeIn()
    {

    }

    /// <summary>
    /// 画面を徐々に隠す
    /// </summary>
    void FadeOut()
    {

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
    Stage1,
    Stage2,
    Stage3,
    Stage4,
    Stage5,
    Stage6,
    BossScene
}
public enum FadeType
{
    None
}
