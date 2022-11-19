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
    #region property
    public static StageGame<T> Instance { get; private set; }
    public abstract Action GameStart { get; set; }
    public abstract Action GamePause { get; set; }
    public abstract Action GameEnd { get; set; }
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
    public abstract void OnGameStart();
    public abstract void OnGameEnd();
    protected abstract  void Init();
    protected abstract IEnumerator GameStartCoroutine(Action action = null);
    protected abstract IEnumerator GameEndCoroutine(Action action = null);
}
