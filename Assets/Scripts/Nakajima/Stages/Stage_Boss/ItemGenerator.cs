using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGenerator : MonoBehaviour
{
    #region serialize
    [Header("Variables")]
    [Tooltip("アイテムを生成する数")]
    [SerializeField]
    int _generateCount = 3;

    [Tooltip("一度に生成する数の限界値")]
    [SerializeField]
    int _currentGenerateLimit = 5;

    [Header("Components")]
    [Tooltip("回復アイテムを管理するController")]
    [SerializeField]
    ItemController _itemCtrl = default;
    #endregion

    #region private
    #endregion

    #region public
    #endregion

    #region property
    public static ItemGenerator Instance { get; private set; }
    #endregion

    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// 生成する
    /// </summary>
    /// <param name="generatePos"> 生成する位置 </param>
    public void Generate(Vector3 generatePos)
    {
        //既にアクティブとなっている数が一定以上の場合は処理を行わない
        if (_currentGenerateLimit <= _itemCtrl.CurrentActiveCount)
        {
            print("既に指定以上の数が生成されています");
            return;
        }
        _itemCtrl.Use(generatePos);
    }

    /// <summary>
    /// 生成中のアイテムを全て非アクティブにする
    /// </summary>
    public void Return()
    {
        _itemCtrl.Return();
    }
}
