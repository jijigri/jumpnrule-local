using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PoisonEffectBehavior : StatusEffectBehavior
{
    float _timeBeforeNextTick = 0;

    GameObject _effect = null;

    public PoisonEffectBehavior(StatusEffectData data) : base(data)
    {

    }

    public override void Start(StatusEffectsManager statusEffectManager)
    {
        base.Start(statusEffectManager);

        statusEffectManager.gameObject.GetComponentInChildren<SpriteRenderer>().color = Color.green;
        _effect = GameObject.Instantiate(data.effect, statusEffectManager.transform);
    }

    public override void Update(StatusEffectsManager statusEffectManager)
    {
        base.Update(statusEffectManager);

        _timeBeforeNextTick -= Time.deltaTime;

        if (_timeBeforeNextTick <= 0)
        {
            if (statusEffectManager.transform.TryGetComponent(out ICanBeDamaged iDamageable))
            {
                iDamageable.DealDamage(data.damage, statusEffectManager.gameObject, Vector2.zero);
            }

            _timeBeforeNextTick = data.tickTime;
        }
    }

    public override void End(StatusEffectsManager statusEffectManager)
    {
        base.End(statusEffectManager);

        statusEffectManager.gameObject.GetComponentInChildren<SpriteRenderer>().color = Color.white;
        GameObject.Destroy(_effect.gameObject);
    }
}
