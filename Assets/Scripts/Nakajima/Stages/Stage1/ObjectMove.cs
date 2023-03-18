using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMove : MonoBehaviour
{
    [SerializeField]
    float _moveSpeed = 2.0f;

    Rigidbody _rb;
    bool _init = false;
    IObjectable _childObject;

    private void OnEnable()
    {
        if (_init)
        {
            StartCoroutine(VanishCoroutine());
        }
    }

    private void OnDisable()
    {
        if (_init)
        {
            _rb.velocity = Vector3.zero;
        }
    }

    IEnumerator Start()
    {
        if (!_init)
        {
            TryGetComponent(out _rb);
            _childObject = GetComponentInChildren<IObjectable>();
            _init = true;

            yield return null;

            StartCoroutine(VanishCoroutine());
        }

    }

    private void FixedUpdate()
    {
        _rb.AddForce(Vector3.up * _moveSpeed, ForceMode.Force);
    }
    IEnumerator VanishCoroutine()
    {

        while (_childObject.IsActive)
        {
            yield return null;
        }
        gameObject.SetActive(false);
    }
}
