using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���𐶐�����Component
/// </summary>
public class ButterflyGenerator : MonoBehaviour
{
    #region serialize
    [Header("Components")]
    [SerializeField]
    ButterflyManager _butterflyMng = default;
    #endregion

    #region private
    #endregion

    #region public
    #endregion

    #region property
    #endregion

    /// <summary>
    /// �o���̖؂ɐ�������
    /// </summary>
    /// <param name="trees"> �o���̖� </param>
    public void OnGenerate(RoseTree[] trees)
    {
        for (int i = 0; i < trees.Length; i++)
        {
            for (int n = 0; n < trees[i].CurrentRose.Length; n++)
            {
                int random = Random.Range(0, 2);

                //1/2�̊m���Œ��𐶐�
                if (random == 0)
                {
                    Debug.Log("������");

                    var type = trees[i].CurrentRose[n].CurrentRoseType;

                    switch (type)
                    {
                        //�o������\���̏ꍇ�̓����_���ɐF���w�肵�ďo��
                        case RoseType.Hidden:
                            int randomColor = Random.Range(0, 2);

                            if (randomColor == 0)
                            {
                                _butterflyMng.ActiveFlyingButterfly(ButterflyState.Idle, ButterflyColor.White, trees[i].CurrentRose[n].transform);
                            }
                            else
                            {
                                _butterflyMng.ActiveFlyingButterfly(ButterflyState.Idle, ButterflyColor.Red, trees[i].CurrentRose[n].transform);
                            }
                            break;
                        case RoseType.Red:
                            _butterflyMng.ActiveFlyingButterfly(ButterflyState.Idle, ButterflyColor.White, trees[i].CurrentRose[n].transform);
                            break;
                        case RoseType.White:
                            _butterflyMng.ActiveFlyingButterfly(ButterflyState.Idle, ButterflyColor.Red, trees[i].CurrentRose[n].transform);
                            break;
                        default:
                            break;
                    }
                }
            }

        }
    }

    /// <summary>
    /// �S�Ă̒����A�N�e�B�u�ɂ���
    /// </summary>
    public void Return()
    {
        _butterflyMng.Return();
    }
}
