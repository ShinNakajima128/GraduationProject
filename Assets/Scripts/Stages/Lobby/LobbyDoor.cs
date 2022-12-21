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

    [SerializeField]
    GameObject _submitIcon = default;
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
                _submitIcon.SetActive(true);
            }
            else
            {
                LobbyManager.OffStageDescription();
                _submitIcon.SetActive(false);
            }
        });

        this.UpdateAsObservable()
            .Where(_ => _submitIcon.activeSelf && UIInput.Submit)
            .Subscribe(_ =>
            {
                TransitionManager.FadeIn(FadeType.Black_TransParent, 0f);
                TransitionManager.SceneTransition(_sceneType);
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
