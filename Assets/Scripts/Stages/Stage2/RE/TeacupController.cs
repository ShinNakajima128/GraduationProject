using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeacupController : MonoBehaviour
{
    #region serialize
    [Header("Variables")]
    [Tooltip("各フェイズのパラメーター")]
    [SerializeField]
    ShuffleParameter[] _params = default;

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

    public IEnumerator ShuffleCoroutine(ShufflePhase phase, Teacup[] cups)
    {
        int pattern = 0;
        int currentShuffleCount = 0;

        switch (phase)
        {
            case ShufflePhase.One:
                currentShuffleCount = _params[0].ShuffleCount;
                break;
            case ShufflePhase.Two:
                currentShuffleCount = _params[1].ShuffleCount;
                break;
            case ShufflePhase.Three:
                currentShuffleCount = _params[2].ShuffleCount;
                break;
            default:
                break;
        }



        for (int i = 0; i < currentShuffleCount; i++)
        {
            int index = Random.Range(0, 6);

            switch (phase)
            {
                case ShufflePhase.One:
                    yield return _shuffler.ShuffleTeacup(cups, index, _params[0].ShuffleTime);
                    break;
                case ShufflePhase.Two:

                    pattern = Random.Range(0, 2);

                    if (pattern == 0)
                    {
                        yield return _shuffler.ShuffleTeacup(cups, index, _params[1].ShuffleTime);
                    }
                    else
                    {
                        yield return _shuffler.WarpTeacup(cups, index, _params[1].WarpTime);
                    }

                    break;
                case ShufflePhase.Three:

                    pattern = Random.Range(0, 2);

                    if (pattern == 0)
                    {
                        yield return _shuffler.ShuffleTeacup(cups, index, _params[2].ShuffleTime);
                    }
                    else
                    {
                        yield return _shuffler.WarpTeacup(cups, index, _params[2].WarpTime);
                    }
                    break;
                default:
                    break;
            }
        }
    }
}

[System.Serializable]
public struct ShuffleParameter
{
    public string PhaseName;
    public int ShuffleCount;
    public float ShuffleTime;
    public float WarpTime;
}
