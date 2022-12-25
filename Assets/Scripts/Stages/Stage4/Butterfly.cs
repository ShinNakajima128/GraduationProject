using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �X�e�[�W4�̒��̋@�\�̎���Component
/// </summary>
public class Butterfly : MonoBehaviour
{
    #region serialize
    [SerializeField]
    float _moveSpeed = 3.0f;

    [Header("Components")]
    [SerializeField]
    Renderer _butterfltRenderer = default;

    [Header("Materials")]
    [SerializeField]
    Material _redMat = default;

    [SerializeField]
    Material _whiteMat = default;
    #endregion

    #region private
    Animator _anim;
    #endregion

    #region public
    #endregion

    #region property
    #endregion

    private void Awake()
    {
        TryGetComponent(out _anim);
    }

    /// <summary>
    /// �o���̐F�ɉ����Ē��̃}�e���A����ύX����
    /// </summary>
    /// <param name="type"> �o���̎�� </param>
    public void ChangeMaterial(RoseType type)
    {
        switch (type)
        {
            case RoseType.Hidden:
                break;
            case RoseType.Red:
                _butterfltRenderer.material = _whiteMat;  //�o���̐F�Ƌt�̐F�ɂ���
                break;
            case RoseType.White:
                _butterfltRenderer.material = _redMat;
                break;
            default:
                break;
        }
    }
}

public enum ButterfltState
{
    Idle,

}
