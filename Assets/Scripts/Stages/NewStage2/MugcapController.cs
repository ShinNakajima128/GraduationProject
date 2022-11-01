using DG.Tweening;
using UnityEngine;

public class MugcapController : MonoBehaviour
{
    [SerializeField]
    private float _duration;

    [SerializeField]
    private Transform _movePosition;

    /// <summary>
    /// ������
    /// </summary>
    public void Down()
    {
        transform.DOLocalMove(_movePosition.localPosition, _duration);
    }
}
