using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffectsManager : MonoBehaviour
{
    Dictionary<StatusEffectBehavior, float> _statusEffects = new Dictionary<StatusEffectBehavior, float>();

    ICanBeDamaged iDamageable = null;

    private void Awake()
    {
        iDamageable = GetComponent<ICanBeDamaged>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            StatusEffectBehavior statusEffect = StatusEffectsPool.Instance.poisonStatusEffect;
            statusEffect.data.damage = .5f;
            statusEffect.data.tickTime = .25f;

            SetStatusEffect(statusEffect, 2);
        }


        List<StatusEffectBehavior> keys = new List<StatusEffectBehavior>(_statusEffects.Keys);
        foreach (StatusEffectBehavior key in keys)
        {
            OnEffectUpdate(key);
            _statusEffects[key] -= Time.deltaTime;

            if (_statusEffects[key] <= 0)
            {
                RemoveStatusEffect(key);
                continue;
            }
        }
    }

    public void SetStatusEffect(StatusEffectBehavior statusEffect, float duration)
    {
        foreach (KeyValuePair<StatusEffectBehavior, float> effect in _statusEffects)
        {
            if (effect.Key.data.type == statusEffect.data.type)
            {
                Debug.Log("AN EFFECT OF THAT TYPE IS ALREADY APPLIED");
                return;
            }
        }

        OnEffectStart(statusEffect);
        _statusEffects.Add(statusEffect, duration);
    }

    public void RemoveStatusEffect(StatusEffectBehavior statusEffect)
    {
        OnEffectEnd(statusEffect);
        _statusEffects.Remove(statusEffect);
    }

    void OnEffectStart(StatusEffectBehavior statusEffect)
    {
        statusEffect.Start(this);
    }

    void OnEffectUpdate(StatusEffectBehavior statusEffect)
    {
        statusEffect.Update(this);
    }

    void OnEffectEnd(StatusEffectBehavior statusEffect)
    {
        statusEffect.End(this);
    }
}