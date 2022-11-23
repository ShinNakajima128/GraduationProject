using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoseTree : MonoBehaviour
{
    #region serialize
    [SerializeField]
    Transform[] _roseTrans = default;

    [SerializeField]
    Rose _rosePrefab = default;

    [SerializeField]
    Transform _roseParent = default;
    #endregion
    #region private
    Rose[] _roses;
    
    #endregion
    #region property
    public int RedRoseCount => _roses.Count(r => r.CurrentRoseType == RoseType.Red);
    public int WhiteRoseCount => _roses.Count(r => r.CurrentRoseType == RoseType.White);
    #endregion

    private void Start()
    {
        _roses = new Rose[_roseTrans.Length];
        for (int i = 0; i < _roses.Length; i++)
        {
            var r = Instantiate(_rosePrefab, _roseParent);
            r.SetRoseType();
            r.gameObject.transform.localPosition = _roseTrans[i].localPosition;
            _roses[i] = r;
        }
    }

    /// <summary>
    /// çƒîzíu
    /// </summary>
    public void Deploy()
    {
        foreach (var r in _roses)
        {
            r.SetRoseType();
        }
    }
}
