using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageAble
{
    void OnDamaged(float dmg, int dir = 0, bool isKnockbackable = false);
}
