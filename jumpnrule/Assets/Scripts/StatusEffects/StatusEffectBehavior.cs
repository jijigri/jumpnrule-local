using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StatusEffectBehavior
{
    public StatusEffectData data;

    public StatusEffectBehavior(StatusEffectData data)
    {
        this.data = data;
    }

    public virtual void Start(StatusEffectsManager statusEffectManager)
    {

    }

    public virtual void Update(StatusEffectsManager statusEffectManager)
    {

    }

    public virtual void End(StatusEffectsManager statusEffectManager)
    {

    }
}
