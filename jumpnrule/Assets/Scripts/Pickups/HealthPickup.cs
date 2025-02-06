using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : Pickup
{
    [SerializeField] private float _healthAmount = 4;
    [SerializeField] private bool _giveOverflow = false;

    protected override void OnPlayerTrigger()
    {
        if (_player.TryGetComponent(out ICanHeal iCanHeal))
        {
            iCanHeal.Heal(_healthAmount, gameObject, _giveOverflow);
        }
    }
}
