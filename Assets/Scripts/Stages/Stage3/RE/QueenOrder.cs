using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// 女王からのお題のオブジェクトの機能を管理するコンポーネント
/// </summary>
public class QueenOrder : MonoBehaviour
{
    #region serialize
    [Header("Variables")]
    [SerializeField]
    float _animTime = 0.5f;

    [SerializeField]
    float _waitTime = 0.5f;

    [Header("UI_Objects")]
    [Tooltip("手紙全体のTransform")]
    [SerializeField]
    Transform _letterParentTrans = default;

    [Tooltip("手紙のTransform")]
    [SerializeField]
    Transform _letterTrans = default;

    [Tooltip("お題のTransform")]
    [SerializeField]
    Transform _orderTrans = default;

    [Tooltip("手紙の上蓋(表)のTransform")]
    [SerializeField]
    Transform _letterCover_Front = default;

    [Tooltip("手紙の上蓋(裏)のTransform")]
    [SerializeField]
    Transform _letterCover_Inside = default;

    [Tooltip("女王の切手のImage")]
    [SerializeField]
    Image _queenStampImage = default;

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
    /// お題を出題するアニメーションのコルーチン
    /// </summary>
    public IEnumerator LetterAnimationCoroutine()
    {
        AnimationSetup();

        yield return null;

        //手紙本体が画面上から降りる
        yield return _letterParentTrans.DOLocalMoveY(-60f, 1.5f)
                    .SetEase(Ease.OutQuad)
                    .WaitForCompletion();

        yield return new WaitForSeconds(_waitTime);

        yield return _letterCover_Front.DOLocalRotate(new Vector3(90, 0, 0), _animTime)
                                       .SetEase(Ease.Linear)
                                       .WaitForCompletion();

        _letterCover_Front.gameObject.SetActive(false);

        yield return _letterCover_Inside.DOLocalRotate(new Vector3(0, 0, 0), _animTime)
                                        .SetEase(Ease.Linear)
                                        .WaitForCompletion();
        
        _letterCover_Inside.SetAsFirstSibling();

        yield return _orderTrans.DOLocalMoveY(700, _animTime)
                                .WaitForCompletion();

        _orderTrans.SetParent(transform);
        _orderTrans.SetAsLastSibling();

        yield return null;

        _orderTrans.DOScale(1, _animTime).SetEase(Ease.Linear);

        yield return _orderTrans.DOLocalMoveY(0, _animTime)
                                .WaitForCompletion();

        yield return new WaitForSeconds(_waitTime);

        _queenStampImage.gameObject.transform.DOScale(10, 0f);
        _queenStampImage.DOFade(0, 0f);

        yield return null;

        _queenStampImage.enabled = true;
        _queenStampImage.gameObject.transform.SetAsLastSibling();
        _queenStampImage.gameObject.transform.DOScale(1, _animTime);
        yield return _queenStampImage.DOFade(1, _animTime)
                                     .WaitForCompletion();

        yield return _letterTrans.DOLocalRotate(new Vector3(0, 0, -10.5f), _animTime)
                                 .SetEase(Ease.OutBounce)
                                 .WaitForCompletion();

        yield return new WaitUntil(() => UIInput.Submit);

        _orderTrans.SetParent(_letterTrans);
        _queenStampImage.transform.SetParent(_letterTrans);
        _queenStampImage.transform.SetAsLastSibling();

        yield return _letterParentTrans.DOLocalMoveY(-150, 0.5f)
                                       .SetEase(Ease.InQuart)
                                       .WaitForCompletion();
        
        yield return new WaitForSeconds(0.1f);

        yield return _letterParentTrans.DOLocalMoveY(800, 0.5f)
                                       .SetEase(Ease.OutQuart)
                                       .WaitForCompletion();
    }

    void AnimationSetup()
    {
        _letterParentTrans.DOLocalMoveY(760, 0f);
        _letterCover_Front.DOLocalRotate(_frontCoverOriginRotate.eulerAngles, 0f);
        _letterCover_Front.gameObject.SetActive(true);
        _letterCover_Inside.DOLocalRotate(_insideCoverOriginRotate.eulerAngles, 0f);
        _orderTrans.localScale = _orderOriginScale;
        _orderTrans.SetParent(_letterTrans);
        _orderTrans.SetSiblingIndex(1);
        _queenStampImage.enabled = false;
    }
}
