using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICanBeDamaged
{
    public void DealDamage(float damageAmount, GameObject source, Vector2 hitVelocity, DamageType damageType = DamageType.DIRECT);
}