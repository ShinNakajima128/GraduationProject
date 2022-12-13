using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rose : MonoBehaviour
{
    #region serialize
    [SerializeField]
    GameObject[] _roseObjects = default;
    
    [SerializeField]
    RoseType _currentRoseType = default;
    #endregion
    #region private
    #endregion
    #region property
    public RoseType CurrentRoseType => _currentRoseType;
    #endregion

    /// <summary>
    /// ƒoƒ‰‚Ìó‘Ô‚ğİ’è‚·‚é
    /// </summary>
    public void SetRoseType()
    {
        RoseType rand = (RoseType)Random.Range(0, 3);

        switch (rand)
        {
            case RoseType.Hidden:
                _roseObjects[0].SetActive(false);
                _roseObjects[1].SetActive(false);
                break;
            case RoseType.Red:
                _roseObjects[0].SetActive(true);
                _roseObjects[1].SetActive(false);
                break;
            case RoseType.White:
                _roseObjects[0].SetActive(true);
                _roseObjects[1].SetActive(false);
                break;
            default:
                break;
        }
        _currentRoseType = rand;
    }
}
public enum RoseType
{
    Hidden,
    Red,
    White
}
