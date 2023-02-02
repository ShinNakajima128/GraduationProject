using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public enum BGMType
{
    /// <summary> タイトル画面 </summary>
    Title,
    Intro,
    Lobby,
    Lobby_MeetingCheshire,
    Stage1,
    Stage2,
    Stage3,
    Stage4,
    Boss_Before,
    Boss_InGame,
    Ending,
    /// <summary> ゲームオーバー </summary>
    Gameover,
    ClearJingle,
    GetStill,
    BossStage_Clear,
    UnderLobby
}
public enum SEType
{
    /// <summary> UI:選択音 </summary>
    UI_Select,
    /// <summary> UI:キャンセル音 </summary>
    UI_Cancel,
    /// <summary> UI:ロード音 </summary>
    UI_Load,
    /// <summary> UI:カーソル移動 </summary>
    UI_CursolMove,
    /// <summary> UI:画面遷移音 </summary>
    UI_Transition,
    /// <summary> UI:ボタン選択 </summary>
    UI_ButtonSelect,
    /// <summary> プレイヤー:足音 </summary>
    Player_FootStep,
    /// <summary> プレイヤー:ダメージ </summary>
    Player_Damage,
    /// <summary> プレイヤー:回復音 </summary>
    Player_Heal,
    /// <summary> プレイヤー:金平糖を獲得 </summary>
    Player_GetItem,
    /// <summary> 敵全般:被弾 </summary>
    Enemy_Damage,
    /// <summary> 敵全般:消滅 </summary>
    Enemy_Vanish,
    /// <summary> アイテム:獲得 </summary>
    Item_Get,
    /// <summary> アイテム:カウントアップ </summary>
    Item_Countup,
    /// <summary> アイテム:紙が飛び散る </summary>
    Object_Scatter,
    /// <summary> ネズミ発見 </summary>
    Finding,
    /// <summary> ゲームオーバージングル </summary>
    Gameover_Jingle,
    /// <summary> ステージに進む </summary>
    GoToStage,
    /// <summary> タイトル：ゲームスタート </summary>
    UI_GameStart,
    /// <summary> ステージ1の落下音 </summary>
    Stage1_Fall,
    Lobby_FirstVisit,
    Lobby_MeetingCheshire,
    Lobby_OpenAlbum,
    Lobby_CloseAlbum,
    Lobby_NearDoor,
    Lobby_ClockMove,
    Lobby_StopClock,
    Lobby_OnTutorial,
    UI_CannotSelect,
    Player_Landing,
    Stage2_Correct,
    Stage2_Wrong,
    Stage2_OpenCup,
    Stage2_CloseCup,
    Stage2_Shuffle,
    Stage2_Warp,
    Stage3_Swing,
    Stage3_Shot,
    Stage3_OpenOrder,
    Stage3_CloseOrder,
    Stage3_Goal,
    Stage3_Success,
    Stage3_Failure,
    Stage3_BlowTrump,
    Stage3_heeloverOrder,
    Stage3_QueenStump,
    Stage4_Question1,
    Stage4_Question2,
    BossStage_DebrisLanding,
    BossStage_QueenLanding,
    BossStage_Down,
    Alice_Landing,
    Trump_Slust, //トランプ兵：突き攻撃
    Trump_Waiting,
    UnderLobby_Lowering, //地下：下降音
    UnderLobby_Arrival, //地下：到着
    BossStage_QueenQuiet, //おだまりSE
    BossStage_QueenAppearance, //女王登場
    BossStage_QueenSwing, //女王が杖を振る
    Trump_AttackEnd, //トランプ兵が槍を戻す音
    Trump_Alignment, //トランプ兵の整列音
    BossStage_QueenDamage, //女王がダメージを受ける
    BossStage_QueenJump, //女王がジャンプした時
    BossStage_QueenFallDown //女王が落ち始めた時
}

public enum VOICEType
{
    /// <summary> ダメージ </summary>
    Damage
}

/// <summary>
/// オーディオ機能を管理するコンポーネント
/// </summary>
public class AudioManager : SingletonMonoBehaviour<AudioManager>
{
    [Header("各音量")]
    [SerializeField, Range(0f, 1f)]
    float _masterVolume = 1.0f;

    [SerializeField, Range(0f, 1f)]
    float _bgmVolume = 0.3f;

    [SerializeField, Range(0f, 1f)]
    float _seVolume = 1.0f;

    [SerializeField, Range(0f, 1f)]
    float _voiceVolume = 1.0f;

    [Header("AudioSourceの生成数")]
    [Tooltip("SEのAudioSourceの生成数")]
    [SerializeField]
    int _seAudioSourceNum = 5;

    [Tooltip("SEのAudioSourceの生成数")]
    [SerializeField]
    int _voiceAudioSourceNum = 5;

    [Header("各音源リスト")]
    [SerializeField]
    List<BGM> _bgmList = new List<BGM>();

    [SerializeField]
    List<SE> _seList = new List<SE>();

