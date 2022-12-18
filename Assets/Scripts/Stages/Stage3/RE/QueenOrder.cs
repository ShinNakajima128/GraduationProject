using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// ��������̂���̃I�u�W�F�N�g�̋@�\���Ǘ�����R���|�[�l���g
/// </summary>
public class QueenOrder : MonoBehaviour
{
    #region serialize
    [Header("UI_Objects")]
    [Tooltip("�莆�{�̂�Transform")]
    [SerializeField]
    Transform _letterTrans = default;

    [Tooltip("�����Transform")]
    [SerializeField]
    Transform _orderTrans = default;

    [Tooltip("�莆�̏�W(�\)��Transform")]
    [SerializeField]
    Transform _letterCover_Front = default;

    [Tooltip("�莆�̏�W(��)��Transform")]
    [SerializeField]
    Transform _letterCover_Inside = default;

    [Header("Texts")]
    [SerializeField]
    Text _roundText = default;

    [SerializeField]
    Text _orderText = default;
    #endregion

    #region private
    Quaternion _frontCoverOriginRotate;
    Quaternion _insideCoverOriginRotate;
    Vector3 _orderOriginScale;
    #endregion

    #region public
    #endregion

    #region property
    #endregion

    private void Awake()
    {
        _frontCoverOriginRotate = _letterCover_Front.localRotation;
        _insideCoverOriginRotate = _letterCover_Inside.localRotation;
        _orderOriginScale = _orderTrans.localScale;
    }
    private void Start()
    {
        StartCoroutine(LetterAnimationCoroutine());
    }

    /// <summary>
    /// ������o�肷��A�j���[�V�����̃R���[�`��
    /// </summary>
    public IEnumerator LetterAnimationCoroutine()
    {
        AnimationSetup();

        yield return null;

        //�莆�{�̂���ʏォ��~���
        yield return _letterTrans.DOLocalMoveY(-60f, 2.0f)
                    .WaitForCompletion();

        yield return new WaitForSeconds(0.5f);

        yield return _letterCover_Front.DOLocalRotate(new Vector3(90, 0, 0), 0.5f)
                                       .SetEase(Ease.Linear)
                                       .WaitForCompletion();

        _letterCover_Front.gameObject.SetActive(false);

        yield return _letterCover_Inside.DOLocalRotate(new Vector3(0, 0, 0), 0.5f)
                                        .SetEase(Ease.Linear)
                                        .WaitForCompletion();
        
        _letterCover_Inside.SetAsFirstSibling();

        yield return _orderTrans.DOLocalMoveY(700, 0.5f)
                                .WaitForCompletion();

        _orderTrans.SetParent(transform);
        _orderTrans.SetAsLastSibling();
        
        _orderTrans.DOScale(1, 0.5f).SetEase(Ease.Linear);

        yield return _orderTrans.DOLocalMoveY(100, 1.0f)
                                .WaitForCompletion();
    }

    void AnimationSetup()
    {
        _letterTrans.DOLocalMoveY(760, 0f);
        _letterCover_Front.DOLocalRotate(_frontCoverOriginRotate.eulerAngles, 0f);
        _letterCover_Front.gameObject.SetActive(true);
        _letterCover_Inside.DOLocalRotate(_insideCoverOriginRotate.eulerAngles, 0f);
        _orderTrans.localScale = _orderOriginScale;
    }
}
