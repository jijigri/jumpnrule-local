using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyPickup : Pickup
{
    [SerializeField] private float _energyAmount = 10;

    protected override void OnPlayerTrigger()
    {
        if (_player.TryGetComponent(out IHasEnergy iHasEnergy))
        {
            iHasEnergy.GiveEnergy(_energyAmount, gameObject);
        }
    }
}