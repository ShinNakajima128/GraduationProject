using DG.Tweening;
using System.Collections;

public class StageGameSystem
{
    public IEnumerator ShuffleAsync(MugcapController cap1, MugcapController cap2)
    {
        cap1.transform.DOLocalMove(cap2.transform.localPosition, 2f);
        cap2.transform.DOLocalMove(cap1.transform.localPosition, 2f);
        yield return null;
    }
}