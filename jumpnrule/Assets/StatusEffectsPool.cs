using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffectsPool : MonoBehaviour
{
    public static StatusEffectsPool Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public PoisonEffectBehavior poisonStatusEffect;
}
