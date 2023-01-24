using System;
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
    [Header("Data")]
    [Tooltip("����̃f�[�^")]
    [SerializeField]
    OrderData[] _data = default;

    [Header("Variables")]
    [Tooltip("�e�A�j���[�V�����̎���")]
    [SerializeField]
    float _animTime = 0.5f;

    [Tooltip("�A�j���[�V�����̑ҋ@����")]
    [SerializeField]
    float _waitTime = 0.5f;

    [Header("UI_Objects")]
    [Tooltip("�莆�S�̂�Transform")]
    [SerializeField]
    Transform _letterParentTrans = default;

    [Tooltip("�莆��Transform")]
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

    [Tooltip("�����̐؎��Image")]
    [SerializeField]
    Image _queenStampImage = default;

    [Tooltip("�Q�[���X�^�[�g���ɕ\������A�C�R��Object")]
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
    /// ������o�肷��A�j���[�V�����̃R���[�`��
    /// </summary>
    public IEnumerator LetterAnimationCoroutine(int round, string order)
    {
        AnimationSetup();

        yield return null;

        _roundText.text = $"{round}";
        _orderText.text = order;

        //�莆�{�̂���ʏォ��~���
        yield return _letterParentTrans.DOLocalMoveY(-60f, 1.0f)
                    .SetEase(Ease.OutQuad)
                    .WaitForCompletion();

        yield return new WaitForSeconds(_waitTime);

        //�莆�̏�W(�\)���J��
        yield return _letterCover_Front.DOLocalRotate(new Vector3(90, 0, 0), _animTime)
                                       .SetEase(Ease.Linear)
                                       .WaitForCompletion();

        //�莆�̏�W(�\)���A�N�e�B�u�ɂ���
        _letterCover_Front.gameObject.SetActive(false);

        //�莆�̏�W(��)�ɍ����ւ��čŌ�܂ŊJ��
        yield return _letterCover_Inside.DOLocalRotate(new Vector3(0, 0, 0), _animTime)
                                        .SetEase(Ease.Linear)
                                        .WaitForCompletion();
        
        //�莆�̏�W�̃I�u�W�F�N�g�̗D��x�������Ďq�I�u�W�F�N�g���̉��ɔz�u
        _letterCover_Inside.SetAsFirstSibling();

        AudioManager.PlaySE(SEType.Stage3_OpenOrder);
        //���肪�\������Ă���I�u�W�F�N�g�̃A�j���[�V����
        yield return _orderTrans.DOLocalMoveY(700, _animTime)
                                .WaitForCompletion();

        //����̃I�u�W�F�N�g�̗D��x���グ�Ďq�I�u�W�F�N�g���̎�O�ɔz�u
        _orderTrans.SetParent(transform);
        _orderTrans.SetAsLastSibling();

        yield return null;

        //����̃I�u�W�F�N�g�̑傫�������ɖ߂�
        _orderTrans.DOScale(1, _animTime).SetEase(Ease.Linear);

        yield return _orderTrans.DOLocalMoveY(0, _animTime)
                                .WaitForCompletion();

        yield return new WaitForSeconds(_waitTime);

        //�����̐؎�̃A�j���[�V�����J�n
        _queenStampImage.gameObject.transform.DOScale(10, 0f);
        _queenStampImage.DOFade(0, 0f);

        yield return null;

        _queenStampImage.enabled = true;
        _queenStampImage.gameObject.transform.SetAsLastSibling();
        _queenStampImage.gameObject.transform.DOScale(1, _animTime);
        yield return _queenStampImage.DOFade(1, _animTime)
                                     .WaitForCompletion();
        AudioManager.PlaySE(SEType.stage3_QueenStump);
        //�����̐؎�̃A�j���[�V�����I��

        //�莆�̌X����ύX
        AudioManager.PlaySE(SEType.Stage3_heeloverOrder);
        yield return _letterTrans.DOLocalRotate(new Vector3(0, 0, -10.5f), _animTime)
                                 .SetEase(Ease.OutBounce)
                                 .WaitForCompletion();

        _gameStartIcon.SetActive(true);

        yield return new WaitUntil(() => UIInput.Submit);

        //������t�F�[�h�A�E�g������ۂɂ܂Ƃ߂ē��������߂̏���
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
    /// �A�j���[�V�����O�̊e�I�u�W�F�N�g�̏���������
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
/// ����̃f�[�^�\��
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
            order = $"<color=#353535>��</color>�̃g�����v����{TargetNum}�̈ȏ�|���I";
        }
        else
        {
            order = $"<color=#D4216F>��</color>�̃g�����v����{TargetNum}�̈ȏ�|���I";
        }
        return order;
    }
}

