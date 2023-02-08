using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using AliceProject;


/// <summary>
/// ���b�Z�[�W���Đ�����R���|�[�l���g
/// </summary>
public class MessagePlayer : MonoBehaviour
{
    #region serialize
    [Tooltip("���̕������\�������܂ł̎���")]
    [SerializeField]
    float _flowTime = 0.05f;

    [Tooltip("���̍s���\�������܂ł̎���")]
    [SerializeField]
    float _nextMessageInterval = 0.7f;

    [Tooltip("���b�Z�[�W��ʂ̓��ߑ��x")]
    [SerializeField]
    float _messagePanelFadeTime = 0.3f;

    [Tooltip("�e�L�X�g�f�[�^")]
    [SerializeField]
    ScenarioData[] _data = default;
    
    [Header("UI")]
    [Tooltip("���b�Z�[�W��\������Panel��CanvasGroup")]
    [SerializeField]
    CanvasGroup _messagePanelCanvasGroup = default;

    [Tooltip("���b�Z�[�W�̘b����")]
    [SerializeField]
    Text _actorText = default;

    [Tooltip("���b�Z�[�W���e")]
    [SerializeField]
    Text _messageText = default;

    [Tooltip("���͑ҋ@���̃A�C�R��")]
    [SerializeField]
    GameObject _submitIcon = default;

    [Tooltip("�b����̃A�C�R��")]
    [SerializeField]
    Image _actorIconImage = default;

    [Tooltip("�w�i��ύX����R���|�[�l���g")]
    [SerializeField]
    BackgroundChanger _bc = default;

    [Header("IconData")]
    [SerializeField]
    IconData _iconData = default;
    #endregion

