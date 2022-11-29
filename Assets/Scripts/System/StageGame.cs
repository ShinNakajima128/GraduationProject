using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StageGame<T> : MonoBehaviour
{
    #region serialize
    [Tooltip("管理するStage")]
    [SerializeField]
    Stages _managedStage = default;
    #endregion
    #region private
    #endregion
    #region public
    public abstract event Action GameSetUp;
    public abstract event Action GameStart;
    public abstract event Action GamePause;
    public abstract event Action GameEnd;
    #endregion
    #region property
    public static StageGame<T> Instance { get; private set; }
    public Action<QuizType> QuizSetUp { get; internal set; }
    #endregion

    protected void Awake()
    {
        Instance = this;
    }

    protected virtual void Start()
    {
        //現在のステージをGameManagerに更新
        GameManager.UpdateCurrentStage(_managedStage);
    }
    public abstract void OnGameSetUp();
    public abstract void OnGameStart();
    public abstract void OnGameEnd();
    protected abstract  void Init();
    protected abstract IEnumerator GameStartCoroutine(Action action = null);
    protected abstract IEnumerator GameEndCoroutine(Action action = null);
}
