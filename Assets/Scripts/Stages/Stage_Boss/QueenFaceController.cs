using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �����̕\����Ǘ�����R���|�[�l���g
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
    /// �\���ύX
    /// </summary>
    /// <param name="type"> �\��̎�� </param>
    public void ChangeFaceType(QueenFaceType type)
    {
        //�u���̎��͏u���p�̏��������s
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

        //�u���̏����̃��[�v
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

            //�����Ŏw�肵���u���̕b���̊ԁA������ҋ@
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
        //���p�N�̏����̃��[�v
        while (_isTalking)
        {
            var randomType = (QueenMouseType)UnityEngine.Random.Range(0, 3);

            yield return new WaitForSeconds(_mouseSwitchTime);

            ChangeMouseType(randomType);
        }
        //�I��������f�t�H���g�ɖ߂�
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
    Default, /// �f�t�H���g
    HerfEye, /// ����
    Close_Default, /// �ʏ�:�ڂ����
    Angry_Default, /// �{��:�ʏ�
    Angry_HerfEye, /// �{��:����
    Angry_Close, /// �{��:�ڂ����
    Damage, /// �_���[�W
    Smile, /// �Ί�
    NUM, /// �ڂ̎�ސ�
}
public enum QueenMouseType
{
    Default, /// �f�t�H���g
    Open_Small, /// �J��:��
    Open_Middle, /// �J��:��
    Open_Lauge, /// �J��:��
    Damage, ///
    Smile1, /// �Ί�1
    Smile2, /// �Ί�2
    NUM /// ���̎�ސ�
}
