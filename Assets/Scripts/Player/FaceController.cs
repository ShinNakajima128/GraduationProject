using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EyeTypes : int
{
    Default, /// �f�t�H���g.
    Angry, /// �{���Ă�.
    Blink, /// �u��.
    Close, /// ���Ă���.
    Open, /// ���J���Ă���.
    Dizzy, /// �t���t��.
    Happy, /// ������.
    Hmm, /// �Ӂ[��.
    NUM, /// �ڂ̎�ސ�. 
}

public class FaceController : MonoBehaviour
{
    #region serialize
    [SerializeField]
    Renderer _eyeRenderer = default;
    #endregion
    #region private
    Dictionary<EyeTypes, Vector2> _eyeTypeDic = new Dictionary<EyeTypes, Vector2>();
    Material _eyeMat;
    #endregion
    #region property
    #endregion

    private void Awake()
    {
        int count = 0;
        for (int i = 0; i < 2; i++)
        {
            for (int n = 0; n < 4; n++)
            {
                _eyeTypeDic.Add((EyeTypes)count, new Vector2(i, (n + 1) * 0.25f));
                Debug.Log($"offset{(EyeTypes)count} x:{i + n}, y { new Vector2(i, (n + 1) * 0.25f)}");
                count++;
            }
        }
        _eyeMat = _eyeRenderer.material;
    }

    public void ChangeEyeType(EyeTypes type)
    {
        _eyeMat.SetTextureOffset("_BaseMap", GetEyeOffset(type));
        _eyeRenderer.material = _eyeMat;
    }

    Vector2 GetEyeOffset(EyeTypes type)
    {
        var v = _eyeTypeDic.FirstOrDefault(x => x.Key == type).Value;

        return v;
    }
}
