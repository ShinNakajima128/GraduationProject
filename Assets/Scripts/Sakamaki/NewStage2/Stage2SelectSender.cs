using UnityEngine;

public class Stage2SelectSender : MonoBehaviour
{
    [SerializeField]
    private Stage2GameManager _manager;

    public void SendSelectNumber(int currentSelectNum)
    {
        _manager.Judge(currentSelectNum);
    }
}