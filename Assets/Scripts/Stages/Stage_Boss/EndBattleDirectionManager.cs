using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EndBattleDirectionManager : MonoBehaviour
{
    #region serialize
    [SerializeField]
    CheshireCatFaceController _cheshireFc = default;

    [SerializeField]
    Transform _aliceFrontCatPosTrans = default;

    [SerializeField]
    Transform _lookUpCatPosTrans = default;
    #endregion

    #region private
    CheshireCat _cat;
    #endregion

    #region public
    #endregion

    #region property
    #endregion

    private void Awake()
    {
        _cheshireFc.TryGetComponent(out _cat);
    }

    private void Start()
    {
        EventManager.ListenEvents(Events.BossStage_End_CheshireFront, () => 
        {
            _cat.transform.DOLocalRotate(new Vector3(0, -70, 0), 0f);
            _cheshireFc.ChangeFaceType(CheshireCatFaceType.Talking);
        });
        EventManager.ListenEvents(Events.BossStage_FrontCheshire, _cheshireFc.OnTaking);
        EventManager.ListenEvents(Events.BossStage_End_CheshireSmile, _cheshireFc.OnGrinning);
        EventManager.ListenEvents(Events.BossStage_End_AliceFloat2, () => 
        {
            _cat.ChangeState(CheshireCatState.Standing_Up);
        });
        //EventManager.ListenEvents(Events.BossStage_End_CheshireOverhead, () =>
        //{
        //    MoveCat();
        //});
        EventManager.ListenEvents(Events.BossStage_End_CheshireLookUp, () =>
        {
            LookUpMoveCat();
        });
    }

    void MoveCat()
    {
        _cheshireFc.transform.localPosition = _aliceFrontCatPosTrans.localPosition;
        _cheshireFc.transform.localRotation = _aliceFrontCatPosTrans.localRotation;
    }

    void LookUpMoveCat()
    {
        _cheshireFc.transform.localPosition = _lookUpCatPosTrans.localPosition;
        _cheshireFc.transform.localRotation = _lookUpCatPosTrans.localRotation;
    }
}
