using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class WhitePaper : MonoBehaviour, IObjectable
{
    [Header("Animation")]
    [SerializeField]
    float _moveTime = 2.0f;

    [SerializeField]
    float _moveValue_X = 2.0f;

    [SerializeField]
    float _moveValue_Y = -1.0f;

    [SerializeField]
    float _rotateAngle = 45;

    [Tooltip("非アクティブになるまでの時間")]
    [SerializeField]
    float _vanishTime = 20.0f;

    #region private
    Vector3[] _path;
    bool _init = false;
    Sequence _seq;
    IEffectable _effectTarget;
    #endregion

    #region property
    public bool IsActive { get; private set; } = true;
    #endregion

    private void OnEnable()
    {
        StartCoroutine(OnVanishTimer());
        if (_init)
        {
            transform.localPosition = new Vector3(-_moveValue_X, 0f, 0f);
            transform.DOLocalRotate(new Vector3(0f, 0f, -_rotateAngle), 0f);
            OnPaperAnimation();
        }
    }

    private void OnDisable()
    {
        IsActive = true;
        
        if (_seq != null)
        {
            _seq.Kill();
            _seq = null;
        }
    }

    private void Start()
    {
        if (!_init)
        {
            transform.localPosition = new Vector3(-_moveValue_X, 0f, 0f);
            _path = new Vector3[] { new Vector3(0f, _moveValue_Y, 0f), new Vector3(_moveValue_X, 0f, 0f) };
            OnPaperAnimation();
            _init = true;
        }
    }
    void OnPaperAnimation()
    {
        _seq = DOTween.Sequence();

        _seq.Append(transform.DOLocalPath(_path, _moveTime, PathType.CatmullRom).SetEase(Ease.InOutSine))
            .Join(transform.DOLocalRotate(new Vector3(0f, 0f, _rotateAngle), _moveTime).SetEase(Ease.InOutSine))
            .SetLoops(-1, LoopType.Yoyo)
            .Play();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            IsActive = false;

            if (_effectTarget == null)
            {
                _effectTarget = other.GetComponent<IEffectable>();
            }
            EffectManager.PlayEffect(EffectType.Get, _effectTarget.EffectPos);
        }
    }

    /// <summary>
    /// 非アクティブするタイマーを開始
    /// </summary>
    /// <returns></returns>
    IEnumerator OnVanishTimer()
    {
        yield return new WaitForSeconds(_vanishTime);
        IsActive = false;
    }
}
