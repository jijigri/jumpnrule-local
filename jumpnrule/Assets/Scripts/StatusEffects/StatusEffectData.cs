using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StatusEffectData
{
    public DamageType type;

    public string name;
    public string description;

    public float damage;
    public float tickTime;

    public GameObject effect;
}
