using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TrumpObstacle : MonoBehaviour
{
    #region serialize
    [SerializeField]
    float _pushPower = 2.0f;

    [SerializeField]
    Transform _parent = default;
    #endregion
    #region private
    ParticleSystem _effect;
    Rigidbody _rb;
    bool _init = false;
    #endregion
    #region public
    #endregion
    #region property
    #endregion

    private void Awake()
    {
        TryGetComponent(out _effect);
        _init = true;
    }
    private void OnEnable()
    {
        if (_init)
        {
            _effect.Play();
            //transform.localRotation = _parent.localRotation;
        }
    }

    private void OnDisable()
    {
        if (_init)
        {
            _effect.Stop();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (_rb == null)
            {
                _rb = other.GetComponent<Rigidbody>();
            }
            //_rb.AddForce(_pushPower * _parent.up, ForceMode.Impulse);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _rb.AddForce(_pushPower * _parent.up, ForceMode.Force);
        }
    }
}
