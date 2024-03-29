using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;

public class LobbyDoor : MonoBehaviour
{
    #region serialize
    [Tooltip("�J�ڐ��Scene")]
    [SerializeField]
    SceneType _sceneType = default;

    [SerializeField]
    UnclearedIcon _icon = default;
    #endregion

    #region private
    ReactiveProperty<bool> _isVicinity = new ReactiveProperty<bool>(false);
    #endregion
    #region property
    #endregion

    private void Start()
    {
        _isVicinity.Subscribe(_ =>
        {
            if (_isVicinity.Value)
            {
                LobbyManager.OnStageDescription(_sceneType);
                if (_icon != null && !_icon.IsCleared)
                {
                    _icon.gameObject.SetActive(false);
                }
            }
            else
            {
                LobbyManager.OffStageDescription();
                if (_icon != null && !_icon.IsCleared)
                {
                    _icon.gameObject.SetActive(true);
                }
            }
        });
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _isVicinity.Value = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _isVicinity.Value = false;
        }
    }
}
