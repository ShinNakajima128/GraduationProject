using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Obstacle : MonoBehaviour
{
    #region serialize
    [SerializeField]
    ObstacleType _obstacleType = default;

    [Tooltip("移動速度")]
    [SerializeField]
    float _moveSpeed = 5.0f;

    [Tooltip("非アクティブになるまでの時間")]
    [SerializeField]
    float _vanishTime = 20.0f;
    #endregion

    #region private
    Rigidbody _rb;
    bool _init = false;
    Vector3 _rotateValue = default;
    #endregion

    private void OnEnable()
    {
        StartCoroutine(OnVanishTimer());
        int randX = Random.Range(-1, 2);
        int randY = Random.Range(-1, 2);
        int randZ = Random.Range(-1, 2);
        _rotateValue = new Vector3(randX, randY, randZ);
    }
    private void OnDisable()
    {
        transform.localPosition = Vector3.zero;
        if (_init)
        {
            _rb.velocity = Vector3.zero;
        }
    }
    private void Start()
    {
        TryGetComponent(out _rb);
        _init = true;
    }

    private void FixedUpdate()
    {
        transform.Rotate(_rotateValue);
        _rb.AddForce(Vector3.up * _moveSpeed, ForceMode.Force);
    }

    /// <summary>
    /// 非アクティブするタイマーを開始
    /// </summary>
    /// <returns></returns>
    IEnumerator OnVanishTimer()
    {
        yield return new WaitForSeconds(_vanishTime);
        gameObject.SetActive(false);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            gameObject.SetActive(false);
        }
    }
}
