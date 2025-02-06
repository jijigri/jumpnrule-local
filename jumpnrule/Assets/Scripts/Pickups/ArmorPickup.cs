using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmorPickup : Pickup
{
    [SerializeField] private float _armorAmount = 10;

    protected override void OnPlayerTrigger()
    {
        if (_player.TryGetComponent(out ICanGetArmor iCanGetArmor))
        {
            iCanGetArmor.GiveArmor(_armorAmount, gameObject);
        }
    }
}
