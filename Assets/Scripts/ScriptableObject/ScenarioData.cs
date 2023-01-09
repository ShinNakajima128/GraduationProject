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
        [Tooltip("メッセージの種類")]
        [SerializeField]
        MessageType _messageType = default;

        [Tooltip("メッセージデータ")]
        [SerializeField]
        DialogData[] _dialog = default;

        [Tooltip("メッセージ表示中の背景")]
        [SerializeField]
        Sprite _background = default;

        [Header("スプレッドシート情報")]
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
        /// スプレッドシートからシナリオデータをロードする。※この関数はEditor上でのみ使用する関数なので、ゲーム中に実行されるクラスでは使わないでください。
        /// </summary>
        /// <param name="url"> スプレッドシートのURL </param>
        /// <param name="sheetName"> シート名 </param>
        public void LoadDialogDataFromSpreadsheet(string url, string sheetName)
        {
            Debug.Log($"シート名:{sheetName}");
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
                //データがロードできなかった場合
                Debug.LogError($"データをロードできませんでした");
            }
        }
        /// <summary>
        /// シナリオデータを読み込む
        /// </summary>
        /// <typeparam name="T"> シナリオデータのクラス </typeparam>
        /// /// <param name="url"> スプレッドシートのURL </param>
        /// <param name="file"> シナリオの場面名 </param>
        /// <param name="callback"></param>
        void LoadMasterData<T>(string url, string file, LoadDataCallback<T> callback)
        {
            if (file == "")
            {
                Debug.LogError("シナリオデータの種類を指定してください");
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
        /// <summary> 話し手 </summary>
        public string Actor;
        /// <summary> メッセージ内容 </summary>
        [TextArea(1, 3)]
        public string Message;
    }
    /// <summary>
    /// メッセージの種類
    /// </summary>
    public enum MessageType
    {
        Intro,
        Tutorial,
        Stage1_End,
        Lobby_ClockEnd,
        Stage_Boss_Start,
        Stage_Boss_Down1,
        Stage_Boss_Down2,
        Stage_Boss_Down3,
        Ending,
        GetStill_Stage1,
        GetStill_Stage2,
        GetStill_Stage3,
        GetStill_Stage4,
        GetStill_Stage_Boss,
        Lobby_Visit,
        Lobby_CheshireCat,
        Stage2_Start_CheshireCat,
        FirstVisit_Stage1,
        FirstVisit_Stage2_Phase1,
        FirstVisit_Stage2_Phase2,
        FirstVisit_Stage2_Phase3,
        FirstVisit_Stage3_Phase1,
        FirstVisit_Stage3_Phase2,
        FirstVisit_Stage3_Phase3,
        FirstVisit_Stage3_Phase4,
        FirstVisit_Stage4_Phase1,
        FirstVisit_Stage4_Phase2,
        FirstVisit_Stage4_Phase3
    }
}