    #region private
    bool _isChanged = false;
    bool _isSkiped = false;
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
        EventManager.ListenEvents(Events.CameraShake, CameraShake);
        EventManager.ListenEvents(Events.ConcentratedLine, ConcentratedLine);
        EventManager.ListenEvents(Events.Closeup, Closeup);
        EventManager.ListenEvents(Events.Reset, Reset);
    }

    /// <summary>
    /// �S�Ẵ��b�Z�[�W�����ɍĐ�����
    /// </summary>
    /// <param name="action"> �Đ��������Event </param>
    /// <returns></returns>
    public IEnumerator PlayAllMessageCoroutine(Action action = null)
    {
        foreach (var d in _data)
        {
            yield return StartCoroutine(FlowMessage(d, action));
        }
    }

    /// <summary>
    /// �w�肵�����b�Z�[�W�f�[�^���Đ�����
    /// </summary>
    /// <param name="type"> �w�肳�ꂽ���b�Z�[�W�f�[�^ </param>
    /// <param name="action"> �Đ��������Event </param>
    /// <returns></returns>
    public IEnumerator PlayMessageCorountine(MessageType type, Action action = null)
    {
        var d = _data.FirstOrDefault(m => m.MessageType == type);

        yield return StartCoroutine(FlowMessage(d, action));
    }

    /// <summary>
    /// ���b�Z�[�W���Đ�
    /// </summary>
    /// <param name="data"> ���b�Z�[�W�̃f�[�^ </param>
    IEnumerator FlowMessage(ScenarioData data, Action action)
    {
        _bc.BackgroundChange(data, () =>
        {
            _isChanged = true;
        });

        yield return new WaitUntil(() => _isChanged); //�w�i���؂�ւ��܂őҋ@

        _isChanged = false;
        FadeMessageCanvas(1f, _messagePanelFadeTime);

        var d = data.Dialog;

        //���b�Z�[�W�𗬂�
        for (int i = 0; i < d.Length; i++)
        {
            _actorText.text = d[i].Actor;
            _actorIconImage.sprite = GetActorIcon(d[i].Actor);

            if (d[i].Actor == "�H�H�H")
            {
                _actorIconImage.color = Color.black;
            }
            else
            {
                _actorIconImage.color = Color.white;
            }
            _messageText.text = "";
            _messageText.fontSize = d[i].FontSize;

            if (d[i].EventType != Events.None)
            {
                EventManager.OnEvent(d[i].EventType);
                print($"{d[i].EventType}");
            }
            //OnScreenEffect(d[i].EventType);

            foreach (var message in d[i].AllMessage)
            {
                _submitIcon.SetActive(false);

                //���b�Z�[�W���ꕶ�����\��
                foreach (var m in message)
                {
                    _messageText.text += m;

                    if (!_isSkiped)
                    {
                        yield return WaitTimer(_flowTime);
                    }                    
                }

                if (!_isSkiped)
                {
                    yield return new WaitForSeconds(_nextMessageInterval);
                }
                _messageText.text += "\n";
            }
            _isSkiped = false;

            EventManager.OnEvent(Events.FinishTalking);

            yield return new WaitForSeconds(d[i].DisplayTime);

            _submitIcon.SetActive(true); //���͂𑣂��A�C�R�����A�N�e�B�u�ɂ���

            yield return new WaitUntil(() => UIInput.Submit); //�S�ĕ\��������v���C���[�̓��͂�ҋ@
            AudioManager.PlaySE(SEType.UI_CursolMove);

            yield return new WaitForSeconds(0.05f);
        }

        _submitIcon.SetActive(false);
        
        OnScreenEffect(ScreenEffectType.Reset);
        FadeMessageCanvas(0f, _messagePanelFadeTime);
        action?.Invoke();
    }

    /// <summary>
    /// �w�肵�����ԑҋ@����
    /// </summary>
    /// <param name="time"> �҂��� </param>
    /// <returns></returns>
    IEnumerator WaitTimer(float time)
    {
        float timer = 0;

        while (true)
        {
            timer += Time.deltaTime;

            if (timer >= time)
            {
                yield break;
            }
            else if (UIInput.Submit)
            {
                _isSkiped = true;
                //AudioManager.PlaySE(SEType.UI_CursolMove);
                yield return null;
                yield break;
            }
            yield return null;
        }
    }

    /// <summary>
    /// ���b�Z�[�W��ʂ̓��ߓx��ύX����
    /// </summary>
    /// <param name="value"> �ύX��̒l </param>
    /// <param name="fadeTime"> ���߂ɂ����鎞�� </param>
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
            case "�A���X":
                icon = _iconData.Actors.FirstOrDefault(x => x.Actor == Actor.Alice).Icon;
                break;
            case "�`�F�V���L":
                icon = _iconData.Actors.FirstOrDefault(x => x.Actor == Actor.CheshireCat).Icon;
                break;
            case "�E�T�M":
                icon = _iconData.Actors.FirstOrDefault(x => x.Actor == Actor.Rabbit).Icon;
                break;
            case "�n�[�g�̏���":
                icon = _iconData.Actors.FirstOrDefault(x => x.Actor == Actor.Queen).Icon;
                break;
            case "�H�H�H":
                if (GameManager.Instance.CurrentScene == SceneType.Lobby ||
                    GameManager.Instance.CurrentScene == SceneType.Stage1_Fall)
                {
                    icon = _iconData.Actors.FirstOrDefault(x => x.Actor == Actor.CheshireCat).Icon;
                }
                else
                {
                    icon = _iconData.Actors.FirstOrDefault(x => x.Actor == Actor.Queen).Icon;
                }
                break;
            default:
                Debug.LogError("���O�̎w�肪�Ԉ���Ă��܂�");
                break;
        }
        return icon;
    }
}

/// <summary>
/// �b����̎��
/// </summary>
public enum Actor
{
    /// <summary> �A���X </summary>
    Alice,
    /// <summary> �`�F�V���L </summary>
    CheshireCat,
    /// <summary> �E�T�M </summary>
    Rabbit,
    /// <summary> ����(�{�X) </summary>
    Queen
}