    [SerializeField]
    List<VOICE> _voiceList = new List<VOICE>();

    [Header("使用する各オブジェクト")]
    [Tooltip("BGM用のAudioSource")]
    [SerializeField]
    AudioSource _bgmSource = default;

    [Tooltip("SE用のAudioSourceをまとめるオブジェクト")]
    [SerializeField]
    Transform _seSourcesParent = default;

    [Tooltip("ボイス用のAudioSourceをまとめるオブジェクト")]
    [SerializeField]
    Transform _voiceSourcesParent = default;

    [Tooltip("AudioMixer")]
    [SerializeField]
    AudioMixerGroup _mixer = default;

    List<AudioSource> _seAudioSourceList = new List<AudioSource>();
    List<AudioSource> _voiceAudioSourceList = new List<AudioSource>();
    bool _isStoping = false;

    void Awake()
    {
        if (this != Instance)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);

        //指定した数のSE用AudioSourceを生成
        for (int i = 0; i < _seAudioSourceNum; i++)
        {
            //SEAudioSourceのオブジェクトを生成し、親オブジェクトにセット
            var obj = new GameObject($"SESource{i + 1}");
            obj.transform.SetParent(_seSourcesParent);

            //生成したオブジェクトにAudioSourceを追加
            var source = obj.AddComponent<AudioSource>();
            
            _seAudioSourceList.Add(source);
        }

