using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class BossDoor : MonoBehaviour
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
                UnderLobbyManager.OnStageDescription(SceneType.Stage_Boss);
            }
            else
            {
                UnderLobbyManager.OffStageDescription();
            }
        });

        //this.UpdateAsObservable()
        //    .Where(_ => _isVicinity.Value && UIInput.Submit)
        //    .Subscribe(_ =>
        //    {
        //        TransitionManager.SceneTransition(_sceneType, FadeType.Mask_KeyHole);
        //    });
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
