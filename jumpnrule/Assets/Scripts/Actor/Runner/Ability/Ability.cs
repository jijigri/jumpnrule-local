using System.Collections;
using UnityEngine;

public abstract class Ability : MonoBehaviour
{
    protected bool _ready;
    protected float _cooldown;

    protected abstract float GetMaxCooldown();

    protected abstract void Cast();

    private void Awake()
    {
        _ready = true;
        _cooldown = 0f;
    }

    public bool IsReady()
    {
        return _ready;
    }

    public float GetCooldown()
    {
        return _cooldown;
    }

    public bool TryCast()
    {
        if (!_ready)
        {
            return false;
        }

        Cast();

        // start the cooldown timer
        StartCoroutine(RunCooldown());

        return true;
    }

    private IEnumerator RunCooldown()
    {
        _ready = false;
        _cooldown = GetMaxCooldown();

        while (_cooldown > 0f)
        {
            _cooldown -= Time.deltaTime; // Reduce the cooldown with time between previous and actual frame
            yield return null; // Wait until next frame
        }

        _ready = true;
        _cooldown = 0f;
    }
}
