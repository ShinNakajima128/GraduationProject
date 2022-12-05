using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class BossAreaController : MonoBehaviour
{
    #region serialize
    [Header("Objects")]
    [Tooltip("ƒgƒ‰ƒ“ƒv•º‚ÌUŒ‚”ÍˆÍ")]
    [SerializeField]
    AttackArea[] _attackAreas = default;

    [SerializeField]
    TrumpSolderManager _trumpSolderMng = default;
    #endregion
    #region private
    #endregion
    #region public
    #endregion
    #region property
    ReactiveProperty<bool> _frontArea;
    ReactiveProperty<bool> _backArea;
    ReactiveProperty<bool> _leftArea;
    ReactiveProperty<bool> _rightArea;
    #endregion
    void Start()
    {
        //_frontArea.AddTo(_attackAreas[0].IsAttacked);
        //_backArea.AddTo(_attackAreas[1]);
        //_leftArea.AddTo(_attackAreas[2]);
        //_rightArea.AddTo(_attackAreas[3]);

        //_frontArea.Subscribe(_ =>
        //{
            
        //});
    }
}
