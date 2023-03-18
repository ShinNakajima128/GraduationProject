using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ボス戦時にトランプ兵の処刑演出の機能を持つComponent
/// </summary>
public class ExcuteDirection : MonoBehaviour
{
    #region serialize
    [Header("Objects")]
    [SerializeField]
    GameObject _directionModels = default;

    [Tooltip("処刑演出を行うボスのAnimator")]
    [SerializeField]
    Animator _bossAnim = default;

    [Tooltip("処刑を実行するトランプ兵のAnimator")]
    [SerializeField]
    Animator[] _trumpAnims = default;

    [Tooltip("処刑されるトランプ兵のAnimator")]
    [SerializeField]
    Animator _excutedTrumpAnim = default;

    [Header("UI")]
    [Tooltip("処刑実行時に暗転する画面")]
    [SerializeField]
    CanvasGroup _excutePanel = default;
    #endregion
    #region private
    #endregion
    #region public
    #endregion
    #region property
    #endregion

    private void Start()
    {
        _directionModels.SetActive(false);
    }

    public IEnumerator ExcuteDirectionCoroutine(BossBattlePhase phase)
    {
        if (phase == BossBattlePhase.First)
        {
            _directionModels.SetActive(true);

            //yield return new WaitForSeconds(3.0f); //画面のフェード演出終了まで待機

            _bossAnim.CrossFadeInFixedTime("Angry", 0.2f);

            yield return new WaitForSeconds(1.0f);

            foreach (var t in _trumpAnims)
            {
                t.CrossFadeInFixedTime("Attack_Setup", 0.2f);
            }
            yield return new WaitForSeconds(1.5f);
        }

        _excutePanel.alpha = 1; //トランプ兵を処刑するモーションがない場合は暗転の演出

        //処刑したと分かるSEの再生はここに記述
        AudioManager.PlaySE(SEType.Trump_Slust);
        VibrationController.OnVibration(Strength.Middle, 0.3f);
        Debug.Log("トランプ兵の首飛ぶ");

        TransitionManager.FadeIn(FadeType.Normal, 1.5f, action: () =>
         {
             _excutePanel.alpha = 0;
             _directionModels.SetActive(false);
         });
        yield return new WaitForSeconds(1.5f);

        TransitionManager.FadeOut(FadeType.Normal, 0.5f);
    }
}
