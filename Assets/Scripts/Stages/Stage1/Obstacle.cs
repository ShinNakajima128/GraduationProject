using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Rigidbody))]
///障害物の機能を持つクラス
public class Obstacle : MonoBehaviour
{
    #region serialize
    [Tooltip("障害物の種類")]
    [SerializeField]
    ObstacleType _obstacleType = default;

    [Tooltip("回転の種類")]
    [SerializeField]
    RotateType _rotateType = default;

    [Tooltip("移動速度")]
    [SerializeField]
    float _moveSpeed = 5.0f;

    [Tooltip("非アクティブになるまでの時間")]
    [SerializeField]
    float _vanishTime = 20.0f;

    [SerializeField]
    GameObject _obstacleObject = default;
    #endregion

    #region private
    Rigidbody _rb;
    bool _init = false;
    Quaternion _rotateValue = default;
    IDamagable _target;
    #endregion

    private void OnEnable()
    {
        StartCoroutine(OnVanishTimer());

        int randX = 0;
        int randY = 0;
        int randZ = 0;

        switch (_rotateType)
        {
            case RotateType.None:
                break;
            case RotateType.Random:
                randX = Random.Range(0, 360);
                randY = Random.Range(0, 360);
                randZ = Random.Range(0, 360);
                break;
            case RotateType.Mirror:
                randY = Random.Range(135, 245);
                break;
            case RotateType.Table:
                randZ = Random.Range(-120, 120);
                break;
            default:
                break;
        }
        transform.DOLocalRotate(new Vector3(randX, randY, randZ), 0f);
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
        _rb.AddForce(Vector3.up * _moveSpeed, ForceMode.Force);
    }

    /// <summary>
    /// 非アクティブするタイマーを開始
    /// </summary>
    /// <returns></returns>
    IEnumerator OnVanishTimer()
    {
        yield return new WaitForSeconds(_vanishTime);
        _obstacleObject.SetActive(false);
        //gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (_target == null)
            {
                _target = other.GetComponent<IDamagable>();
            }
            
            if (!_target.IsInvincibled)
            {
                _target.Damage(1);
                AudioManager.PlaySE(SEType.Player_Damage);
                _obstacleObject.SetActive(false);
                //gameObject.SetActive(false);
            }
        }
    }

    enum RotateType
    {
        None,
        Random,
        Mirror,
        Table
    }
}
