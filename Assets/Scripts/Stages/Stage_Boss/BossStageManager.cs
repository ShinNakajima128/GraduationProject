using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

/// <summary>
/// ボスステージの機能を管理するマネージャー
/// </summary>
public class BossStageManager : StageGame<BossStageManager>
{
    #region serialize
    [Header("Variables")]
    [Tooltip("戦闘回数")]
    [SerializeField]
    int _battleNum = 3;

    [Tooltip("別のカメラへ遷移する際にかける時間")]
    [SerializeField]
    float _cameraBlendTime = 2.0f;

    [SerializeField]
    float _closeupTime = 0.25f;

    [Header("Cameras")]
    [Tooltip("Cinemachineを管理するBrain")]
    [SerializeField]
    CinemachineBrain _cameraBrain = default;

    [Tooltip("女王に注目するカメラ")]
    [SerializeField]
    CinemachineVirtualCamera _directionCamera = default;

    [Tooltip("女王に更にズームインするカメラ")]
    [SerializeField]
    CinemachineVirtualCamera _closeupCamera = default;

    [Tooltip("戦闘中の見下ろしカメラ")]
    [SerializeField]
    CinemachineVirtualCamera _battleCamera = default;

    [Header("Objects")]
    [Tooltip("戦闘中の移動可能範囲のエフェクト")]
    [SerializeField]
    GameObject _areaEffect = default;

    [Header("Component")]
    [SerializeField]
    MessagePlayer _messagePlayer = default;

    [SerializeField]
    BossController _bossCtrl = default;
    #endregion
    #region private
    /// <summary> 演出中かどうか </summary>
    bool _isDirecting = false;
    #endregion
    #region public
    public override event Action GameSetUp;
    public override event Action GameStart;
    public override event Action GamePause;
    public override event Action GameEnd;
    #endregion
    #region property
    #endregion

    protected override void Start()
    {
        base.Start();
        Init();
        OnGameStart();
    }

    public override void OnGameSetUp()
    {
        GameSetUp?.Invoke();
        _areaEffect.SetActive(true);
    }

    public override void OnGameStart()
    {
        GameStart?.Invoke();
        StartCoroutine(GameStartCoroutine());
    }

    public override void OnGameEnd()
    {
        GameEnd?.Invoke();
    }

    protected override IEnumerator GameStartCoroutine(Action action = null)
    {
        yield return new WaitForSeconds(1.5f);

        //カメラをボスに寄せる
        CameraBlend(CameraType.Direction, _cameraBlendTime);

        yield return new WaitForSeconds(_cameraBlendTime);

        //ボス戦開始時の演出を再生
        StartCoroutine(_messagePlayer.PlayMessageCorountine(MessageType.Stage_Boss_Start, () =>
        {
            _isDirecting = true;
        }));

        yield return new WaitUntil(() => _isDirecting);

        _isDirecting = false;

        CameraBlend(CameraType.Battle, _cameraBlendTime);

        yield return new WaitForSeconds(_cameraBlendTime);

        StartCoroutine(InGameCoroutine());
    }

    protected override IEnumerator GameEndCoroutine(Action action = null)
    {
        throw new NotImplementedException();
    }

    protected override void Init()
    {
        _messagePlayer.Closeup += OnCloseup;
        _messagePlayer.Reset += OnReset;
    }

    IEnumerator InGameCoroutine()
    {
        for (int i = 0; i < _battleNum; i++)
        {
            CharacterMovable?.Invoke(true);
            OnGameSetUp();

            yield return _bossCtrl.BattlePhaseCoroutine();
        }
        
    }

    /// <summary>
    /// 現在のカメラから別のカメラへ遷移する
    /// </summary>
    /// <param name="type"> カメラの種類 </param>
    /// <param name="blendTime"> 遷移にかける時間 </param>
    void CameraBlend(CameraType type, float blendTime)
    {
        _cameraBrain.m_DefaultBlend.m_Time = blendTime;

        switch (type)
        {
            case CameraType.Default:
                _directionCamera.Priority = 9;
                _closeupCamera.Priority = 9;
                _battleCamera.Priority = 9;
                break;
            case CameraType.Direction:
                _directionCamera.Priority = 15;
                _closeupCamera.Priority = 9;
                _battleCamera.Priority = 9;
                break;
            case CameraType.Battle:
                _directionCamera.Priority = 9;
                _closeupCamera.Priority = 9;
                _battleCamera.Priority = 15;
                break;
            case CameraType.Direction_Closeup:
                _directionCamera.Priority = 9;
                _closeupCamera.Priority = 15;
                _battleCamera.Priority = 9;
                break;
            default:
                break;
        }
    }

    void OnCloseup()
    {
        CameraBlend(CameraType.Direction_Closeup, _closeupTime);
    }

    void OnReset()
    {
        CameraBlend(CameraType.Direction_Closeup, _cameraBlendTime);
    }
}
public enum CameraType
{
    Default,
    Direction,
    Battle,
    Direction_Closeup
}
