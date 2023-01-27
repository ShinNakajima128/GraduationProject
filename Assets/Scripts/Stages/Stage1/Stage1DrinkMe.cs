using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Stage1DrinkMe : DrinkMe
{
    #region serialize
    [Header("Variables")]
    [SerializeField]
    float _targetPosition_y = 20f;

    [SerializeField]
    float _moveTime = 10f;
    #endregion

    #region private
    #endregion

    #region public
    #endregion

    #region property
    #endregion

    protected override void OnEnable()
    {
        base.OnEnable();

        if (_init)
        {
            OnMoving();
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        if (_init)
        {
            transform.localPosition = Vector3.zero;
        }
    }

    protected override void Start()
    {
        base.Start();
        OnMoving();
    }

    private void OnMoving()
    {
        transform.DOLocalMoveY(transform.localPosition.y + _targetPosition_y, _moveTime)
                 .SetEase(Ease.Linear)
                 .OnComplete(() => 
                 {
                     gameObject.SetActive(false);
                 })
                 .SetLink(gameObject, LinkBehaviour.KillOnDisable);
    }
}
