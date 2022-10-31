using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ���b�Z�[�W���Đ�����R���|�[�l���g
/// </summary>
public class MessagePlayer : MonoBehaviour
{
    #region serialize
    [Tooltip("�e�L�X�g�f�[�^")]
    [SerializeField]
    TextData[] _data = default;

    [Tooltip("���̕������\�������܂ł̎���")]
    [SerializeField]
    float _flowTime = 0.05f;
    #endregion

    #region UI Objects
    [Tooltip("���b�Z�[�W��\������Panel")]
    [SerializeField]
    GameObject _messagePanel = default;

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
    /// ���b�Z�[�W���Đ�
    /// </summary>
    /// <param name="data"> ���b�Z�[�W�̃f�[�^ </param>
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

        yield return new WaitUntil(() => _isChanged); //�w�i���؂�ւ��܂őҋ@

        _isChanged = false;

        var t = data.Texts;

        //���b�Z�[�W�𗬂�
        for (int i = 0; i < t.Length; i++)
        {
            _actorText.text = t[i].Actor;
            _messageText.text = "";

            //���b�Z�[�W���ꕶ�����\��
            foreach (var m in t[i].Message)
            {
                _messageText.text += m;
                yield return new WaitForSeconds(_flowTime);
            }

            _submitIcon.enabled = true; //���͂𑣂��A�C�R�����A�N�e�B�u�ɂ���
            yield return new WaitUntil(() => UIInput.Submit); //�S�ĕ\��������v���C���[�̓��͂�ҋ@
        }
    }
}
