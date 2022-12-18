using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassGenerator : MonoBehaviour
{
    #region serialize
    [Header("Variables")]
    [SerializeField]
    int _generateNum = 100;

    [SerializeField]
    float _generateInterval_x = 0.3f;

    [SerializeField]
    Vector2 _generateHeight_y = default;

    [SerializeField]
    Vector2 _generateDepth_z = default;

    [SerializeField]
    GameObject _grassPrefab = default;

    [SerializeField]
    Transform _grassParent = default;
    #endregion

    #region private
    #endregion
    #region public
    #endregion
    #region property
    #endregion

    void Start()
    {
        for (int i = 0; i < _generateNum; i++)
        {
            float y = Random.Range(_generateHeight_y.x, _generateHeight_y.y);
            float z = Random.Range(_generateDepth_z.x, _generateDepth_z.y);

            var grass = Instantiate(_grassPrefab, _grassParent);

            grass.transform.localPosition = new Vector3(grass.transform.localPosition.x + _generateInterval_x * i, y, z);
        }
    }
}
