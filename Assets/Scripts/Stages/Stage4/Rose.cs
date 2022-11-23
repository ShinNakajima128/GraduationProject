using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rose : MonoBehaviour
{
    #region serialize
    [SerializeField]
    RoseData _spriteData = default; 
    [SerializeField]
    RoseType _currentRoseType = default;
    #endregion
    #region private
    SpriteRenderer _sr;
    #endregion
    #region property
    public RoseType CurrentRoseType => _currentRoseType;
    #endregion
    
    private void Awake()
    {
        TryGetComponent(out _sr);
    }

    public void SetRoseType()
    {
        
        RoseType rand = (RoseType)Random.Range(0, 3);

        switch (rand)
        {
            case RoseType.Hidden:
                _sr.enabled = false;
                break;
            case RoseType.Red:
                _sr.enabled = true;
                _sr.sprite = _spriteData.RoseSprites[0];
                break;
            case RoseType.White:
                _sr.enabled = true;
                _sr.sprite = _spriteData.RoseSprites[1];
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
