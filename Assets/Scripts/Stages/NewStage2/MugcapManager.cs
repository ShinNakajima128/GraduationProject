using UnityEngine;

public class MugcapManager : MonoBehaviour
{
    [SerializeField]
    private MugcapController[] _mugArray;

    public void DownRequest()
    {
        foreach (var item in _mugArray)
        {
            item.Down();
        }
    }
}
