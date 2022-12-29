using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleAreaTrumpSolder : TrumpSolder
{
    #region serialize
    [Tooltip("配置された位置")]
    [SerializeField]
    DirectionType _dirType = default;
    #endregion
    #region private
    private Animator _anim;
    #endregion
    #region public
    #endregion
    #region property
    public DirectionType DirType { get => _dirType; set => _dirType = value; }
    #endregion

    protected override void Awake()
    {
        base.Awake();
        TryGetComponent(out _anim);
    }

    protected override void Start()
    {
        base.Start();
    }

    /// <summary>
    /// 攻撃をする
    /// </summary>
    /// <param name="waitTime"> 構えてから待機する時間 </param>
    /// <param name="action"> 槍を突き出した瞬間のアクション </param>
    public void OnAttack(float waitTime, Action action = null)
    {
        StartCoroutine(AttackCoroutine(waitTime, action));
    }

    /// <summary>
    /// 待機状態に戻る
    /// </summary>
    public void OnReturnToStandby()
    {
        _anim.CrossFadeInFixedTime("Attack_Return", 0.1f);
    }

    /// <summary>
    /// 焦るモーションを再生
    /// </summary>
    public void OnAnimation(string name)
    {
        _anim.CrossFadeInFixedTime(name, 0.1f);
    }

    /// <summary>
    /// トランプ兵の攻撃処理のコルーチン
    /// </summary>
    /// <param name="waitTime"> 構えてから待機する時間 </param>
    /// <param name="action"> 槍を突き出した瞬間のアクション </param>
    /// <returns></returns>
    IEnumerator AttackCoroutine(float waitTime, Action action)
    {
        _anim.CrossFadeInFixedTime("Attack_Setup", 0.1f);

        yield return new WaitForSeconds(waitTime);

        _anim.CrossFadeInFixedTime("Attack_Start", 0.1f);

        yield return new WaitForSeconds(0.3f); //前に突きだすまでのモーション時間を待機

        action?.Invoke();
    }
}
