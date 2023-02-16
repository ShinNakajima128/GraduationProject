using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using DG.Tweening;

/// <summary>
/// �`�F�V���L�̍s�����Ǘ�����R���|�[�l���g
/// </summary>
public class CheshireCatBehaviour : MonoBehaviour
{
    #region serialize
    [Header("Variables")]
    [Tooltip("�f�t�H���g�̑ҋ@����")]
    [SerializeField]
    float _waitTime = 5.0f;

    [Tooltip("�����Ă��鎞��")]
    [SerializeField, Range(0, 10)]
    float _sittingTime = 5.0f;

    [Tooltip("�ړ�����")]
    [SerializeField]
    float _moveTime = 3.0f;

    [Tooltip("�ړ����x")]
    [SerializeField]
    float _moveSpeed = 3.0f;

    [SerializeField]
    float _rotateTime = 0.5f;

    [Tooltip("���ڂ��n�߂鋗��")]
    [SerializeField]
    float _lookAtDistance = 5.0f;

    [Tooltip("���ڂ��Ēǂ�������X�s�[�h")]
    [SerializeField]
    float _lookSpeed = 5.0f;

    [Tooltip("��������p�x�̐����l")]
    [SerializeField]
    float _lookAtAngleLimit = 60f;

    [Header("Objects")]
    [Tooltip("���[�v����ʒu�f�[�^")]
    [SerializeField]
    WarpBehaviourData[] _warpPositionsData = default;

    [Tooltip("�`�F�V���L�̓���Transform")]
    [SerializeField]
    Transform _catHeadTrans = default;
    #endregion

    #region private
    Transform _playerTrans;
    CheshireCat _cat;
    int _currentWarpPosIndex = 0;
    #endregion

    #region public
    #endregion

    #region property
    #endregion

    private void Awake()
    {
        _playerTrans = GameObject.FindGameObjectWithTag("Player").transform;
        TryGetComponent(out _cat);
    }

    //private void Start()
    //{
    //    this.LateUpdateAsObservable()
    //        .Where(_ => _lookAtDistance >= Vector3.Distance(transform.position, _playerTrans.position))
    //        .Subscribe(_ =>
    //        {
    //            Vector3 relativePos = _playerTrans.position - transform.position;
    //            // �������A��]���ɕϊ�
    //            Quaternion rotation = Quaternion.LookRotation(relativePos);
    //            // ���݂̉�]���ƁA�^�[�Q�b�g�����̉�]����⊮����
    //            _catHeadTrans.localRotation = Quaternion.Slerp(_catHeadTrans.localRotation, rotation, _lookSpeed * Time.deltaTime);
    //            Debug.Log("�A���X����");
    //        })
    //        .AddTo(this);
    //}

    public void StartMoving()
    {
        StartCoroutine(WarpCoroutine());
        _cat.CheshireFaceCtrl.ChangeFaceType(CheshireCatFaceType.Blink);
    }

    IEnumerator IdleCoroutine()
    {
        Debug.Log("������蒆");
        yield return new WaitForSeconds(_waitTime);
    }

    IEnumerator SittingCoroutine()
    {
        Debug.Log("���났��");
        yield return new WaitForSeconds(_sittingTime);
    }
    IEnumerator MoveCoroutine()
    {
        Debug.Log("�ړ���");
        float timer;
        int random = -1;
        yield return new WaitForSeconds(0.5f);
        
        _cat.ChangeState(CheshireCatState.Walk);

        while (random != 0)
        {
            timer = 0;

            while (timer < _moveTime)
            {
                transform.localPosition += _moveSpeed * Time.deltaTime * transform.forward;
                timer += Time.deltaTime;
                yield return null;
            }
            int randomMotion = UnityEngine.Random.Range(0, 4);

            if (randomMotion != 0)
            {
                _cat.ChangeState(CheshireCatState.Idle_Standing, 1.0f);
            }
            else
            {
                _cat.ChangeState(CheshireCatState.Idle, 1.0f);
            }
            yield return new WaitForSeconds(_waitTime);
            random = UnityEngine.Random.Range(0, 3);

            if (random != 0)
            {
                int rotate_y = UnityEngine.Random.Range(0, 360);
                
                _cat.ChangeState(CheshireCatState.Walk);
                yield return transform.DOLocalRotate(new Vector3(0f, rotate_y, 0f), _rotateTime)
                                      .WaitForCompletion();
            }
        }
    }

    IEnumerator WarpCoroutine()
    {
        if (!_cat.IsDissolved)
        {
            _cat.ActivateDissolve(true, 0f); 
        }

        yield return null;

        while (true)
        {
            if (!_cat.IsDissolved)
            {
                _cat.ActivateDissolve(true);

                yield return new WaitForSeconds(1.5f);     
            }
            int randomIndex = _currentWarpPosIndex;

            //�O��̃��[�v�|�C���g�ƈႤIndex�ɂȂ�܂Ń��[�v
            while (randomIndex == _currentWarpPosIndex)
            {
                randomIndex = UnityEngine.Random.Range(0, _warpPositionsData.Length);
            }
            _currentWarpPosIndex = randomIndex;

            var data = _warpPositionsData[_currentWarpPosIndex]; //���݂̃��[�v�ʒu�̍��W�f�[�^���擾

            transform.DOLocalMove(data.WarpTrans.localPosition, 0f);
            transform.DOLocalRotate(data.WarpTrans.localRotation.eulerAngles, 0f);

            yield return new WaitForSeconds(0.5f);

            _cat.ActivateDissolve(false);

            _cat.ChangeState(data.CatAnimState);

            yield return new WaitForSeconds(1.5f);

            //���[�v�ʒu�̎�ނɂ���Ď��̍s����ύX����
            switch (data.PointType)
            {
                case WarpPointType.Default:
                    yield return MoveCoroutine();
                    break;
                case WarpPointType.Cushion:
                    yield return SittingCoroutine();
                    break;
                case WarpPointType.Chair:
                    yield return IdleCoroutine();
                    break;
                default:
                    Debug.LogError("�\�����ʃG���[�ł�");
                    break;
            }
        }
    }
}

/// <summary>
/// �`�F�V���L�̍s���̎��
/// </summary>
public enum CheshireBehaviorType
{
    Idle,
    Sitting,
    Move,
    Warp,
    NUM
}

/// <summary>
/// ���[�v���W�̈ʒu
/// </summary>
public enum WarpPointType
{
    Default,
    Cushion,
    Chair
}

/// <summary>
/// ���[�v���̍��W�ƃA�j���[�V�����̃f�[�^
/// </summary>
[Serializable]
public struct WarpBehaviourData
{
    public string BehaviourName;
    public WarpPointType PointType;
    public Transform WarpTrans;
    public CheshireCatState CatAnimState;
}