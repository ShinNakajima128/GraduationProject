using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �A���X��HP�̕\���@�\�����R���|�[�l���g
/// </summary>
public class AliceHP : MonoBehaviour
{
    #region serialize
    [Tooltip("HP�̌��݂̃X�e�[�^�X")]
    [SerializeField]
    HPState _currentHPState = HPState.ON;

    [Tooltip("HP�̉摜�f�[�^")]
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
    /// HP�̃X�e�[�^�X��ύX����
    /// </summary>
    /// <param name="state"> �ύX��̃X�e�[�^�X </param>
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
/// HP�̃A�N�e�B�u��
/// </summary>
public enum HPState
{
    ON,
    OFF
}