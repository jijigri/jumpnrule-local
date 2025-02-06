using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedPickup : Pickup
{
    [SerializeField] private float _speedAmount = .5f;

    protected override void OnPlayerTrigger()
    {
        if (_player.TryGetComponent(out ICanIncreaseSpeed iCanIncreaseSpeed))
        {
            iCanIncreaseSpeed.GiveSpeed(_speedAmount, gameObject);
        }
    }
}
