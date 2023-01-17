using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public enum FaceType
{
    Default,
    Blink,
    Smile,
    Damage,
    Angry,
    Fancy,
    Cry,
    Rotation
}

public enum EyeType
{
    Default, /// �f�t�H���g
    HerfEye, /// ����
    Close_Default, /// ����:�ʏ�
    Close_Smile, /// ����:�Ί�
    Close_Damage, /// ����:�_���\�W
    Fancy, /// ��z�A����
    Angry, /// �{��
    Close_Cry, /// ����:�߂���
    NUM, /// �ڂ̎�ސ�
}
public enum MouseType
{
    Default, /// �f�t�H���g
    Negative, /// �s���A�l�K�e�B�u
    Surprise, /// ����
    Smile, /// �Ί�
    Thinking, /// �l����
    NUM /// ���̎�ސ�
}
public class AliceFaceController : MonoBehaviour
{
    #region serialize
    [Header("variables")]
    [SerializeField]
    FaceType _testFaceType = default;

    [SerializeField]
    [Range(3.0f, 5.0f)]
    int _blinkMaxInterval = 5;

    [SerializeField]
    float _blinkSwitchTime = 0.03f;

    [SerializeField]
    float _testInterval = 1.5f;

    [SerializeField]
    Face[] _faceTypes = default;

    [Header("Renderer")]
    [SerializeField]
    Renderer _faceRenderer = default;
    #endregion

    #region private
    Dictionary<EyeType, Vector2> _eyeTypeDic = new Dictionary<EyeType, Vector2>();
    Dictionary<MouseType, Vector2> _mouseTypeDic = new Dictionary<MouseType, Vector2>();
    Material _eyeMat;
    Material _mouseMat;
    bool _isBlinking = false;
    bool _init = false;
    Coroutine _blinkCoroutine;
    #endregion
    #region property
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
                _eyeTypeDic.Add((EyeType)eyeCount, new Vector2(n * 0.5f, i * - 0.25f));
                eyeCount++;
            }
        }
        
        int mouseCount = 0;
        
        for (int i = 0; i < 4; i++)
        {
            for (int n = 0; n < 2; n++)
            {
                if (mouseCount >= (int)MouseType.NUM)
                {
                    break;
                }

                _mouseTypeDic.Add((MouseType)mouseCount, new Vector2(n * 0.5f, i * -0.25f));
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
    public void ChangeFaceType(FaceType type)
    {
        //�u���̎��͏u���p�̏��������s
        if (type == FaceType.Blink)
        {
            _isBlinking = true;

            if (_blinkCoroutine != null)
            {
                StopCoroutine(_blinkCoroutine);
                _blinkCoroutine = null;
            }
            _blinkCoroutine = StartCoroutine(BlinkCoroutine());
        }
        else
        {
            _isBlinking = false;
            var face = _faceTypes.FirstOrDefault(f => f.FaceType == type);

            ChangeEyeType(face.EyeType);
            ChangeMouseType(face.MouseType);
        }
    }

    void ChangeEyeType(EyeType type)
    {
        _eyeMat.SetTextureOffset("_MainTex", GetEyeOffset(type));
        _faceRenderer.materials[1] = _eyeMat;
    }

    void ChangeMouseType(MouseType type)
    {
        _mouseMat.SetTextureOffset("_MainTex", GetMouseOffset(type));
        _faceRenderer.materials[1] = _mouseMat;
    }

    Vector2 GetEyeOffset(EyeType type)
    {
        var v = _eyeTypeDic.FirstOrDefault(x => x.Key == type).Value;

        return v;
    }
    Vector2 GetMouseOffset(MouseType type)
    {
        var v = _mouseTypeDic.FirstOrDefault(x => x.Key == type).Value;

        return v;
    }

    IEnumerator BlinkCoroutine()
    {
        float timer;
        float rand;
        
        ChangeMouseType(MouseType.Default);

        //�u���̏����̃��[�v
        while (_isBlinking)
        {
            yield return new WaitForSeconds(_blinkSwitchTime);

            ChangeEyeType(EyeType.HerfEye);

            yield return new WaitForSeconds(_blinkSwitchTime);

            ChangeEyeType(EyeType.Close_Default);

            yield return new WaitForSeconds(_blinkSwitchTime);

            ChangeEyeType(EyeType.HerfEye);

            yield return new WaitForSeconds(_blinkSwitchTime);

            ChangeEyeType(EyeType.Default);
            
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
}

[Serializable]
public struct Face
{
    public FaceType FaceType;
    public EyeType EyeType;
    public MouseType MouseType;
}

