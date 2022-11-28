using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// メッセージを再生するコンポーネント
/// </summary>
public class MessagePlayer : MonoBehaviour
{
    #region serialize
    [Tooltip("テキストデータ")]
    [SerializeField]
    ScenarioData[] _data = default;

    [Tooltip("次の文字が表示されるまでの時間")]
    [SerializeField]
    float _flowTime = 0.05f;

    [Tooltip("メッセージ画面の透過速度")]
    [SerializeField]
    float _messagePanelFadeTime = 0.3f;
    
    [Header("UI")]
    [Tooltip("メッセージを表示するPanelのCanvasGroup")]
    [SerializeField]
    CanvasGroup _messagePanelCanvasGroup = default;

    [Tooltip("メッセージの話し手")]
    [SerializeField]
    Text _actorText = default;

    [Tooltip("メッセージ内容")]
    [SerializeField]
    Text _messageText = default;

    [Tooltip("入力待機中のアイコン")]
    [SerializeField]
    Image _submitIcon = default;

    [Tooltip("背景を変更するコンポーネント")]
    [SerializeField]
    BackgroundChanger _bc = default;
    #endregion

    #region private
    bool _isChanged = false;
    bool _isPressed = false;
    #endregion

    #region property
    #endregion
    private void Start()
    {
        _actorText.text = "";
        _messageText.text = "";
    }

    /// <summary>
    /// 全てのメッセージを順に再生する
    /// </summary>
    /// <param name="action"> 再生完了後のEvent </param>
    /// <returns></returns>
    public IEnumerator PlayAllMessageCoroutine(Action action = null)
    {
        foreach (var d in _data)
        {
            yield return StartCoroutine(FlowMessage(d, action));
        }
    }

    /// <summary>
    /// 指定したメッセージデータを再生する
    /// </summary>
    /// <param name="type"> 指定されたメッセージデータ </param>
    /// <param name="action"> 再生完了後のEvent </param>
    /// <returns></returns>
    public IEnumerator PlayMessageCorountine(MessageType type, Action action = null)
    {
        var d = _data.FirstOrDefault(m => m.MessageType == type);

        yield return StartCoroutine(FlowMessage(d, action));
    }

    /// <summary>
    /// メッセージを再生
    /// </summary>
    /// <param name="data"> メッセージのデータ </param>
    IEnumerator FlowMessage(ScenarioData data, Action action)
    {
        _bc.BackgroundChange(data.Background, () =>
        {
            _isChanged = true;
        });

        yield return new WaitUntil(() => _isChanged); //背景が切り替わるまで待機

        _isChanged = false;
        FadeMessageCanvas(1f, _messagePanelFadeTime);

        var d = data.Dialog;

        //メッセージを流す
        for (int i = 0; i < d.Length; i++)
        {
            _actorText.text = d[i].Actor;
            _messageText.text = "";
            _submitIcon.enabled = false;

            foreach (var message in d[i].AllMessage)
            {
                //メッセージを一文字ずつ表示
                foreach (var m in message)
                {
                    _messageText.text += m;
                    yield return new WaitForSeconds(_flowTime);
                }
                yield return new WaitForSeconds(0.5f);
            }
            

            _submitIcon.enabled = true; //入力を促すアイコンをアクティブにする
            yield return new WaitUntil(() => UIInput.Submit); //全て表示したらプレイヤーの入力を待機
        }

        FadeMessageCanvas(0f, _messagePanelFadeTime);
        action?.Invoke();
    }

    /// <summary>
    /// メッセージ画面の透過度を変更する
    /// </summary>
    /// <param name="value"> 変更後の値 </param>
    /// <param name="fadeTime"> 透過にかける時間 </param>
    void FadeMessageCanvas(float value, float fadeTime = 0f)
    {
        DOTween.To(() => _messagePanelCanvasGroup.alpha,
            x => _messagePanelCanvasGroup.alpha = x,
            value,
            fadeTime);
    }
}
