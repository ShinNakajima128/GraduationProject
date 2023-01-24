using System;
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
    [Header("Data")]
    [Tooltip("お題のデータ")]
    [SerializeField]
    OrderData[] _data = default;

    [Header("Variables")]
    [Tooltip("各アニメーションの時間")]
    [SerializeField]
    float _animTime = 0.5f;

    [Tooltip("アニメーションの待機時間")]
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

    [Tooltip("ゲームスタート時に表示するアイコンObject")]
    [SerializeField]
    GameObject _gameStartIcon = default;

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
    public OrderData[] Data => _data;
    #endregion

    private void Awake()
    {
        _frontCoverOriginRotate = _letterCover_Front.localRotation;
        _insideCoverOriginRotate = _letterCover_Inside.localRotation;
        _orderOriginScale = _orderTrans.localScale;
    }

    /// <summary>
    /// お題を出題するアニメーションのコルーチン
    /// </summary>
    public IEnumerator LetterAnimationCoroutine(int round, string order)
    {
        AnimationSetup();

        yield return null;

        _roundText.text = $"{round}";
        _orderText.text = order;

        //手紙本体が画面上から降りる
        yield return _letterParentTrans.DOLocalMoveY(-60f, 1.0f)
                    .SetEase(Ease.OutQuad)
                    .WaitForCompletion();

        yield return new WaitForSeconds(_waitTime);

        //手紙の上蓋(表)が開く
        yield return _letterCover_Front.DOLocalRotate(new Vector3(90, 0, 0), _animTime)
                                       .SetEase(Ease.Linear)
                                       .WaitForCompletion();

        //手紙の上蓋(表)を非アクティブにする
        _letterCover_Front.gameObject.SetActive(false);

        //手紙の上蓋(裏)に差し替えて最後まで開く
        yield return _letterCover_Inside.DOLocalRotate(new Vector3(0, 0, 0), _animTime)
                                        .SetEase(Ease.Linear)
                                        .WaitForCompletion();
        
        //手紙の上蓋のオブジェクトの優先度を下げて子オブジェクト内の奥に配置
        _letterCover_Inside.SetAsFirstSibling();

        AudioManager.PlaySE(SEType.Stage3_OpenOrder);
        //お題が表示されているオブジェクトのアニメーション
        yield return _orderTrans.DOLocalMoveY(700, _animTime)
                                .WaitForCompletion();

        //お題のオブジェクトの優先度を上げて子オブジェクト内の手前に配置
        _orderTrans.SetParent(transform);
        _orderTrans.SetAsLastSibling();

        yield return null;

        //お題のオブジェクトの大きさを元に戻す
        _orderTrans.DOScale(1, _animTime).SetEase(Ease.Linear);

        yield return _orderTrans.DOLocalMoveY(0, _animTime)
                                .WaitForCompletion();

        yield return new WaitForSeconds(_waitTime);

        //女王の切手のアニメーション開始
        _queenStampImage.gameObject.transform.DOScale(10, 0f);
        _queenStampImage.DOFade(0, 0f);

        yield return null;

        _queenStampImage.enabled = true;
        _queenStampImage.gameObject.transform.SetAsLastSibling();
        _queenStampImage.gameObject.transform.DOScale(1, _animTime);
        yield return _queenStampImage.DOFade(1, _animTime)
                                     .WaitForCompletion();
        AudioManager.PlaySE(SEType.stage3_QueenStump);
        //女王の切手のアニメーション終了

        //手紙の傾きを変更
        AudioManager.PlaySE(SEType.Stage3_heeloverOrder);
        yield return _letterTrans.DOLocalRotate(new Vector3(0, 0, -10.5f), _animTime)
                                 .SetEase(Ease.OutBounce)
                                 .WaitForCompletion();

        _gameStartIcon.SetActive(true);

        yield return new WaitUntil(() => UIInput.Submit);

        //お題をフェードアウトさせる際にまとめて動かすための処理
        _orderTrans.SetParent(_letterTrans);
        _queenStampImage.transform.SetParent(_letterTrans);
        _queenStampImage.transform.SetAsLastSibling();
        _gameStartIcon.SetActive(false);

        yield return _letterParentTrans.DOLocalMoveY(-150, 0.5f)
                                       .SetEase(Ease.OutQuad)
                                       .WaitForCompletion();
        
        yield return new WaitForSeconds(0.02f);

        AudioManager.PlaySE(SEType.Stage3_CloseOrder);
        yield return _letterParentTrans.DOLocalMoveY(800, 0.5f)
                                       .SetEase(Ease.OutQuart)
                                       .WaitForCompletion();
    }

    /// <summary>
    /// アニメーション前の各オブジェクトの初期化処理
    /// </summary>
    void AnimationSetup()
    {
        _letterParentTrans.DOLocalMoveY(760, 0f);

        _letterTrans.DOLocalRotate(Vector3.zero, 0f);
        
        _letterCover_Front.DOLocalRotate(_frontCoverOriginRotate.eulerAngles, 0f);
        _letterCover_Front.gameObject.SetActive(true);
        _letterCover_Inside.DOLocalRotate(_insideCoverOriginRotate.eulerAngles, 0f);
        _letterCover_Inside.SetAsLastSibling();

        _orderTrans.localScale = _orderOriginScale;
        _orderTrans.SetParent(_letterTrans);
        _orderTrans.SetSiblingIndex(1);
        _orderTrans.DOScale(0.9f, 0f);
        _orderTrans.DOLocalRotate(Vector3.zero, 0f);
        _orderTrans.localPosition = new Vector3(0, 100, 0);
        
        _queenStampImage.enabled = false;
        _queenStampImage.transform.SetParent(transform);
        _queenStampImage.transform.localPosition = new Vector3(-293, -278, 0);

        _gameStartIcon.SetActive(false);
    }

    public void SetOrderData(OrderData[] data)
    {
        _data = data;
    }
}

/// <summary>
/// お題のデータ構造
/// </summary>
[Serializable]
public struct OrderData
{
    public string OrderName;
    public TrumpColorType TargetTrumpColor;
    public int TargetNum;

    public override string ToString()
    {
        string order;

        if (TargetTrumpColor == TrumpColorType.Black)
        {
            order = $"<color=#353535>黒</color>のトランプ兵を{TargetNum}体以上倒せ！";
        }
        else
        {
            order = $"<color=#D4216F>赤</color>のトランプ兵を{TargetNum}体以上倒せ！";
        }
        return order;
    }
}

