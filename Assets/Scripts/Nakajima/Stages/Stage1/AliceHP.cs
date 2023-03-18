using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// アリスのHPの表示機能を持つコンポーネント
/// </summary>
public class AliceHP : MonoBehaviour
{
    #region serialize
    [Tooltip("HPの現在のステータス")]
    [SerializeField]
    HPState _currentHPState = HPState.ON;

    [Tooltip("HPの画像データ")]
    [SerializeField]
    HPData _data = default;
    #endregion

    #region private
    Image _hpImage = default;
    #endregion

    #region public
    #endregion

    #region property
    #endregion

    private void Awake()
    {
        TryGetComponent(out _hpImage);
    }

    private void Start()
    {
        ChangeState(HPState.ON);
    }

    /// <summary>
    /// HPのステータスを変更する
    /// </summary>
    /// <param name="state"> 変更先のステータス </param>
    public void ChangeState(HPState state)
    {
        _currentHPState = state;

        switch (_currentHPState)
        {
            case HPState.ON:
                _hpImage.sprite = _data.ONSprite;
                break;
            case HPState.OFF:
                _hpImage.sprite = _data.OFFSprite;
                break;
            default:
                break;
        }
    }
}

/// <summary>
/// HPのアクティブ状況
/// </summary>
public enum HPState
{
    ON,
    OFF
}