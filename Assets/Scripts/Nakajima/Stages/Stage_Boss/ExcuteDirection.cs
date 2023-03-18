using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �{�X�펞�Ƀg�����v���̏��Y���o�̋@�\������Component
/// </summary>
public class ExcuteDirection : MonoBehaviour
{
    #region serialize
    [Header("Objects")]
    [SerializeField]
    GameObject _directionModels = default;

    [Tooltip("���Y���o���s���{�X��Animator")]
    [SerializeField]
    Animator _bossAnim = default;

    [Tooltip("���Y�����s����g�����v����Animator")]
    [SerializeField]
    Animator[] _trumpAnims = default;

    [Tooltip("���Y�����g�����v����Animator")]
    [SerializeField]
    Animator _excutedTrumpAnim = default;

    [Header("UI")]
    [Tooltip("���Y���s���ɈÓ]������")]
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

            //yield return new WaitForSeconds(3.0f); //��ʂ̃t�F�[�h���o�I���܂őҋ@

            _bossAnim.CrossFadeInFixedTime("Angry", 0.2f);

            yield return new WaitForSeconds(1.0f);

            foreach (var t in _trumpAnims)
            {
                t.CrossFadeInFixedTime("Attack_Setup", 0.2f);
            }
            yield return new WaitForSeconds(1.5f);
        }

        _excutePanel.alpha = 1; //�g�����v�������Y���郂�[�V�������Ȃ��ꍇ�͈Ó]�̉��o

        //���Y�����ƕ�����SE�̍Đ��͂����ɋL�q
        AudioManager.PlaySE(SEType.Trump_Slust);
        VibrationController.OnVibration(Strength.Middle, 0.3f);
        Debug.Log("�g�����v���̎���");

        TransitionManager.FadeIn(FadeType.Normal, 1.5f, action: () =>
         {
             _excutePanel.alpha = 0;
             _directionModels.SetActive(false);
         });
        yield return new WaitForSeconds(1.5f);

        TransitionManager.FadeOut(FadeType.Normal, 0.5f);
    }
}
