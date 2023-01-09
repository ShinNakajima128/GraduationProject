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
    Text _targetCountText = default;

    [SerializeField]
    Text _currentCountText = default;
    #endregion

    #region private
    DirectionPaper[] _directionPapers;
    ReactiveProperty<int> _currentCount = new ReactiveProperty<int>();
    int _targetCount;
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

    private IEnumerator Start()
    {
        FallGameManager.Instance.GetItem += GetAnimation;

        yield return null;

        _targetCount = FallGameManager.Instance.TargetCount;

        _targetCountText.text = $"{_targetCount.ToString("D2")}";
        _currentCountText.text = $"<color=#C320B7>{_currentCount.Value.ToString("D2")}</color>";
        _currentCount.Subscribe(_ =>
        {
            _currentCountText.text = $"<color=#C320B7>{_currentCount.Value.ToString("D2")}</color>";

            _currentCountText.gameObject.transform.DOScale(1.5f, 0.12f)
                                                  .OnComplete(() =>
                                                  {
                                                      _currentCountText.gameObject.transform.DOScale(1f, 0.12f);
                                                  });

            if (_currentCount.Value >= _targetCount)
            {
                FallGameManager.Instance.OnGameEnd();
            }
        })
        .AddTo(this);
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
