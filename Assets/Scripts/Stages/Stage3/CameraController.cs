using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private float _speed;

    [SerializeField]
    private Vector3 _stopPoint;

    private void Start()
    {
        MoveRequest();
    }

    public void MoveRequest()
    {
        StartCoroutine(MoveAsync());
    }

    private IEnumerator MoveAsync()
    {
        while (this.transform.position.z > _stopPoint.z)
        {
            var pos = this.transform.position;
            pos.z -= _speed;
            this.transform.position = pos;
            yield return null;
        }

        yield return null;
    }
}
