using DG.Tweening;
using System.Collections;
using UnityEngine;

public class MugcapController : MonoBehaviour
{
    [SerializeField]
    private float _duration;

    [SerializeField]
    private Transform _movePosition;

    /// <summary>
    /// ‰º‚ª‚é
    /// </summary>
    public IEnumerator DownAsync()
    {
        yield return transform.DOLocalMove(_movePosition.localPosition, _duration).AsyncWaitForCompletion();
    }
}
