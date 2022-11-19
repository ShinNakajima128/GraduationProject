using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamagable
{
    bool IsInvincibled { get; }
    void Damage(int value);
}
