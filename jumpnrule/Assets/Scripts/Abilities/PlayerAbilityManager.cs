using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAbilityManager : MonoBehaviour, IHasEnergy
{
    public PlayerAbility[] equippedAbilities = new PlayerAbility[2];
    [SerializeField] public float maxEnergy = 120f;

    //20 = one bar
    public float CurrentEnergy { get; private set; }
    public void GiveEnergy(float energyAmount, GameObject source)
    {
        CurrentEnergy += energyAmount;

        if(CurrentEnergy > maxEnergy)
        {
            CurrentEnergy = maxEnergy;
        }

        GameEvents.Instance.PlayerGetEnergy(energyAmount, this);
    }
    public void RemoveEnergy(float energyAmount, GameObject source)
    {
        CurrentEnergy -= energyAmount;

        if (CurrentEnergy < 0)
        {
            CurrentEnergy = 0;
        }

        GameEvents.Instance.PlayerConsumeEnergy(energyAmount, this);
    }

    private void Start()
    {
        //equippedAbilities[0] = new Ability_FrozenGrenade();

        for(int i = 0; i < equippedAbilities.Length; i++)
        {
            if (equippedAbilities[i] != null)
            {
                equippedAbilities[i].OnAbilityEquipped(this);
                HudManager.Instance.SetAbility(i, equippedAbilities[i].gameObject.GetComponent<WeaponInformations>());
            }
        }
    }

    private void Update()
    {
        RemoveEnergy(Time.deltaTime * 3, gameObject);

        if (Input.GetKeyDown(KeyCode.C))
        {
            ObjectPooler.Instance.SpawnObject("MoneyPickup", transform.position, Quaternion.identity);
        }

        //First Ability Inputs
        if (equippedAbilities[0] != null)
        {
            PlayerAbility ability = equippedAbilities[0];

            if (Input.GetButtonDown("Ability1"))
            {
                ability.OnAbilityPressed();
            }
            else if (Input.GetButtonUp("Ability1"))
            {
                ability.OnAbilityReleased();
            }
            else if (Input.GetButton("Ability1"))
            {
                ability.OnAbility();
            }
        }

        //Second Ability Inputs
        if (equippedAbilities[1] != null)
        {
            PlayerAbility ability = equippedAbilities[1];

            if (Input.GetButtonDown("Ability2"))
            {
                ability.OnAbilityPressed();
            }
            else if (Input.GetButtonUp("Ability2"))
            {
                ability.OnAbilityReleased();
            }
            else if (Input.GetButton("Ability2"))
            {
                ability.OnAbility();
            }
        }
    }
}
