using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeacupController : MonoBehaviour
{
    #region serialize
    [Header("Variables")]
    [Tooltip("ƒVƒƒƒbƒtƒ‹‚·‚é‰ñ”")]
    [SerializeField]
    int _shuffleCount = 8;

    [Header("components")]
    [SerializeField]
    TeacupShuffler _shuffler = default;
    #endregion
    #region private
    #endregion
    #region public
    #endregion
    #region property
    #endregion

    public IEnumerator ShuffleCoroutine(ShufflePhase phase)
    {
        for (int i = 0; i < _shuffleCount; i++)
        {
            int index = Random.Range(0, 6);

            yield return _shuffler.ShuffleTeacup(null, index);
        }
    }
}
