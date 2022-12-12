using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using AliceProject;


/// <summary>
/// メッセージを再生するコンポーネント
/// </summary>
public class MessagePlayer : MonoBehaviour
{
    #region serialize
    [Tooltip("次の文字が表示されるまでの時間")]
    [SerializeField]
    float _flowTime = 0.05f;

    [Tooltip("次の行が表示されるまでの時間")]
    [SerializeField]
    float _nextMessageInterval = 0.7f;

    [Tooltip("メッセージ画面の透過速度")]
    [SerializeField]
    float _messagePanelFadeTime = 0.3f;

    [Tooltip("テキストデータ")]
    [SerializeField]
    ScenarioData[] _data = default;
    
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
    GameObject _submitIcon = default;

    [Tooltip("話し手のアイコン")]
    [SerializeField]
    Image _actorIconImage = default;

    [Tooltip("背景を変更するコンポーネント")]
    [SerializeField]
    BackgroundChanger _bc = default;

    [Header("IconData")]
    [SerializeField]
    IconData _iconData = default;
    #endregion

    #region private
    bool _isChanged = false;
    bool _isPressed = false;
    #endregion
    public event Action CameraShake;
    public event Action ConcentratedLine;
    public event Action Closeup;
    public event Action Reset;
    #region property
    public static MessagePlayer Instance { get; private set; }
    #endregion

    private void Awake()
    {
        Instance = this;
    }

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
        _bc.BackgroundChange(data, () =>
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
            _actorIconImage.sprite = GetActorIcon(d[i].Actor);
            _messageText.text = "";
            OnScreenEffect(d[i].EffectType);

            foreach (var message in d[i].AllMessage)
            {
                _submitIcon.SetActive(false);

                //メッセージを一文字ずつ表示
                foreach (var m in message)
                {
                    _messageText.text += m;
                    yield return new WaitForSeconds(_flowTime);
                }

                yield return new WaitForSeconds(_nextMessageInterval);
                
                _messageText.text += "\n";
            }

            _submitIcon.SetActive(true); //入力を促すアイコンをアクティブにする

            yield return new WaitUntil(() => UIInput.Submit); //全て表示したらプレイヤーの入力を待機
        }

        _submitIcon.SetActive(false);
        
        OnScreenEffect(ScreenEffectType.Reset);
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

    void OnScreenEffect(ScreenEffectType type)
    {
        switch (type)
        {
            case ScreenEffectType.CameraShake:
                CameraShake?.Invoke();
                break;
            case ScreenEffectType.ConcentratedLine:
                ConcentratedLine?.Invoke();
                break;
            case ScreenEffectType.Closeup:
                Closeup?.Invoke();
                break;
            case ScreenEffectType.Reset:
                Reset?.Invoke();
                break;
            default:
                break;
        }
    }

    Sprite GetActorIcon(string actorName)
    {
        Sprite icon = default;

        switch (actorName)
        {
            case "アリス":
                icon = _iconData.Actors.FirstOrDefault(x => x.Actor == Actor.Alice).Icon;
                break;
            case "チェシャ猫":
                icon = _iconData.Actors.FirstOrDefault(x => x.Actor == Actor.CheshireCat).Icon;
                break;
            case "ウサギ":
                icon = _iconData.Actors.FirstOrDefault(x => x.Actor == Actor.Rabbit).Icon;
                break;
            case "女王":
                icon = _iconData.Actors.FirstOrDefault(x => x.Actor == Actor.Queen).Icon;
                break;
            default:
                Debug.LogError("名前の指定が間違っています");
                break;
        }
        return icon;
    }
}

/// <summary>
/// 話し手の種類
/// </summary>
public enum Actor
{
    /// <summary> アリス </summary>
    Alice,
    /// <summary> チェシャ猫 </summary>
    CheshireCat,
    /// <summary> ウサギ </summary>
    Rabbit,
    /// <summary> 女王(ボス) </summary>
    Queen
}
