using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableController : MonoBehaviour, IEffectable
{
    #region serialize
    [SerializeField]
    Transform _effectTrans = default;

    [SerializeField]
    float _seInterval = 0.1f;

    [SerializeField]
    int _seCount = 5;

    [SerializeField]
    GameObject _chaseObject = default;

    [SerializeField]
    TrumpObstacle _tableTrump = default;

    [SerializeField]
    Animator _tableAnimator = default;
    #endregion
    #region private
    bool _isEffected;
    WaitForSeconds _wait;
    #endregion
    #region property
    #endregion

    public Transform EffectPos => _effectTrans;

    private void OnEnable()
    {
        _isEffected = false;
        transform.localPosition = Vector3.zero;
        StartCoroutine(SetupCoroutine());
    }

    void OnDisable()
    {
        _tableTrump.gameObject.SetActive(false);
        _tableAnimator.CrossFadeInFixedTime("Default", 0.1f);
    }
    private void Start()
    {
        _wait = new WaitForSeconds(_seInterval);
    }
    private void FixedUpdate()
    {
        transform.localPosition = _chaseObject.transform.localPosition;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Hit");

            if (!_isEffected)
            {
                _tableAnimator.CrossFadeInFixedTime("OnGimmick", 0.1f);
                _tableTrump.gameObject.SetActive(true);
                _tableTrump.OffPushing();
                StartCoroutine(SECoroutine());
                _isEffected = true;
            }
        }
    }

    IEnumerator SetupCoroutine()
    {
        yield return null;

        transform.localRotation = _chaseObject.transform.localRotation;
    }

    IEnumerator SECoroutine()
    {
        for (int i = 0; i < _seCount; i++)
        {
            AudioManager.PlaySE(SEType.Object_Scatter);
            yield return _wait;
        }
    }
}
