using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICanHeal
{
    public void Heal(float healAmount, GameObject source, bool giveOverflow = false);
}