        //指定した数のボイス用AudioSourceを生成
        for (int i = 0; i < _voiceAudioSourceNum; i++)
        {
            //VOICEAudioSourceのオブジェクトを生成し、親オブジェクトにセット
            var obj = new GameObject($"VOICESource{i + 1}");
            obj.transform.SetParent(_voiceSourcesParent);

            //生成したオブジェクトにAudioSourceを追加
            var source = obj.AddComponent<AudioSource>();

            _voiceAudioSourceList.Add(source);
        }
    }

    #region play method
    /// <summary>
    /// BGMを再生
    /// </summary>
    /// <param name="type"> BGMの種類 </param>
    public static void PlayBGM(BGMType type, bool loopType = true)
    {
        var bgm = GetBGM(type);

        if (bgm != null)
        {
            if (Instance._bgmSource.clip == null)
            {
                Instance._bgmSource.clip = bgm.Clip;
                Instance._bgmSource.loop = loopType;
                Instance._bgmSource.volume = Instance._bgmVolume * Instance._masterVolume;
                Instance._bgmSource.Play();
                Debug.Log($"{bgm.BGMName}を再生");

            }
            else
            {
                Instance.StartCoroutine(Instance.SwitchingBgm(bgm, loopType));
                Debug.Log($"{bgm.BGMName}を再生");
            }

        }
        else
        {
            Debug.LogError($"BGM:{type}を再生できませんでした");
        }
    }

    /// <summary>
    /// SEを再生
    /// </summary>
    /// <param name="type"> SEの種類 </param>
    public static void PlaySE(SEType type)
    {
        var se = GetSE(type);

        if (se != null)
        {
            foreach (var s in Instance._seAudioSourceList)
            {
                if (!s.isPlaying)
                {
                    s.PlayOneShot(se.Clip, Instance._seVolume * Instance._masterVolume * se.Volume);
                    //Debug.Log($"{se.SEName}を再生");
                    return;
                }
            }
        }
        else
        {
            Debug.LogError($"SE:{type}を再生できませんでした");
        }
    }

    /// <summary>
    /// ボイスを再生
    /// </summary>
    /// <param name="type"> ボイスの種類 </param>
    public static void PlayVOICE(VOICEType type)
    {
        var voice = GetVOICE(type);

        if (voice != null)
        {
            foreach (var s in Instance._voiceAudioSourceList)
            {
                if (!s.isPlaying)
                {
                    s.PlayOneShot(voice.Clip, Instance._voiceVolume * Instance._masterVolume * voice.Volume);
                    //Debug.Log($"{voice.VOICEName}を再生");
                    return;
                }
            }
        }
        else
        {
            Debug.LogError($"VOICE:{type}を再生できませんでした");
        }
    }
    #endregion

    #region stop method
    /// <summary>
    /// 再生中のBGMを停止する
    /// </summary>
    public static void StopBGM()
    {
        Instance._bgmSource.Stop();
        Instance._bgmSource.clip = null;
    }

    /// <summary>
    /// 再生中のBGMの音量徐々に下げて停止する
    /// </summary>
    /// <param name="stopTime"> 停止するまでの時間 </param>
    public static void StopBGM(float stopTime)
    {
        Instance.StartCoroutine(Instance.LowerVolume(stopTime));
    }
    /// <summary>
    /// 再生中のSEを停止する
    /// </summary>
    public static void StopSE()
    {
        foreach (var s in Instance._seAudioSourceList)
        {
            s.Stop();
            s.clip = null;
        }
    }
    /// <summary>
    /// 再生中のボイスを停止する
    /// </summary>
    public void StopVOICE()
    {
        foreach (var s in Instance._voiceAudioSourceList)
        {
            s.Stop();
            s.clip = null;
        }
    }
    #endregion
    #region volume Method
    /// <summary>
    /// マスター音量を変更する
    /// </summary>
    /// <param name="masterValue"> 音量 </param>
    public static void MasterVolChange(float masterValue)
    {
        Instance._masterVolume = masterValue;
        Instance._bgmSource.volume = Instance._bgmVolume * Instance._masterVolume;
    }

    /// <summary>
    /// BGM音量を変更する
    /// </summary>
    /// <param name="bgmValue"> 音量 </param>
    public static void BgmVolChange(float bgmValue)
    {
        Instance._bgmVolume = bgmValue;
        Instance._bgmSource.volume = Instance._bgmVolume * Instance._masterVolume;
    }

    /// <summary>
    /// SE音量を変更する
    /// </summary>
    /// <param name="seValue"> 音量 </param>
    public static void SeVolChange(float seValue)
    {
        Instance._seVolume = seValue;
        foreach (var s in Instance._seAudioSourceList)
        {
            s.volume = Instance._seVolume;
        }
    }

    /// <summary>
    /// ボイス音量を変更する
    /// </summary>
    /// <param name="voiceValue"> 音量 </param>
    public static void VoiceVolChange(float voiceValue)
    {
        Instance._voiceVolume = voiceValue;
        foreach (var v in Instance._voiceAudioSourceList)
        {
            v.volume = Instance._voiceVolume;
        }
    }

    /// <summary>
    /// 各音量をセットする
    /// </summary>
    /// <param name="data"> サウンドデータ </param>
    //public static void SetVolume(SoundOption data)
    //{
    //    Instance._masterVolume = data.MasterVolume;
    //    Instance._bgmVolume = data.BgmVolume;
    //    Instance._seVolume = data.SeVolume;
    //    Instance._voiceVolume = data.VoiceVolume;
    //}

    /// <summary>
    /// BGMを徐々に変更する
    /// </summary>
    /// <param name="afterBgm"> 変更後のBGM </param>
    IEnumerator SwitchingBgm(BGM afterBgm, bool loopType = true)
    {
        _isStoping = false;
        float currentVol = _bgmSource.volume;

        while (_bgmSource.volume > 0)　//現在の音量を0にする
        {
            _bgmSource.volume -= Time.deltaTime * 1.5f;
            yield return null;
        }

        _bgmSource.clip = afterBgm.Clip;　//BGMの入れ替え
        _bgmSource.loop = loopType;
        _bgmSource.Play();

        while (_bgmSource.volume < currentVol)　//音量を元に戻す
        {
            _bgmSource.volume += Time.deltaTime * 1.5f;
            yield return null;
        }
        _bgmSource.volume = currentVol;
    }

    /// <summary>
    /// 音量を徐々に下げて停止するコルーチン
    /// </summary>
    /// <param name="time"> 停止するまでの時間 </param>
    IEnumerator LowerVolume(float time)
    {
        float currentVol = _bgmSource.volume;
        _isStoping = true;
        
        while (_bgmSource.volume > 0)　//現在の音量を0にする
        {
            _bgmSource.volume -= Time.deltaTime * currentVol / time;

            //途中でBGM等が変更された場合は処理を中断
            if (!_isStoping)
            {
                yield break;
            }
            yield return null;
        }

        _isStoping = false;
        Instance._bgmSource.Stop();
        Instance._bgmSource.clip = null;
    }
    #endregion

    #region get method
    /// <summary>
    /// BGMを取得
    /// </summary>
    /// <param name="type"> BGMの種類 </param>
    /// <returns> 指定したBGM </returns>
    static BGM GetBGM(BGMType type)
    {
        var bgm = Instance._bgmList.FirstOrDefault(b => b.BGMType == type);
        return bgm;
    }
    /// <summary>
    /// SEを取得
    /// </summary>
    /// <param name="type"> SEの種類 </param>
    /// <returns> 指定したSE </returns>
    static SE GetSE(SEType type)
    {
        var se = Instance._seList.FirstOrDefault(s => s.SEType == type);
        return se;
    }
    /// <summary>
    /// ボイスを取得
    /// </summary>
    /// <param name="type"> ボイスの種類 </param>
    /// <returns> 指定したボイス </returns>
    static VOICE GetVOICE(VOICEType type)
    {
        var voice = Instance._voiceList.FirstOrDefault(v => v.VOICEType == type);
        return voice;
    }
    #endregion
}

[Serializable]
public class BGM
{
    public string BGMName;
    public BGMType BGMType;
    public AudioClip Clip;
}
[Serializable]
public class SE
{
    public string SEName;
    public SEType SEType;
    public AudioClip Clip;
    [Range(0,1)]
    public float Volume = 1f;
}
[Serializable]
public class VOICE
{
    public string VOICEName;
    public VOICEType VOICEType;
    public AudioClip Clip;
    [Range(0, 1)]
    public float Volume = 1f;
}
