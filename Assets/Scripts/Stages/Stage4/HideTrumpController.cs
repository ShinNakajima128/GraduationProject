using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideTrumpController : MonoBehaviour
{
    #region serialize
    [Header("Variables")]
    [Tooltip("�A�j���[�V�������J�n���鋗��")]
    [SerializeField]
    float _onActionDistance = 3.0f;

    [Tooltip("�B����Ԃɖ߂�܂ł̎���")]
    [SerializeField]
    float _returnHideTime = 2.5f;
    #endregion

    #region private
    Stage4TrumpSolder _trump;
    Animator _anim;
    Transform _playerTrans;
    bool _isAnimationed = false;
    #endregion

    #region public
    #endregion

    #region property
    #endregion

    private void OnDisable()
    {
        _isAnimationed = false;
    }

    private void Awake()
    {
        TryGetComponent(out _trump);
        TryGetComponent(out _anim);
        _playerTrans = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        if (_isAnimationed)
        {
            return;
        }

        var distance = Vector3.Distance(_playerTrans.position, transform.position);

        if (distance <= _onActionDistance)
        {
            StartCoroutine(FoundOutCoroutine());
            _isAnimationed = true;
            Debug.Log("�A���X���߂Â���");
        }
    }

    /// <summary>
    /// ������₷������������R���[�`��
    /// </summary>
    IEnumerator FoundOutCoroutine()
    {
        //�؂̗��ɉB��Ă���g�����v���̃A�j���[�V��������
        if (_trump.CurrentDirType == Stage4TrumpDirectionType.HideTree)
        {
            _anim.CrossFadeInFixedTime("FindTree", 0.1f);

            yield return new WaitForSeconds(_returnHideTime);

            _anim.CrossFadeInFixedTime("ReturnHideTree", 0.1f);
        }
        //�ς܂ꂽ�o�P�c�ɉB��Ă���g�����v���̃A�j���[�V��������
        else if (_trump.CurrentDirType == Stage4TrumpDirectionType.HideBucket)
        {
            _anim.CrossFadeInFixedTime("FindBucket", 0.1f);

            yield return new WaitForSeconds(_returnHideTime);
            
            _anim.CrossFadeInFixedTime("ReturnHideBucket", 0.1f);
        }
    }
}
