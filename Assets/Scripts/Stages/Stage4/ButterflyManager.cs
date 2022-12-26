using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stage4�̒����Ǘ�����}�l�[�W���[�N���X
/// </summary>
public class ButterflyManager : MonoBehaviour
{
    #region serialize
    [Tooltip("���̐�����")]
    [SerializeField]
    int _butterflyGenerateNum = 10;

    [Tooltip("���̐e�I�u�W�F�N�g��Transform")]
    [SerializeField]
    Transform _butterflyParent = default;

    [Header("Components")]
    [SerializeField]
    Butterfly _butterflPrefab = default;
    #endregion

    #region private
    /// <summary> ����ComponentList </summary>
    List<Butterfly> _butterflyList = new List<Butterfly>();
    #endregion

    #region public
    #endregion

    #region property
    #endregion

    private void Awake()
    {
        for (int i = 0; i < _butterflyGenerateNum; i++)
        {
            var butterfly = Instantiate(_butterflPrefab, _butterflyParent);
            butterfly.transform.localPosition = Vector3.zero;
            _butterflyList.Add(butterfly);
            butterfly.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// �����A�N�e�B�u�ɂ���
    /// </summary>
    /// <param name="state"> ���̃X�e�[�^�X </param>
    /// <param name="color"> ���̐F </param>
    /// <param name="target"> �A�N�e�B�u�ɂ���ʒu </param>
    public void ActiveFlyingButterfly(ButterflyState state, ButterflyColor color, Transform target)
    {
        foreach (var b in _butterflyList)
        {
            b.gameObject.SetActive(true);
            b.transform.localPosition = target.position;
            b.ChangeMaterial(color);
            b.ChangeState(state);
        }
    }
}
