using System;
using System.Collections;
using UnityEngine;

public class Stage3TrumpSolderManager : MonoBehaviour
{
    [Header("�g�����v���̑O��̊Ԋu")]
    [SerializeField]
    private float _forwardDistance;

    [Header("�g�����v���̃Y���X��")]
    [SerializeField]
    private float _gap;

    [Header("�΂߂ɕ��Ԏ��̊")]
    [SerializeField]
    private Transform _pointOfXSlant;

    [Header("������ɕ��Ԏ��̊")]
    [SerializeField]
    private Transform _pointOfStraight;

    [Header("�����ɕ��Ԏ��̊")]
    [SerializeField]
    private Transform _pointOfXrossLeft;

    [SerializeField]
    private Transform _pointOfXrossRight;

    [Space]
    [SerializeField]
    private Stage3ScoreConter _stage3ScoreConter;

    private Stage3TrumpSolderController[] _trumpsArray;

    // ����
    const float ROAD_WIDTH = 2;

    /// <summary>
    /// ���Ԏ��
    /// </summary>
    private enum LineUpPattern
    {
        // ����
        straight,
        // ����
        Xross,
        // �΂�
        Slant
    }

    private void Start()
    {
        _trumpsArray = GetComponentsInChildren<Stage3TrumpSolderController>();

        foreach (var item in _trumpsArray)
        {
            item.SetCounter(_stage3ScoreConter);
        }
    }

    /// <summary>
    /// ����̃��N�G�X�g
    /// </summary>
    public void RequestSetSolder()
    {
        Debug.Log("����");
        // ����̎�ނ��擾
        var num = UnityEngine.Random.Range(0, (int)LineUpPattern.Slant + 1);
        // ����
        SetSolder(num);
    }

    /// <summary>
    /// ����̏���
    /// </summary>
    private void SetSolder(int patternLength)
    {
        switch (patternLength)
        {
            case (int)LineUpPattern.straight:
                StartCoroutine(SetStraightAsync());
                break;
            case (int)LineUpPattern.Xross:
                StartCoroutine(SetXrossAsync());
                break;
            case (int)LineUpPattern.Slant:
                StartCoroutine(SetSlantAsync());
                break;
        }
    }

    /// <summary>
    /// �΂߂ɕ���
    /// </summary>
    private IEnumerator SetXrossAsync()
    {
        var leftPoint = _pointOfXrossLeft.position;
        var rightPoint = _pointOfXrossRight.position;

        for (int i = 0; i < _trumpsArray.Length; i++)
        {
            var trump = _trumpsArray[i];

            if (i % 2 == 0)
            {
                trump.GetComponent<TrumpSolder>().ChangeRandomPattern(TrumpColorType.Red);
                trump.gameObject.transform.position = leftPoint;
            }
            else
            {
                trump.GetComponent<TrumpSolder>().ChangeRandomPattern(TrumpColorType.Black);
                trump.gameObject.transform.position = rightPoint;
            }

            leftPoint.z = leftPoint.z + _forwardDistance;
            leftPoint.x = leftPoint.x + _gap;

            rightPoint.z = rightPoint.z + _forwardDistance;
            rightPoint.x = rightPoint.x - _gap;
        }

        yield return null;
    }

    /// <summary>
    /// �����ɕ���
    /// </summary>
    private IEnumerator SetStraightAsync()
    {
        var pos = _pointOfStraight.position;

        for (int index = 0; index < _trumpsArray.Length; index++)
        {
            var trump = _trumpsArray[index];

            if (index % 2 == 0)
            {
                // �g�����v�̎�ނ��w��
                trump.GetComponent<TrumpSolder>().ChangeRandomPattern(TrumpColorType.Red);
                _trumpsArray[index].SetCardType(CardType.Red);
            }
            else
            {
                // �g�����v�̎�ނ��w��
                trump.GetComponent<TrumpSolder>().ChangeRandomPattern(TrumpColorType.Black);
                _trumpsArray[index].SetCardType(CardType.Black);
            }

            // �g�����v�̐ݒ�
            trump.Reset();
            trump.gameObject.transform.position = pos;
            pos.z = pos.z + _forwardDistance;

            yield return null;
        }
    }

    /// <summary>
    /// �΂߂ɕ���
    /// </summary>
    private IEnumerator SetSlantAsync()
    {
        var pos = _pointOfXSlant.position;

        for (int index = 0; index < _trumpsArray.Length; index++)
        {
            _trumpsArray[index].gameObject.transform.position = pos;
            var card = _trumpsArray[index].gameObject.GetComponent<TrumpSolder>();
            if (index % 2 == 0)
            {
                // �g�����v�̎�ނ��w��
                card.ChangeRandomPattern(TrumpColorType.Red);
                _trumpsArray[index].SetCardType(CardType.Red);
            }
            else
            {
                // �g�����v�̎�ނ��w��
                card.ChangeRandomPattern(TrumpColorType.Black);
                _trumpsArray[index].SetCardType(CardType.Black);
            }
            pos.x = pos.x + _gap;
            pos.z = pos.z + _forwardDistance;
            yield return null;
        }
    }

    public void Reset()
    {
        foreach (var item in _trumpsArray)
        {
            item.gameObject.SetActive(true);
        }
    }
}