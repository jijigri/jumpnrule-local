using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CooldownManager))]
public abstract class PlayerAbility : MonoBehaviour
{
    [SerializeField] protected float _abilityEnergyCost = 10;

    protected PlayerAbilityManager _playerAbilityManager;
    protected CooldownManager _cooldownManager;
    protected GameObject _player;

    public virtual void OnAbilityEquipped(PlayerAbilityManager abilityManager)
    {
        _playerAbilityManager = abilityManager;
        _cooldownManager = GetComponent<CooldownManager>();
        _player = abilityManager.gameObject;
    }

    public virtual void OnAbilityPressed()
    {
       
    }

    public virtual void OnAbilityReleased()
    {
        _cooldownManager.OnKeyReleased();
    }

    public virtual void OnAbility()
    {
        if (_playerAbilityManager.CurrentEnergy >= _abilityEnergyCost)
        {
            if (_cooldownManager.CheckCooldown())
            {
                _playerAbilityManager.RemoveEnergy(_abilityEnergyCost, _playerAbilityManager.gameObject);
                UseAbility();
            }
        }
        else
        {
            Debug.Log("Current ability (PlayerAbility): Not enough energy to use current equipped ability");
        }
    }

    protected virtual void UseAbility()
    {
       
    }

    public float GetAbilityCost()
    {
        return _abilityEnergyCost;
    }
}
