using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AliceGrowup : MonoBehaviour
{
    #region serialize
    [Header("Variables")]
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
    #endregion

    #region public
    #endregion

    #region property
    public bool IsGrowuped => _isGrowuped;
    #endregion

    /// <summary>
    /// ãêëÂâª
    /// </summary>
    public void Growup()
    {
        transform.DOScale(_sizeUpValue, _animTime)
                 .SetEase(_animEase);

        EventManager.OnEvent(Events.BossStage_GrowAlice);
        AudioManager.PlaySE(SEType.Alice_Growup);
        VibrationController.OnVibration(Strength.Middle, 0.3f);
        _isGrowuped = true;

        StartCoroutine(DiminishCoroutine());
    }

    IEnumerator DiminishCoroutine()
    {
        yield return new WaitForSeconds(_growupTime);

        AudioManager.PlaySE(SEType.Alice_ReturnSize);
        VibrationController.OnVibration(Strength.Middle, 0.3f);

        transform.DOScale(1f, _animTime)
                 .SetEase(_animEase);

        EventManager.OnEvent(Events.BossStage_DiminishAlice);

        yield return new WaitForSeconds(1.0f);
        _isGrowuped = false;
        EatMeGenerator.Instance.IsActived.Value = true;
    }
}
