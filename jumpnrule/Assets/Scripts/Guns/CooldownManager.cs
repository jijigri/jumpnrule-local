using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CooldownManager : MonoBehaviour
{
    protected enum UseType
    {
        MANUAL,
        AUTOMATIC
    }

    [SerializeField] protected UseType _useType = UseType.MANUAL;
    [SerializeField] protected float _generalUseRate = 60f;

    protected float _timeSinceLastUse = 0;

    protected bool hasKeyBeenReleasedSinceLastShot = true;

    protected virtual void Start()
    {
        _timeSinceLastUse = _generalUseRate;
    }

    protected virtual void Update()
    {

        if (_timeSinceLastUse < 60.0f)
        {
            _timeSinceLastUse += Time.deltaTime * _generalUseRate;
        }
        else
        {
            _timeSinceLastUse = 60.0f;
        }
    }

    public void OnKeyReleased()
    {
        hasKeyBeenReleasedSinceLastShot = true;
    }

    public virtual bool CheckCooldown()
    {
        if(_useType == UseType.MANUAL && !hasKeyBeenReleasedSinceLastShot)
        {
            return false;
        }

        if (_timeSinceLastUse >= 60.0f)
        {
            _timeSinceLastUse = 0;
            hasKeyBeenReleasedSinceLastShot = false;

            return true;
        }

        return false;
    }

    public float GetCastRate()
    {
        return _generalUseRate;
    }

    public float GetTimeSinceLastCast()
    {
        return _timeSinceLastUse;
    }
}
