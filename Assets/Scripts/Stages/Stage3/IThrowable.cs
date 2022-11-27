using UnityEngine;

public interface IThrowable
{
    /// <summary>
    /// 投げる
    /// </summary>
    void Throw(Vector3 throwPos,Quaternion dir);
}