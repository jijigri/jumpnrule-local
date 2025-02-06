using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHasEnergy
{
    public void GiveEnergy(float energyAmount, GameObject source);
    public void RemoveEnergy(float energyAmount, GameObject source);
}
