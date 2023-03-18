using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHealable
{
    bool IsMaxHP { get; }
    void Heal(int value);
}
