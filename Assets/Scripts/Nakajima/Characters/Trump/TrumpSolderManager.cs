using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TrumpSolderManager : MonoBehaviour
{
    #region serialize
    [Header("Variables")]
    [Tooltip("構えてから突くまでの待機時間")]
    [SerializeField]
    float _waitTime = 2.0f;

    [Tooltip("攻撃判定が残る時間")]
    [SerializeField]
    float _durationTime = 3.0f;

    [Header("Objects")]
    [Tooltip("トランプ兵のPrefab")]
    [SerializeField]
    BattleAreaTrumpSolder _trumpPrefab = default;

    [SerializeField]
    Transform _trumpSolderParent = default;

    [SerializeField]
    BossAreaPositions[] _areaPositions = default;

    [SerializeField]
    GameObject[] _attackRangeEffect = default;
    #endregion
    #region private
    List<BattleAreaTrumpSolder> _trumpSolderList = new List<BattleAreaTrumpSolder>();
    readonly List<BattleAreaTrumpSolder> _frontTrumps = new List<BattleAreaTrumpSolder>();
    readonly List<BattleAreaTrumpSolder> _backTrumps = new List<BattleAreaTrumpSolder>();
    readonly List<BattleAreaTrumpSolder> _leftTrumps = new List<BattleAreaTrumpSolder>();
    readonly List<BattleAreaTrumpSolder> _rightTrumps = new List<BattleAreaTrumpSolder>();
    Coroutine[] _attackCoroutines = new Coroutine[4];
    Action _finishAttackAction;
    #endregion
    #region public
    #endregion
    #region property
    #endregion

    private void Awake()
    {
        int generateNum = 0;
        foreach (var p in _areaPositions)
        {
            generateNum += p.LineUpTrans.Length;
        }
        Debug.Log($"トランプ兵の生成数：{generateNum}");

        for (int i = 0; i < generateNum; i++)
        {
            var trump = Instantiate(_trumpPrefab, _trumpSolderParent);
            _trumpSolderList.Add(trump);
        }
    }

    private void Start()
    {
        SetupTrumpSolders();
        //EventManager.ListenEvents(Events.BossStage_End_CheshireFront, () => OnAllTrumpAnimation("LookUp"));
    }

    /// <summary>
    /// 全てのトランプ兵をアクティブ状況を切り替える
    /// </summary>
    /// <param name="isActive"> アクティブかどうか </param>
    public void OnAllTrumpActivate(bool isActive)
    {
        foreach (var t in _frontTrumps)
        {
            t.gameObject.SetActive(isActive);
        }

        foreach (var t in _backTrumps)
        {
            t.gameObject.SetActive(isActive);
        }

        foreach (var t in _leftTrumps)
        {
            t.gameObject.SetActive(isActive);
        }

        foreach (var t in _rightTrumps)
        {
            t.gameObject.SetActive(isActive);
        }
    }

    public void OnTrumpSoldersAttack(DirectionType dir, Action start = null, Action finish = null)
    {
        _attackCoroutines[(int)dir] = StartCoroutine(TrumpSoldersAttackCoroutine(dir, start, finish));

        if (finish != null)
        {
            finish = null;
        }
        _finishAttackAction = finish;
    }

    /// <summary>
    /// 全てのトランプ兵を焦らせる
    /// </summary>
    public void OnAllTrumpAnimation(string name)
    {
        for (int i = 0; i < _attackCoroutines.Length; i++)
        {
            if (_attackCoroutines[i] != null)
            {
                StopCoroutine(_attackCoroutines[i]);
                _attackCoroutines[i] = null;
            }
        }

        foreach (var t in _frontTrumps)
        {
            t.OnAnimation(name);
        }

        foreach (var t in _backTrumps)
        {
            t.OnAnimation(name);
        }

        foreach (var t in _leftTrumps)
        {
            t.OnAnimation(name);
        }

        foreach (var t in _rightTrumps)
        {
            t.OnAnimation(name);
        }
        _finishAttackAction?.Invoke();
        _finishAttackAction = null;
    }

    void SetupTrumpSolders()
    {
        int index = 0;
        for (int i = 0; i < _areaPositions.Length; i++)
        {
            for (int n = 0; n < _areaPositions[i].LineUpTrans.Length; n++)
            {
                var trump = _trumpSolderList[index];

                trump.transform.localPosition = _areaPositions[i].LineUpTrans[n].position;
                trump.DirType = _areaPositions[i].DirectionType;

                Vector3 dir = default;

                switch (_trumpSolderList[index].DirType)
                {
                    case DirectionType.Front:
                        dir.y = 0f;
                        _frontTrumps.Add(trump);
                        break;
                    case DirectionType.Back:
                        dir.y = 180f;
                        _backTrumps.Add(trump);
                        break;
                    case DirectionType.Left:
                        _leftTrumps.Add(trump);
                        dir.y = 90f;
                        break;
                    case DirectionType.Right:
                        _rightTrumps.Add(trump);
                        dir.y = 270f;
                        break;
                }
                trump.transform.DOLocalRotate(dir, 0f);
                index++;
                trump.gameObject.SetActive(false);
            }
        }
    }

    IEnumerator TrumpSoldersAttackCoroutine(DirectionType dir, Action start, Action finish)
    {
        switch (dir)
        {
            case DirectionType.Front:
                foreach (var t in _frontTrumps)
                {
                    t.OnAttack(_waitTime);
                }
                _attackRangeEffect[0].SetActive(true);
                break;
            case DirectionType.Back:
                foreach (var t in _backTrumps)
                {
                    t.OnAttack(_waitTime);
                }
                _attackRangeEffect[1].SetActive(true);
                break;
            case DirectionType.Left:
                foreach (var t in _leftTrumps)
                {
                    t.OnAttack(_waitTime);
                }
                _attackRangeEffect[2].SetActive(true);
                break;
            case DirectionType.Right:
                foreach (var t in _rightTrumps)
                {
                    t.OnAttack(_waitTime);
                }
                _attackRangeEffect[3].SetActive(true);
                break;
            default:
                break;
        }

        AudioManager.PlaySE(SEType.Trump_Waiting);
        yield return new WaitForSeconds(_waitTime); //トランプ兵の攻撃するまでのモーションを待機

        yield return new WaitForSeconds(0.3f);
        //当たり判定をONにする処理
        start?.Invoke();
        AudioManager.PlaySE(SEType.Trump_Slust);
        //攻撃範囲のエフェクトを全て非アクティブにする
        foreach (var effect in _attackRangeEffect)
        {
            effect.SetActive(false);
        }

        yield return new WaitForSeconds(_durationTime);

        finish?.Invoke(); //当たり判定をOFFにする

        AudioManager.PlaySE(SEType.Trump_AttackEnd);
        switch (dir)
        {
            case DirectionType.Front:
                foreach (var t in _frontTrumps)
                {
                    t.OnReturnToStandby();
                }
                break;
            case DirectionType.Back:
                foreach (var t in _backTrumps)
                {
                    t.OnReturnToStandby();
                }
                break;
            case DirectionType.Left:
                foreach (var t in _leftTrumps)
                {
                    t.OnReturnToStandby();
                }
                break;
            case DirectionType.Right:
                foreach (var t in _rightTrumps)
                {
                    t.OnReturnToStandby();
                }
                break;
        }
        yield return new WaitForSeconds(1.5f);
    }
}

[Serializable]
public struct BossAreaPositions
{
    public DirectionType DirectionType;
    public Transform[] LineUpTrans;
}
public enum DirectionType
{
    Front,
    Back,
    Left,
    Right
}
