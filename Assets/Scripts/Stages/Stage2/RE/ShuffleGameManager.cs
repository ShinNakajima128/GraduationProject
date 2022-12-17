using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShuffleGameManager : StageGame<ShuffleGameManager>
{
    #region serialize
    [Header("Variables")]
    [SerializeField]
    int _phaseCount = 3;

    [Header("UI")]
    [SerializeField]
    Text _infoText = default;

    [Header("Components")]
    [SerializeField]
    Stage2CameraManager _stage2Cameras = default;

    [SerializeField]
    TeacupManager _teacupManager = default;

    [SerializeField]
    TeacupController _teacupCtrl = default;
    #endregion

    #region private
    bool _isChoiced = false;
    #endregion

    #region public
    public override event Action GameSetUp;
    public override event Action GameStart;
    public override event Action GamePause;
    public override event Action GameEnd;
    #endregion
    
    #region property
    public static new ShuffleGameManager Instance { get; private set; }
    #endregion

    protected override void Awake()
    {
        Instance = this;
    }

    protected override void Start()
    {
        base.Start();
        OnGameStart();
        Init();        
    }

    public override void OnGameSetUp()
    {
    }

    public override void OnGameStart()
    {
        StartCoroutine(GameStartCoroutine());
    }

    public override void OnGameEnd()
    {
    }

    /// <summary>
    /// ゲーム開始時のコルーチン
    /// </summary>
    /// <param name="action"> コルーチン終了時のAction </param>
    protected override IEnumerator GameStartCoroutine(Action action = null)
    {
        yield return _stage2Cameras.StartDirectionCoroutine();

        yield return new WaitForSeconds(0.5f);

        _stage2Cameras.ChangeCamera(Stage2CameraType.CloseupMouse);

        yield return new WaitForSeconds(2.5f);

        _teacupManager.AllCupDown();

        yield return new WaitForSeconds(1.1f);

        _teacupManager.OnMouseAnimation(MouseState.CloseEar);
        
        yield return new WaitForSeconds(1.4f);

        _stage2Cameras.ChangeCamera(Stage2CameraType.Ingame);

        yield return new WaitForSeconds(2.5f);

        _infoText.text = "Start!";

        yield return new WaitForSeconds(2.0f);

        _infoText.text = "";
        StartCoroutine(InGameCoroutine());
    }

    IEnumerator InGameCoroutine()
    {
        for (int i = 0; i < _phaseCount; i++)
        {
            _isChoiced = false;

            yield return _teacupCtrl.ShuffleCoroutine((ShufflePhase)i);

            yield return new WaitUntil(() => _isChoiced);
        }
    }

    protected override IEnumerator GameEndCoroutine(Action action = null)
    {
        throw new NotImplementedException();
    }

    protected override void Init()
    {
        TransitionManager.FadeOut(FadeType.Normal);
        AudioManager.PlayBGM(BGMType.Stage2);
        _teacupManager.RandomHideMouse();
        _infoText.text = "";
    }
}
/// <summary>
/// シャッフルゲームのフェイズの種類
/// </summary>
public enum ShuffleGamePhase
{
    Shuffle_ExChange,
    Shuffle_ExChangeAndWarp,
    Shuffle_High_ExChangeAndWarp
}
