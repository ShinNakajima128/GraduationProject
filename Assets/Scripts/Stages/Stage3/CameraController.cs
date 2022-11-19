using System;
using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private float _speed;

    [SerializeField]
    private Vector3 _stopPoint;

    public void MoveRequest(Action action = null)
    {
        StartCoroutine(MoveAsync(action));
    }

    private IEnumerator MoveAsync(Action action = null)
    {
        while (this.transform.position.z > _stopPoint.z)
        {
            var pos = this.transform.position;
            pos.z -= _speed;
            this.transform.position = pos;
            yield return null;
        }
        // ˆÚ“®‚ªI‚í‚Á‚½
        action();
        yield return null;
    }
}
