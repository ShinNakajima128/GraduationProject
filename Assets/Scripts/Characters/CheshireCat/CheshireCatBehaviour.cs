using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using DG.Tweening;

/// <summary>
/// チェシャ猫の行動を管理するコンポーネント
/// </summary>
public class CheshireCatBehaviour : MonoBehaviour
{
    #region serialize
    [Header("Variables")]
    [Tooltip("デフォルトの待機時間")]
    [SerializeField]
    float _waitTime = 5.0f;

    [Tooltip("座っている時間")]
    [SerializeField, Range(0, 10)]
    float _sittingTime = 5.0f;

    [Tooltip("移動時間")]
    [SerializeField]
    float _moveTime = 3.0f;

    [Tooltip("移動速度")]
    [SerializeField]
    float _moveSpeed = 3.0f;

    [SerializeField]
    float _rotateTime = 0.5f;

    [Tooltip("注目を始める距離")]
    [SerializeField]
    float _lookAtDistance = 5.0f;

    [Tooltip("注目して追いかけるスピード")]
    [SerializeField]
    float _lookSpeed = 5.0f;

    [Tooltip("注視する角度の制限値")]
    [SerializeField]
    float _lookAtAngleLimit = 60f;

    [Header("Objects")]
    [Tooltip("ワープする位置データ")]
    [SerializeField]
    WarpBehaviourData[] _warpPositionsData = default;

    [Tooltip("チェシャ猫の頭のTransform")]
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
    //            // 方向を、回転情報に変換
    //            Quaternion rotation = Quaternion.LookRotation(relativePos);
    //            // 現在の回転情報と、ターゲット方向の回転情報を補完する
    //            _catHeadTrans.localRotation = Quaternion.Slerp(_catHeadTrans.localRotation, rotation, _lookSpeed * Time.deltaTime);
    //            Debug.Log("アリス注目");
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
        Debug.Log("おすわり中");
        yield return new WaitForSeconds(_waitTime);
    }

    IEnumerator SittingCoroutine()
    {
        Debug.Log("くつろぎ中");
        yield return new WaitForSeconds(_sittingTime);
    }
    IEnumerator MoveCoroutine()
    {
        Debug.Log("移動中");
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

            //前回のワープポイントと違うIndexになるまでループ
            while (randomIndex == _currentWarpPosIndex)
            {
                randomIndex = UnityEngine.Random.Range(0, _warpPositionsData.Length);
            }
            _currentWarpPosIndex = randomIndex;

            var data = _warpPositionsData[_currentWarpPosIndex]; //現在のワープ位置の座標データを取得

            transform.DOLocalMove(data.WarpTrans.localPosition, 0f);
            transform.DOLocalRotate(data.WarpTrans.localRotation.eulerAngles, 0f);

            yield return new WaitForSeconds(0.5f);

            _cat.ActivateDissolve(false);

            _cat.ChangeState(data.CatAnimState);

            yield return new WaitForSeconds(1.5f);

            //ワープ位置の種類によって次の行動を変更する
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
                    Debug.LogError("予期せぬエラーです");
                    break;
            }
        }
    }
}

/// <summary>
/// チェシャ猫の行動の種類
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
/// ワープ座標の位置
/// </summary>
public enum WarpPointType
{
    Default,
    Cushion,
    Chair
}

/// <summary>
/// ワープ時の座標とアニメーションのデータ
/// </summary>
[Serializable]
public struct WarpBehaviourData
{
    public string BehaviourName;
    public WarpPointType PointType;
    public Transform WarpTrans;
    public CheshireCatState CatAnimState;
}