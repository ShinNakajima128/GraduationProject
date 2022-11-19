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
    int _currentRedRoseCount;
    int _currentWhiteRoseCount;
    #endregion
    #region property
    public int RedRoseCount => _currentRedRoseCount;
    public int WhiteRoseCount => _currentWhiteRoseCount;
    #endregion

    private void Start()
    {
        _roses = new Rose[_roseTrans.Length];
        Debug.Log(_roseTrans.Length);
        for (int i = 0; i < _roses.Length; i++)
        {
            var r = Instantiate(_rosePrefab, _roseParent);
            r.SetRoseType();
            r.gameObject.transform.localPosition = _roseTrans[i].localPosition;
            _roses[i] = r;
        }
    }
}
