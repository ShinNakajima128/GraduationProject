using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// ステージ2のティーカップをシャッフルするController
/// </summary>
public class ShuffuleController : MonoBehaviour
{
    [Tooltip("シャッフルする回数")]
    [SerializeField]
    int _shuffuleCount = 10;

    [Tooltip("シャッフルの初期スピード")]
    [SerializeField]
    float _initualShuffuleSpeed = 2.0f;

    [Tooltip("シャッフル時間を変化させる値")]
    [SerializeField]
    float _changeshuffuleTimeValue = 2f;

    #region serialize
    [Tooltip("ティーカップ")]
    [SerializeField]
    List<Transform> _teacups = new List<Transform>();

    [Tooltip("ティーカップをまとめた親オブジェクトのTransform")]
    [SerializeField]
    Transform _teacupsTrans = default;

    [Tooltip("ティーカップに隠れるキャラクター")]
    [SerializeField]
    Transform _hidingCharacter = default;

    [Tooltip("隠れる位置")]
    [SerializeField]
    Transform[] _hidingPositions = default;
    #endregion

    #region private
    float _currentShuffuleSpeed;
    #endregion

    private void Start()
    {
        TeapotGameManager.Instance.GameStart += () => StartCoroutine(StartShuffule());
        TeapotGameManager.Instance.OpenTeacupPhase += OpenTeacup;
        TeapotGameManager.Instance.Initialize += InitializeObject;

        _currentShuffuleSpeed = _initualShuffuleSpeed;
    }
    /// <summary>
    /// シャッフルする
    /// </summary>
    /// <param name="speed"> 入れ替える速度 </param>
    void Shuffule(float speed)
    {
        bool isSet = false; //セット完了のフラグ
        int rand1 = 0;
        int rand2 = 0;

        while (!isSet)
        {
            rand1 = Random.Range(0, _teacups.Count); //シャッフルする1つ目のターゲット
            rand2 = Random.Range(0, _teacups.Count); //シャッフルする2つ目のターゲット

            if (rand1 != rand2)　//各値が違うものになったらループから抜ける
            {
                isSet = true;
            }
        }
        var pos1 = _teacups[rand1].position;
        var pos2 = _teacups[rand2].position;

        _teacups[rand1].DOMove(pos2, speed);
        _teacups[rand2].DOMove(pos1, speed);

        (_teacups[rand1], _teacups[rand2]) = (_teacups[rand2], _teacups[rand1]); //Tupleを利用して入れ替える
    }

    /// <summary>
    /// シャッフル開始
    /// </summary>
    /// <returns></returns>
    IEnumerator StartShuffule()
    {
        int pos = Random.Range(0, _hidingPositions.Length); //初期位置をランダムで決める
        _hidingCharacter.position = _hidingPositions[pos].position;

        var corect = _teacups[pos];

        yield return new WaitForSeconds(1.0f);
        
        //カップを降ろしてキャラクターを隠す
        _teacupsTrans.DOMoveY(0.5f, 1.0f)
                     .OnComplete(() =>
                     {
                         _hidingCharacter.SetParent(_teacups[pos]);
                     });

        yield return new WaitForSeconds(2.0f);

        //指定した回数分シャッフルする
        for (int i = 0; i < _shuffuleCount; i++)
        {
            Shuffule(_currentShuffuleSpeed);

            yield return new WaitForSeconds(_currentShuffuleSpeed);
            

            if (i < _shuffuleCount / 2)
            {
                _currentShuffuleSpeed /= _changeshuffuleTimeValue; //徐々に速くする
            }
            else
            {
                _currentShuffuleSpeed *= _changeshuffuleTimeValue; //徐々に遅くする
            }
        }

        //正解のカップを調べる
        for (int i = 0; i < _teacups.Count; i++)
        {
            if (_teacups[i] == corect)
            {
                TeapotGameManager.Instance.OnSelectTeacup(i);
                break;
            }
        }
        Debug.Log("シャッフル完了");
    }
    /// <summary>
    /// オブジェクトの配置を初期化
    /// </summary>
    void InitializeObject()
    {
        _teacupsTrans.DOMoveY(1.5f, 0f);

        foreach (var t in _teacups)
        {
            t.DOLocalMoveY(0f, 0f);
        }
    }
    /// <summary>
    /// ティーカップの中身を見せる
    /// </summary>
    /// <param name="selectNum"> 見せるカップの番号 </param>
    /// <param name="result"> 結果 </param>
    void OpenTeacup(int selectNum, bool result)
    {
        _hidingCharacter.parent = null;
        
        //はずれた場合はあたりの位置を見せる
        if (!result)
        {
            _teacups[selectNum].DOMoveY(2f, 1f)
                               .OnComplete(() => 
                               {
                                   for (int i = 0; i < _teacups.Count; i++)
                                   {
                                       if (i == selectNum)
                                       {
                                           continue;
                                       }
                                       _teacups[i].DOMoveY(2f, 0.5f);
                                   }
                               });
        }
        else
        {
            _teacups[selectNum].DOMoveY(2f, 1f);
        }
    }
}
