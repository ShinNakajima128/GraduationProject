using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EatMe : MonoBehaviour
{
    #region serialize
    [SerializeField]
    float _sizeUpValue = 1.5f;

    [SerializeField]
    float _animTime = 0.5f;

    [SerializeField]
    Ease _animEase = Ease.InBounce;

    [SerializeField]
    float _growupTime = 10.0f;
    #endregion

    #region private
    bool _isGrowuped = false;
    Transform _player;
    #endregion

    #region public
    #endregion

    #region property
    #endregion

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (_isGrowuped)
            {
                return;
            }

            if (_player == null)
            {
                _player = other.transform;
            }
            _isGrowuped = true;
            EventManager.OnEvent(Events.BossStage_GrowAlice);
            _player.transform.DOScale(_sizeUpValue, _animTime)
                           .SetEase(_animEase);
            StartCoroutine(DiminishCoroutine());
        }
    }
    IEnumerator DiminishCoroutine()
    {
        yield return new WaitForSeconds(_growupTime);
        
        _player.transform.DOScale(1f, _animTime)
                           .SetEase(_animEase);

        EventManager.OnEvent(Events.BossStage_DiminishAlice);
        _isGrowuped = false;
    }
}
