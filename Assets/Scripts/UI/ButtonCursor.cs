using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonCursor : MonoBehaviour
{
    #region serialize
    [SerializeField]
    Transform _cursorTrans = default;
    #endregion

    #region private
    #endregion

    #region public
    #endregion

    #region property
    public static ButtonCursor Instance { get; private set; }
    #endregion

    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// �J�[�\���𓮂���
    /// </summary>
    /// <param name="pos"> �������ʒu </param>
    /// <param name="parent"> �z�u��̐e�I�u�W�F�N�g </param>
    public static void MoveCursor(Vector3 pos)
    {
        Instance._cursorTrans.position = pos;
    }

    /// <summary>
    /// �J�[�\���𓮂���
    /// </summary>
    /// <param name="pos"> �������ʒu </param>
    /// <param name="parent"> �z�u��̐e�I�u�W�F�N�g </param>
    public static void MoveCursor(Vector3 pos, Transform parent)
    {
        Instance._cursorTrans.SetParent(parent);
        Instance._cursorTrans.position = pos;
    }
}
