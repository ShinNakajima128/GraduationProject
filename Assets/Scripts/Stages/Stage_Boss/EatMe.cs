using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EatMe : MonoBehaviour
{
    #region serialize
    [Header("Variables")]
    [SerializeField]
    float _startHeight = 10.0f;

    [Tooltip("ˆê‰ñ“]‚É‚©‚¯‚éŽžŠÔ")]
    [SerializeField]
    float _rotateTime = 2.0f;
    #endregion

    #region private
    AliceGrowup _growup;
    #endregion

    #region protected
    protected bool _init = false;
    #endregion

    #region public
    #endregion

    #region property
    #endregion

    protected virtual void OnEnable()
    {
        if (_init)
        {
            OnRotate();
        }
    }

    protected virtual void OnDisable()
    {
        transform.DOLocalRotate(new Vector3(75, 180, 0), 0f);
        transform.DOLocalMoveY(_startHeight, 0f);
    }

    protected virtual void Start()
    {
        if (!_init)
        {
            OnRotate();
            _init = true;
        }
    }

    protected void OnRotate()
    {
        transform.DOLocalRotate(new Vector3(75, -360, 0), _rotateTime, RotateMode.FastBeyond360)
                 .SetEase(Ease.Linear)
                 .SetLoops(-1)
                 .SetLink(gameObject, LinkBehaviour.KillOnDisable);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (_growup == null)
            {
                _growup = other.GetComponent<AliceGrowup>();
            }

            if (_growup.IsGrowuped)
            {
                return;
            }

            _growup.Growup();
            EatMeGenerator.Instance.IsActived.Value = false;
            gameObject.SetActive(false);
        }
    }
}
