using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 女王の表情を管理するコンポーネント
/// </summary>
public class QueenFaceController : MonoBehaviour
{
    #region serialize
    [Header("variables")]
    [SerializeField]
    QueenFaceType _testFaceType = default;

    [SerializeField]
    [Range(3.0f, 5.0f)]
    int _blinkMaxInterval = 5;

    [SerializeField]
    float _blinkSwitchTime = 0.03f;

    [SerializeField]
    float _mouseSwitchTime = 0.1f;

    [SerializeField]
    float _testInterval = 1.5f;

    [SerializeField]
    QueenFace[] _faceTypes = default;

    [Header("Renderer")]
    [SerializeField]
    Renderer _faceRenderer = default;
    #endregion

    #region private
    Dictionary<QueenEyeType, Vector2> _eyeTypeDic = new Dictionary<QueenEyeType, Vector2>();
    Dictionary<QueenMouseType, Vector2> _mouseTypeDic = new Dictionary<QueenMouseType, Vector2>();
    Material _eyeMat;
    Material _mouseMat;
    bool _isBlinking = false;
    bool _isTalking = false;
    bool _init = false;
    Coroutine _blinkCoroutine;
    Coroutine _talkCoroutine;
    #endregion
    #region property
    public bool IsTalking => _isTalking;
    #endregion

    private void OnValidate()
    {
        if (_init)
        {
            ChangeFaceType(_testFaceType);
        }
    }
    private void Awake()
    {
        int eyeCount = 0;

        for (int i = 0; i < 4; i++)
        {
            for (int n = 0; n < 2; n++)
            {
                _eyeTypeDic.Add((QueenEyeType)eyeCount, new Vector2(n * 0.5f, i * -0.25f));
                eyeCount++;
            }
        }

        int mouseCount = 0;

        for (int i = 0; i < 4; i++)
        {
            for (int n = 0; n < 2; n++)
            {
                if (mouseCount >= (int)QueenMouseType.NUM)
                {
                    break;
                }

                _mouseTypeDic.Add((QueenMouseType)mouseCount, new Vector2(n * 0.5f, i * -0.25f));
                mouseCount++;
            }
        }
        _eyeMat = _faceRenderer.materials[1];
        _mouseMat = _faceRenderer.materials[2];
        _init = true;
    }

    /// <summary>
    /// 表情を変更
    /// </summary>
    /// <param name="type"> 表情の種類 </param>
    public void ChangeFaceType(QueenFaceType type)
    {
        //瞬きの時は瞬き用の処理を実行
        if (type == QueenFaceType.Blink)
        {
            _isBlinking = true;

            if (_blinkCoroutine != null)
            {
                StopCoroutine(_blinkCoroutine);
                _blinkCoroutine = null;
            }
            _blinkCoroutine = StartCoroutine(BlinkCoroutine());
        }
        else if (type == QueenFaceType.Talking)
        {
            _isTalking = true;

            if (_talkCoroutine != null)
            {
                StopCoroutine(_talkCoroutine);
                _talkCoroutine = null;
            }
            _talkCoroutine = StartCoroutine(TalkingCoroutine());
        }
        else
        {
            _isBlinking = false;
            var face = _faceTypes.FirstOrDefault(f => f.FaceType == type);

            ChangeEyeType(face.EyeType);
            ChangeMouseType(face.MouseType);
        }
    }

    public void FinishTalk()
    {
        _isTalking = false;
    }

    void ChangeEyeType(QueenEyeType type)
    {
        _eyeMat.SetTextureOffset("_MainTex", GetEyeOffset(type));
        _faceRenderer.materials[1] = _eyeMat;
    }

    void ChangeMouseType(QueenMouseType type)
    {
        _mouseMat.SetTextureOffset("_MainTex", GetMouseOffset(type));
        _faceRenderer.materials[1] = _mouseMat;
    }

    Vector2 GetEyeOffset(QueenEyeType type)
    {
        var v = _eyeTypeDic.FirstOrDefault(x => x.Key == type).Value;

        return v;
    }
    Vector2 GetMouseOffset(QueenMouseType type)
    {
        var v = _mouseTypeDic.FirstOrDefault(x => x.Key == type).Value;

        return v;
    }

    IEnumerator BlinkCoroutine()
    {
        float timer;
        float rand;

        ChangeMouseType(QueenMouseType.Default);

        //瞬きの処理のループ
        while (_isBlinking)
        {
            yield return new WaitForSeconds(_blinkSwitchTime);

            ChangeEyeType(QueenEyeType.HerfEye);

            yield return new WaitForSeconds(_blinkSwitchTime);

            ChangeEyeType(QueenEyeType.Close_Default);

            yield return new WaitForSeconds(_blinkSwitchTime);

            ChangeEyeType(QueenEyeType.HerfEye);

            yield return new WaitForSeconds(_blinkSwitchTime);

            ChangeEyeType(QueenEyeType.Default);

            //ここで指定した瞬きの秒数の間、処理を待機
            timer = 0;
            rand = UnityEngine.Random.Range(3, _blinkMaxInterval);
            while (_isBlinking && timer < rand)
            {
                timer += Time.deltaTime;
                yield return null;
            }
        }
    }

    IEnumerator TalkingCoroutine()
    {
        //口パクの処理のループ
        while (_isTalking)
        {
            var randomType = (QueenMouseType)UnityEngine.Random.Range(0, 3);

            yield return new WaitForSeconds(_mouseSwitchTime);

            ChangeMouseType(randomType);
        }
        //終了したらデフォルトに戻す
        ChangeMouseType(QueenMouseType.Default);
    }
}

[Serializable]
public struct QueenFace
{
    public QueenFaceType FaceType;
    public QueenEyeType EyeType;
    public QueenMouseType MouseType;
}

public enum QueenFaceType
{
    Default,
    Blink,
    Smile,
    Damage,
    Angry,
    Talking,
    CloseEyes
}

public enum QueenEyeType
{
    Default, /// デフォルト
    HerfEye, /// 半目
    Close_Default, /// 通常:目を閉じる
    Angry_Default, /// 怒り:通常
    Angry_HerfEye, /// 怒り:半目
    Angry_Close, /// 怒り:目を閉じる
    Damage, /// ダメージ
    Smile, /// 笑顔
    NUM, /// 目の種類数
}
public enum QueenMouseType
{
    Default, /// デフォルト
    Open_Small, /// 開く:小
    Open_Middle, /// 開く:中
    Open_Lauge, /// 開く:大
    Damage, ///
    Smile1, /// 笑顔1
    Smile2, /// 笑顔2
    NUM /// 口の種類数
}
