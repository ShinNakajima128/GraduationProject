using System.Collections;
using UnityEngine;

public class CardmanManager : MonoBehaviour
{
    [SerializeField]
    private CardManController[] _cardmans;

    [SerializeField]
    private float _distance;

    private float _posZ;

    private IEnumerator Start()
    {
        _posZ = _cardmans[0].gameObject.transform.position.z;

        yield return null;
        
        for (int i = 0; i < _cardmans.Length; i++)
        {
            var pos = _cardmans[i].gameObject.transform.position;
            pos.z = _posZ;
            _cardmans[i].gameObject.transform.position = pos;
            _posZ += _distance;
        }

        for (int i = 0; i < _cardmans.Length; i++)
        {
            if (i % 2 == 0)
            {
                var card = _cardmans[i].gameObject.GetComponent<TrumpSolder>();
                card.ChangeRandomPattern(TrumpColorType.Red);
            }
            else
            {
                var card = _cardmans[i].gameObject.GetComponent<TrumpSolder>();
                card.ChangeRandomPattern(TrumpColorType.Black);
            }

        }
    }
}
