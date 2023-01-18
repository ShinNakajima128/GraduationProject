using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;

public class LobbyDoor : MonoBehaviour
{
    #region serialize
    [Tooltip("ëJà⁄êÊÇÃScene")]
    [SerializeField]
    SceneType _sceneType = default;
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
            }
            else
            {
                LobbyManager.OffStageDescription();
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
