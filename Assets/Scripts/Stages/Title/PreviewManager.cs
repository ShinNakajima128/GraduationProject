using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class PreviewManager : MonoBehaviour
{
    #region serialize
    [Tooltip("プレビュー再生までの時間")]
    [SerializeField]
    float _waitTime = 15f;

    [Header("UI_Object")]
    [SerializeField]
    CanvasGroup _previewPanelGroup = default;
    #endregion

    #region private
    VideoPlayer _player;
    #endregion

    #region public
    #endregion

    #region property
    public static PreviewManager Instance { get; private set; }
    public bool IsPreviewed { get; private set; } = false;
    #endregion

    private void Awake()
    {
        Instance = this;
        TryGetComponent(out _player);
    }

    private void Start()
    {
        _player.loopPointReached += FinishMovie;
        _player.started += v =>
        {
            TransitionManager.FadeOut(FadeType.Mask_KeyHole);
            _previewPanelGroup.alpha = 1;
            AudioManager.StopBGM();
            StartCoroutine(StopMovie());
            IsPreviewed = true;
        };
        StartCoroutine(StartCount());
    }

    /// <summary>
    /// プレビュー再生カウントダウン開始
    /// </summary>
    IEnumerator StartCount()
    {
        yield return null;

        Debug.Log("カウント開始");
        float timer = 0;

        while (_waitTime >= timer)
        {
            if (UIInput.Any)
            {
                Debug.Log("カウントリセット");
                StartCoroutine(StartCount());
                yield break;
            }
            timer += Time.deltaTime;
            yield return null;
        }

        TransitionManager.FadeIn(FadeType.Mask_KeyHole, action: () =>
        {
            _player.Play();
        });
    }
    /// <summary>
    /// プレビューを停止
    /// </summary>
    IEnumerator StopMovie()
    {
        while (!UIInput.Any)
        {
            yield return null;
        }
        FinishMovie();

        Debug.Log("ムービーキャンセル");
    }

    /// <summary>
    /// ムービー再生終了時のCallback
    /// </summary>
    void FinishMovie(VideoPlayer vp = null)
    {
        TransitionManager.FadeIn(FadeType.Mask_KeyHole, action: () => 
        {
            TransitionManager.FadeOut(FadeType.Mask_KeyHole);
            _player.Stop();
            _previewPanelGroup.alpha = 0;
            AudioManager.PlayBGM(BGMType.Title);
            TitleManager.Instance.ChangeUIPanel(TitleUIType.Start);
            StartCoroutine(StartCount());
            IsPreviewed = false;
        });
    }
}
