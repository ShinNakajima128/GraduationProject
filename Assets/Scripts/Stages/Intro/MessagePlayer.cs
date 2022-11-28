using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// ���b�Z�[�W���Đ�����R���|�[�l���g
/// </summary>
public class MessagePlayer : MonoBehaviour
{
    #region serialize
    [Tooltip("�e�L�X�g�f�[�^")]
    [SerializeField]
    ScenarioData[] _data = default;

    [Tooltip("���̕������\�������܂ł̎���")]
    [SerializeField]
    float _flowTime = 0.05f;

    [Tooltip("���b�Z�[�W��ʂ̓��ߑ��x")]
    [SerializeField]
    float _messagePanelFadeTime = 0.3f;
    
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
    Image _submitIcon = default;

    [Tooltip("�w�i��ύX����R���|�[�l���g")]
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
        _bc.BackgroundChange(data.Background, () =>
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
            _messageText.text = "";
            _submitIcon.enabled = false;

            foreach (var message in d[i].AllMessage)
            {
                //���b�Z�[�W���ꕶ�����\��
                foreach (var m in message)
                {
                    _messageText.text += m;
                    yield return new WaitForSeconds(_flowTime);
                }
                yield return new WaitForSeconds(0.5f);
            }
            

            _submitIcon.enabled = true; //���͂𑣂��A�C�R�����A�N�e�B�u�ɂ���
            yield return new WaitUntil(() => UIInput.Submit); //�S�ĕ\��������v���C���[�̓��͂�ҋ@
        }

        FadeMessageCanvas(0f, _messagePanelFadeTime);
        action?.Invoke();
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
}
