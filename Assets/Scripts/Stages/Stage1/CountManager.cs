using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using DG.Tweening;

public class CountManager : MonoBehaviour
{
    #region serialize
    [Header("UIObjects")]
    [SerializeField]
    RectTransform _targetPaperTrans = default;

    [SerializeField]
    Ease _targetPaperEase = default;

    [SerializeField]
    DirectionPaper _directionPaperPrefab = default;

    [SerializeField]
    GameObject _directionPaperParent = default;

    [SerializeField]
    int _generateNum = 3;

    [SerializeField]
    Text _countText = default;

    [SerializeField]
    int _targetCount = 10;
    #endregion
    #region private
    DirectionPaper[] _directionPapers;
    ReactiveProperty<int> _currentCount = new ReactiveProperty<int>();
    #endregion
    #region property
    #endregion

    private void Awake()
    {
        _directionPapers = new DirectionPaper[_generateNum];

        for (int i = 0; i < _directionPapers.Length; i++)
        {
            var paper = Instantiate(_directionPaperPrefab, _directionPaperParent.transform);
            _directionPapers[i] = paper;
        }
    }

    private void Start()
    {
        FallGameManager.Instance.GetItem += GetAnimation;
        _countText.text = $"{_currentCount}枚 / {_targetCount}枚";
        _currentCount.Subscribe(_ =>
        {
            _countText.text = $"{_currentCount}枚 / {_targetCount}枚";

            if (_currentCount.Value >= _targetCount)
            {
                FallGameManager.Instance.OnGameEnd();
            }
        });
    }

    void GetAnimation(IEffectable effect)
    {
        var p = UsePaper();

        try
        {
            Debug.Log("紙獲得");
            AudioManager.PlaySE(SEType.Item_Get);
            p.OnAnimation(effect.EffectPos, _targetPaperTrans, () =>
            {
                _currentCount.Value++;
                AudioManager.PlaySE(SEType.Item_Countup);

                _targetPaperTrans.DOScale(1.3f, 0.1f).SetEase(_targetPaperEase)
                                 .OnComplete(() => 
                                 {
                                     _targetPaperTrans.DOScale(1f, 0.1f).SetEase(_targetPaperEase);
                                 });
            });
        }
        catch (System.Exception e)
        {
            Debug.LogError($"使用可能なオブジェクトがありませんでした{e}");
        }
    }

    DirectionPaper UsePaper()
    {
        foreach (var p in _directionPapers)
        {
            if (!p.gameObject.activeSelf)
            {
                p.gameObject.SetActive(true);
                return p;
            }
        }
        return default;
    }
}
