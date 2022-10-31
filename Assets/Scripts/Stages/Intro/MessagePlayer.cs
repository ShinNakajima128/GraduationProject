using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// メッセージを再生するコンポーネント
/// </summary>
public class MessagePlayer : MonoBehaviour
{
    #region serialize
    [Tooltip("テキストデータ")]
    [SerializeField]
    TextData[] _data = default;

    [Tooltip("次の文字が表示されるまでの時間")]
    [SerializeField]
    float _flowTime = 0.05f;
    #endregion

    #region UI Objects
    [Tooltip("メッセージを表示するPanel")]
    [SerializeField]
    GameObject _messagePanel = default;

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
        _messagePanel.SetActive(false);
    }

    public IEnumerator PlayAllMessageCoroutine(Action action = null)
    {
        foreach (var d in _data)
        {
            yield return StartCoroutine(FlowMessage(d, action));
        }
    }

    public IEnumerator PlayMessageCorountine(MessageType type, Action action = null)
    {
        var d = _data.FirstOrDefault(m => m.MessageType == type);

        yield return StartCoroutine(FlowMessage(d, action));
    }

    /// <summary>
    /// メッセージを再生
    /// </summary>
    /// <param name="data"> メッセージのデータ </param>
    IEnumerator FlowMessage(TextData data, Action action)
    {
        _messagePanel.SetActive(false);

        _bc.BackgroundChange(data.Background, () =>
        {
            _isChanged = true;

            if (!_messagePanel.activeSelf)
            {
                _messagePanel.SetActive(true);
            }
        });

        yield return new WaitUntil(() => _isChanged); //背景が切り替わるまで待機

        _isChanged = false;

        var t = data.Texts;

        //メッセージを流す
        for (int i = 0; i < t.Length; i++)
        {
            _actorText.text = t[i].Actor;
            _messageText.text = "";

            //メッセージを一文字ずつ表示
            foreach (var m in t[i].Message)
            {
                _messageText.text += m;
                yield return new WaitForSeconds(_flowTime);
            }

            _submitIcon.enabled = true; //入力を促すアイコンをアクティブにする
            yield return new WaitUntil(() => UIInput.Submit); //全て表示したらプレイヤーの入力を待機
        }
    }
}
