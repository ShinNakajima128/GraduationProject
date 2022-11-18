using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// オブジェクトの選択をする機能
/// </summary>
public class Stage2Selector : MonoBehaviour
{
    [SerializeField]
    private PlayerInput _input;

    [SerializeField]
    private GameObject[] _selectIcons;

    private int _currentSelect = 0;

    private void Awake()
    {
        foreach (var item in _selectIcons)
        {
            item.SetActive(false);
        }

        // _input.actions["SelectUp"].  = Increment;
    }

    public void PlaySelect()
    {
        _selectIcons[0].SetActive(true);
    }

    private void Increment()
    {

    }
}
