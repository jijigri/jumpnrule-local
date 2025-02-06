using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoCooldown : CooldownManager
{
    [SerializeField] private int _numberOfAmmos = 3;
    [SerializeField] private float _timeToRechargeAmmo = 2f;

    private int _numberOfAmmosLeft = 0;
    private float _timeToRechargeLeft = 0;

    private bool IsQueued;

    protected override void Start()
    {
        _numberOfAmmosLeft = _numberOfAmmos;
    }

    protected override void Update()
    {
        base.Update();

        if (_numberOfAmmosLeft < _numberOfAmmos)
        {
            _timeToRechargeLeft -= Time.deltaTime;

            if (_timeToRechargeLeft <= 0)
            {
                _numberOfAmmosLeft++;
                _timeToRechargeLeft = _timeToRechargeAmmo;
            }
        }

        if (Input.GetKey(KeyCode.B))
        {
            CheckCooldown();
        }
    }

    public override bool CheckCooldown()
    {
        if (_useType == UseType.MANUAL && !hasKeyBeenReleasedSinceLastShot)
        {
            return false;
        }

        bool canUse = false;
        if (_timeSinceLastUse >= 60.0f)
        {
            if (_numberOfAmmosLeft > 0)
            {
                _numberOfAmmosLeft--;
                _timeSinceLastUse = 0;

                hasKeyBeenReleasedSinceLastShot = false;
                canUse = true;
            }
        }

        return canUse;
    }
}
