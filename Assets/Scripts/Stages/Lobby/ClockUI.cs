using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 時計のUIの機能を持つコンポーネント
/// </summary>
public class ClockUI : MonoBehaviour
{
    #region serialize
    [Tooltip("時計のImage")]
    [SerializeField]
    Image _clockImage = default;

    [Tooltip("時計UIのデータ")]
    [SerializeField]
    ClockUIData[] _clockDatas = default;
    #endregion

    #region private
    #endregion

    #region public
    #endregion

    #region property
    public static ClockUI Instance { get; private set; }
    #endregion

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        SetClockUI(GameManager.Instance.CurrentClockState);
    }

    public void SetClockUI(ClockState State)
    {
        _clockImage.sprite = _clockDatas.FirstOrDefault(d => d.ClockState == State).ClockUISprite;
    }
}

[Serializable]
public struct ClockUIData
{
    public string ClockUIName;
    public ClockState ClockState;
    public Sprite ClockUISprite;
}
