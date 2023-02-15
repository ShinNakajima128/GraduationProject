using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���r�[�̃`�F�V���L���Ǘ�����Manager�N���X
/// </summary>
public class LobbyCheshireCatManager : MonoBehaviour
{
    #region serialize
    [Header("Objects")]
    [Tooltip("���r�[�̃`�F�V���L�f�[�^")]
    [SerializeField]
    LobbyCheshireCat[] _cheshireCats = default;

    [Header("Components")]
    [Tooltip("���r�[�𓮂����`�F�V���L�̃R���|�[�l���g")]
    [SerializeField]
    CheshireCat _cheshireCat = default;

    [Tooltip("���r�[�𓮂����`�F�V���L�̍s�������̃R���|�[�l���g")]
    [SerializeField]
    CheshireCatBehaviour _catBehaviour = default;

    [Header("Debug")]
    [SerializeField]
    bool _debugMode = false;
    #endregion

    #region private
    /// <summary> �������`�F�V���L��Component </summary>
    CheshireCat _movableCat = default;
    /// <summary> ���r�[�̃`�F�V���L���A�N�e�B�u�ɂȂ������̃A�N�V���� </summary>
    Dictionary<LobbyCheshireCatType, Action> _activateActionDic = new Dictionary<LobbyCheshireCatType, Action>();
    #endregion

    #region public
    #endregion

    #region property
    public static LobbyCheshireCatManager Instance { get; private set; }
    public CheshireCat MovableCat => _movableCat;
    #endregion

    private void Awake()
    {
        Instance = this;

        for (int i = 0; i < _cheshireCats.Length; i++)
        {
            _activateActionDic.Add((LobbyCheshireCatType)i, null);
        }

        //���r�[�𓮂����`�F�V���L�̋@�\������Component���擾
        _cheshireCats.FirstOrDefault(c => c.CatType == LobbyCheshireCatType.Movable)
                     .CheshireCat.TryGetComponent(out _movableCat);
    }

    IEnumerator Start()
    {
        //�������`�F�V���L�̃A�N�e�B�u���̃A�N�V������o�^
        _activateActionDic[LobbyCheshireCatType.Movable] += () => _movableCat.ChangeState(CheshireCatState.Idle);

        if (_debugMode)
        {
            ActiveCheshireCat(LobbyCheshireCatType.Movable);
        }

        yield return null;

        foreach (var cat in _cheshireCats)
        {
            cat.CheshireCat.SetActive(false);
        }
    }

    /// <summary>
    /// �w�肵���`�F�V���L���A�N�e�B�u�ɂ���
    /// </summary>
    /// <param name="type"> �`�F�V���L�I�u�W�F�N�g�̎�� </param>
    public void ActiveCheshireCat(LobbyCheshireCatType type, bool isMoving = false)
    {
        foreach (var cat in _cheshireCats)
        {
            cat.CheshireCat.SetActive(false);
        }

        var activeCat = _cheshireCats.FirstOrDefault(c => c.CatType == type).CheshireCat;
        
        activeCat.SetActive(true);

        if (type == LobbyCheshireCatType.Movable) 
        {
            if (isMoving)
            {
                _catBehaviour.StartMoving();
            }
        }

        print($"{type}");
    }
}

/// <summary>
/// ���r�[�̃`�F�V���L�f�[�^
/// </summary>
[Serializable]
public struct LobbyCheshireCat
{
    public string TypeName;
    public LobbyCheshireCatType CatType;
    public GameObject CheshireCat;
}

/// <summary>
/// ���r�[�̃`�F�V���L�̎��
/// </summary>
public enum LobbyCheshireCatType
{
    /// <summary> ���o </summary>
    Appearance,
    /// <summary> ���r�[�������� </summary>
    Movable
}
