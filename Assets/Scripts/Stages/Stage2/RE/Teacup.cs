using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Teacup : MonoBehaviour
{
    #region serialize
    [Header("Variables")]
    [SerializeField]
    float _animTime = 1.5f;
    
    [Header("Objects")]
    [SerializeField]
    Transform _teacupCover = default;

    [SerializeField]
    Transform _mouseTrans = default;
    #endregion

    #region private
    bool _isInMouse = false;
    #endregion

    #region public
    #endregion

    #region property
    public Vector3 MousePos => _mouseTrans.position;
    public bool IsInMouse { get => _isInMouse; set => _isInMouse = value; }
    #endregion

    public void UpCup()
    {
        _teacupCover.DOLocalMoveY(0.2f, _animTime);
    }

    public void DownCup()
    {
        if (!_teacupCover.gameObject.activeSelf)
        {
            _teacupCover.gameObject.SetActive(true);
        }
        _teacupCover.DOLocalMoveY(0.044f, _animTime);
    }
}
