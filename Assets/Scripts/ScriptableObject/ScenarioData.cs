using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AliceProject
{
    [CreateAssetMenu(menuName = "MyScriptable/Create ScenarioData")]
    public class ScenarioData : ScriptableObject
    {
        #region serialize
        [Tooltip("���b�Z�[�W�̎��")]
        [SerializeField]
        MessageType _messageType = default;

        [Tooltip("���b�Z�[�W�f�[�^")]
        [SerializeField]
        DialogData[] _dialog = default;

        [Tooltip("���b�Z�[�W�\�����̔w�i")]
        [SerializeField]
        Sprite _background = default;

        [Header("�X�v���b�h�V�[�g���")]
        [SerializeField]
        string m_spreadSheetURL = default;

        [SerializeField]
        string m_scenarioSheetName = default;
        #endregion
        #region private
        delegate void LoadDataCallback<T>(T data);
        #endregion
        #region property
        public MessageType MessageType => _messageType;
        public DialogData[] Dialog => _dialog;
        public Sprite Background => _background;
        public string URL => m_spreadSheetURL;
        public string ScenarioSheetName => m_scenarioSheetName;
        #endregion

        /// <summary>
        /// �X�v���b�h�V�[�g����V�i���I�f�[�^�����[�h����B�����̊֐���Editor��ł̂ݎg�p����֐��Ȃ̂ŁA�Q�[�����Ɏ��s�����N���X�ł͎g��Ȃ��ł��������B
        /// </summary>
        /// <param name="url"> �X�v���b�h�V�[�g��URL </param>
        /// <param name="sheetName"> �V�[�g�� </param>
        public void LoadDialogDataFromSpreadsheet(string url, string sheetName)
        {
            Debug.Log($"�V�[�g��:{sheetName}");
            try
            {
                LoadMasterData(url, sheetName, (ScenarioMasterDataClass<DialogData> data) =>
                {
                    _dialog = data.Data;
                    for (int i = 0; i < _dialog.Length; i++)
                    {
                        _dialog[i].MessagesToArray();
                    }
                });
            }
            catch
            {
                //�f�[�^�����[�h�ł��Ȃ������ꍇ
                Debug.LogError($"�f�[�^�����[�h�ł��܂���ł���");
            }
        }
        /// <summary>
        /// �V�i���I�f�[�^��ǂݍ���
        /// </summary>
        /// <typeparam name="T"> �V�i���I�f�[�^�̃N���X </typeparam>
        /// /// <param name="url"> �X�v���b�h�V�[�g��URL </param>
        /// <param name="file"> �V�i���I�̏�ʖ� </param>
        /// <param name="callback"></param>
        void LoadMasterData<T>(string url, string file, LoadDataCallback<T> callback)
        {
            if (file == "")
            {
                Debug.LogError("�V�i���I�f�[�^�̎�ނ��w�肵�Ă�������");
                return;
            }
            //https://script.google.com/macros/s/AKfycbxq24i-ssdzebp-MNLK59lllv42aTGyT6fwxqyH-1ytfeM5EcxDTTA3-am2-untkQY-/exec
            Network.WebRequest.Request<Network.WebRequest.GetString>(url + "?sheet=" + file, Network.WebRequest.ResultType.String, (string json) =>
            {
                Debug.Log(json);
                var dldata = JsonUtility.FromJson<T>(json);
                callback(dldata);
            //Debug.Log("Network download. : " + file + " / " + json + "/" + dldata);
        });
        }
    }

    [Serializable]
    public struct MessageText
    {
        /// <summary> �b���� </summary>
        public string Actor;
        /// <summary> ���b�Z�[�W���e </summary>
        [TextArea(1, 3)]
        public string Message;
    }
    /// <summary>
    /// ���b�Z�[�W�̎��
    /// </summary>
    public enum MessageType
    {
        Intro,
        Tutorial,
        Stage1_End,
        Stage_Boss_Start
    }
}

