using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeacupManager : MonoBehaviour
{
    #region serialize
    [Header("Objects")]
    [SerializeField]
    Teacup[] _teacups = default;

    [SerializeField]
    DorMouse _mouse = default;
    #endregion

    #region private
    #endregion
    
    #region public
    #endregion
    
    #region property
    #endregion

    void Start()
    {
        
    }

    /// <summary>
    /// ランダムでネズミを移動する
    /// </summary>
    public void RandomHideMouse()
    {
        int index = Random.Range(0, _teacups.Length);

        _mouse.transform.position = _teacups[index].MousePos;
        _mouse.transform.SetParent(_teacups[index].transform);
        
    }

    public void AllCupDown()
    {
        foreach (var c in _teacups)
        {
            c.DownCup();
        }
    }

    public void OnMouseAnimation(MouseState state, float animTime = 0.3f)
    {
        _mouse.OnAnimation(state, animTime);

    }
}
