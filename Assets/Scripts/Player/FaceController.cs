using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EyeTypes
{
    Default, /// デフォルト.
    Angry, /// 怒ってる.
    Blink, /// 瞬き.
    Close, /// 閉じている.
    Open, /// 見開いている.
    Dizzy, /// フラフラ.
    Happy, /// 嬉しい.
    Hmm, /// ふーん.
    NUM, /// 目の種類数. 
}

public class FaceController : MonoBehaviour
{
    #region serialize
    [SerializeField]
    EyeTypes _TestEyeType = default;

    [SerializeField]
    Renderer _eyeRenderer = default;
    #endregion
    #region private
    Dictionary<EyeTypes, Vector2> _eyeTypeDic = new Dictionary<EyeTypes, Vector2>();
    Material _eyeMat;
    bool _init = false;
    #endregion
    #region property
    #endregion

    private void OnValidate()
    {
        if (_init)
        {
            ChangeEyeType(_TestEyeType);
        }
    }
    private void Awake()
    {
        int count = 0;
        for (int i = 0; i < 4; i++)
        {
            for (int n = 0; n < 2; n++)
            {
                _eyeTypeDic.Add((EyeTypes)count, new Vector2(n * 0.5f, i * - 0.25f));
                //Debug.Log($"offset{(EyeTypes)count} x:{n * 0.5f}, y { i * -0.25f}");
                count++;
            }
        }
        _eyeMat = _eyeRenderer.materials[1];
        _init = true;
    }

    public void ChangeEyeType(EyeTypes type)
    {
        _eyeMat.SetTextureOffset("_MainTex", GetEyeOffset(type));
        _eyeRenderer.materials[1] = _eyeMat;
    }

    Vector2 GetEyeOffset(EyeTypes type)
    {
        var v = _eyeTypeDic.FirstOrDefault(x => x.Key == type).Value;

        return v;
    }
}
