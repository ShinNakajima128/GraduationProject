using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackArea : MonoBehaviour
{
    #region serialize
    [Tooltip("”ÍˆÍ‚ÌˆÊ’u")]
    [SerializeField]
    DirectionType _directionType = default;

    [Tooltip("UŒ‚”ÍˆÍ")]
    [SerializeField]
    BoxCollider _attackArea = default;

    [Header("Debug")]
    [SerializeField]
    TrumpSolderManager _trumpMng = default;

    [SerializeField]
    Color _gizmoColor = default;
    #endregion
    #region private
    Mesh _cubeMesh;
    #endregion
    #region public
    #endregion
    #region property
    public BoxCollider AttackAreaCollider => _attackArea;
    public bool IsAttacked { get; set; } = false;
    #endregion

    private void Awake()
    {
        _cubeMesh = GetPrimitiveMesh(PrimitiveType.Cube);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            IsAttacked = true;
            _trumpMng.OnTrumpSoldersAttack(_directionType);
        }
    }

    #region reference https://taka8.hateblo.jp/entry/2016/09/01/140055
    private void OnDrawGizmos()
    {
        if (_attackArea && _attackArea.enabled)
        {
            Vector3 offset = transform.right * _attackArea.center.x + transform.up * _attackArea.center.y + transform.forward * _attackArea.center.z;

            DrawMesh(_cubeMesh, offset, transform.localScale);
            Gizmos.color = _gizmoColor;
        }
    }

    private Mesh GetPrimitiveMesh(PrimitiveType type)
    {

        GameObject gameObject = GameObject.CreatePrimitive(type);
        Mesh mesh = gameObject.GetComponent<MeshFilter>().sharedMesh;
        DestroyImmediate(gameObject);
        return mesh;
    }

    private void DrawMesh(Mesh mesh, Vector3 positionOffset, Vector3 scale)
    {
        Gizmos.DrawMesh(mesh, transform.position + positionOffset, transform.rotation, scale);
    }
    #endregion
}
