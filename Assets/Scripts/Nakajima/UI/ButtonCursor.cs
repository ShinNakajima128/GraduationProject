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
    /// カーソルを動かす
    /// </summary>
    /// <param name="pos"> 動かす位置 </param>
    /// <param name="parent"> 配置先の親オブジェクト </param>
    public static void MoveCursor(Vector3 pos)
    {
        Instance._cursorTrans.position = pos;
    }

    /// <summary>
    /// カーソルを動かす
    /// </summary>
    /// <param name="pos"> 動かす位置 </param>
    /// <param name="parent"> 配置先の親オブジェクト </param>
    public static void MoveCursor(Vector3 pos, Transform parent)
    {
        Instance._cursorTrans.SetParent(parent);
        Instance._cursorTrans.position = pos;
    }
}
