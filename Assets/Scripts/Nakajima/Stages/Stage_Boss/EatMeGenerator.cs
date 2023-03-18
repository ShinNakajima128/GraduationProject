using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using DG.Tweening;

public class EatMeGenerator : MonoBehaviour
{
    #region serialize
    [Header("Variables")]
    [SerializeField]
    float _animTime = 10f;

    [SerializeField]
    EatMe _eatMeObject = default;
    #endregion

    #region private
    ReactiveProperty<bool> _isActived = new ReactiveProperty<bool>(false);
    #endregion

    #region public
    #endregion

    #region property
    public static EatMeGenerator Instance { get; private set; }
    public bool IsEatMeActived => _eatMeObject.gameObject.activeSelf;
    public ReactiveProperty<bool> IsActived => _isActived;
    #endregion

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _isActived.Subscribe(value =>
        {
            if (value)
            {
                OnGenerate();
            }
        })
        .AddTo(this);
    }

    /// <summary>
    /// EatMeÇê∂ê¨Ç∑ÇÈ
    /// </summary>
    void OnGenerate()
    {
        _eatMeObject.gameObject.SetActive(true);

        _eatMeObject.transform.DOLocalMoveY(0.5f, _animTime)
                              .SetEase(Ease.Linear);
    }

    public void Return()
    {
        _eatMeObject.gameObject.SetActive(false);
    }
}
