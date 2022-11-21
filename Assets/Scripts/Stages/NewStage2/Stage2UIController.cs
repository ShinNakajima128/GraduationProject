using UnityEngine;

public class Stage2UIController : MonoBehaviour
{
    public enum UIType
    {
        GameClear,
        Miss,
        Clear
    }

    [SerializeField]
    private GameObject _gameClearPanel;

    [SerializeField]
    private GameObject _missPanel;

    [SerializeField]
    private GameObject _clearPanel;

    [SerializeField]
    private RectTransform _pointerImage;

    /// <summary>
    /// ポインターの移動
    /// </summary>
    public void PointerMove(Vector2 value)
    {
        var currentPos = _pointerImage.position;
        currentPos.x += value.x;
        currentPos.y += value.y;

        if (value.x < -1 * Screen.width / 2)
        {
            value.x = -1 * Screen.width / 2;
        }
        else if (value.x > Screen.width / 2)
        {
            value.x = Screen.width / 2;
        }

        if (value.y < -1 * Screen.height / 2)
        {
            value.y = -1 * -Screen.height / 2;
        }
        else if (value.y > Screen.height / 2)
        {
            value.y = Screen.height / 2;
        }

        _pointerImage.position = value;
    }

    /// <summary>
    /// UIの表示切り替え
    /// </summary>
    public void ChengeActivate(UIType type, bool active)
    {
        switch (type)
        {
            case UIType.GameClear:
                _gameClearPanel.SetActive(active);
                break;
            case UIType.Miss:
                _missPanel.SetActive(active);
                break;
            case UIType.Clear:
                _clearPanel.SetActive(active);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// UIを全て非表示にする
    /// </summary>
    public void SetDisable()
    {
        _gameClearPanel.SetActive(false);
        _clearPanel.SetActive(false);
        _missPanel.SetActive(false);
    }
}
