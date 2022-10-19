using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ティーカップゲームを管理するManagerクラス
/// </summary>
public class TeapotGameManager : MonoBehaviour
{
    #region serialize
    [Tooltip("ティーカップを選択するボタンのPanel")]
    [SerializeField]
    GameObject _selectButtonPanel = default;

    [Tooltip("リザルト画面のPanel")]
    [SerializeField]
    GameObject _resultPanel = default;

    [Tooltip("選択するButton")]
    [SerializeField]
    Button[] _selectButtons = default;

    [Tooltip("もう一度プレイする用のButton")]
    [SerializeField]
    Button _replayButton = default;

    [Tooltip("結果を表示するText")]
    [SerializeField]
    Text _resultText = default;
    #endregion

    #region private
    /// <summary> 正解の位置の番号 </summary>
    int _correct = 0;
    #endregion
    #region property
    public static TeapotGameManager Instance { get; private set; }
    public Action GameStart { get; set; }
    public Action TeapotSelectPhase { get; set; }
    public Action<int, bool> OpenTeacupPhase { get; set; }
    public Action GameEnd { get; set; }
    public Action Initialize { get; set; }
    #endregion

    private void Awake()
    {
        Instance = this;
    }
    IEnumerator Start()
    {
        _replayButton.onClick.AddListener(() => OnGameStart());
        yield return null;
        OnGameStart();
    }

    /// <summary>
    /// ティーポットを選択する画面を表示する
    /// </summary>
    public void OnSelectTeacup(int correctNum)
    {
        _selectButtonPanel.SetActive(true);
        TeapotSelectPhase?.Invoke();
        _correct = correctNum;
    }

    /// <summary>
    /// ゲームを開始する
    /// </summary>
    void OnGameStart()
    {
        InitializeObject();
        GameStart?.Invoke();
    }
    /// <summary>
    /// ゲームを終了する
    /// </summary>
    void OnGameEnd()
    {
        GameEnd?.Invoke();
    }
    /// <summary>
    /// オブジェクトの初期化
    /// </summary>
    void InitializeObject()
    {
        _selectButtonPanel.SetActive(false);
        _resultPanel.SetActive(false);
        Initialize?.Invoke();
    }
    /// <summary>
    /// Buttonを選択
    /// </summary>
    /// <param name="buttonNum"> 選択したButtonの番号 </param>
    public void Select(int buttonNum)
    {
        StartCoroutine(OnResult(buttonNum));
    }

    IEnumerator OnResult(int buttonNum)
    {
        var waitTime = new WaitForSeconds(1f);
        bool result = _correct == buttonNum;

        OpenTeacupPhase?.Invoke(buttonNum, result);
        _selectButtonPanel.SetActive(false);

        yield return waitTime;

        if (result) //正解の位置と選択したボタンの番号が同じ場合
        {
            _resultText.text = "あたり！";
        }
        else
        {
            _resultText.text = "はずれ";
        }

        yield return waitTime;

        _resultPanel.SetActive(true);　//リザルト画面表示
    }
}
