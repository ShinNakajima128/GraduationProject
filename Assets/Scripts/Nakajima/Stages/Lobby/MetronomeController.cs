using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetronomeController : MonoBehaviour
{
    #region serialize
    [SerializeField]
    float _seInterval = 1.0f;
    #endregion

    #region private
    AudioSource _audioSource;
    #endregion

    #region public
    #endregion

    #region property
    #endregion

    private void Awake()
    {
        TryGetComponent(out _audioSource);
    }

    private void Start()
    {
        StartCoroutine(OnPlaySECoroutine());
    }

    IEnumerator OnPlaySECoroutine()
    {
        var interval = new WaitForSeconds(_seInterval);

        while (true)
        {
            _audioSource.Play();

            yield return interval;
        }
    }
}
