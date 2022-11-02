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
    }
    private void Start()
    {
        _wait = new WaitForSeconds(_seInterval);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (!_isEffected)
            {
                EffectManager.PlayEffect(EffectType.Obstacle, EffectPos.position);
                StartCoroutine(SECoroutine());
                _isEffected = true;
            }
        }
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
