using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICanIncreaseSpeed
{
    public void GiveSpeed(float speedAmount, GameObject source);
    public void RemoveSpeed(float speedAmount, GameObject source);
}
