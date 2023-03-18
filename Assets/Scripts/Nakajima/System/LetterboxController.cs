using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LetterboxController : MonoBehaviour
{
    #region serialize
    [SerializeField]
    Transform[] _letterbox = default;
    #endregion

    #region private
    #endregion

    #region public
    #endregion
    #region property
    public static LetterboxController Instance { get; private set; }
    #endregion

    private void Awake()
    {
        Instance = this;
        ActivateLetterbox(false, 0);
    }

    public static void ActivateLetterbox(bool value, float time = 1.0f)
    {
        if (value)
        {
            Instance._letterbox[0].DOLocalMoveY(500, time);
            Instance._letterbox[1].DOLocalMoveY(-500, time);
        }
        else
        {
            Instance._letterbox[0].DOLocalMoveY(600, time);
            Instance._letterbox[1].DOLocalMoveY(-600, time);
        }
    }
}
