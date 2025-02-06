using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability_Grenade : PlayerAbility
{
    [SerializeField] private GameObject _grenadePrefab = null;
    [SerializeField] private float _damage = 10;
    [SerializeField] private float _radius = 10;
    [SerializeField] private float _throwForce = 10;
    [SerializeField] private float _minThrowForce = 5;
    [SerializeField] private float _maxThrowForce = 30;

    private Camera _camera;

    public override void OnAbilityEquipped(PlayerAbilityManager abilityManager)
    {
        base.OnAbilityEquipped(abilityManager);

        _camera = Camera.main;
    }

    protected override void UseAbility()
    {
        base.UseAbility();
        

        Debug.Log("ABILITY BEING USED");

        GameObject grenade = Instantiate(_grenadePrefab, _player.transform.position, Quaternion.identity);
        Vector2 throwDirection = _camera.ScreenToWorldPoint(Input.mousePosition) - _player.transform.position;

        //if (throwDirection.y < 5f) throwDirection.y = 5f;

        float clampMagnitude = Mathf.Clamp(throwDirection.magnitude * _throwForce, _minThrowForce, _maxThrowForce);
        Vector2 throwVelocity = throwDirection.normalized * clampMagnitude;

        GrenadeScript grenadeScript = grenade.GetComponent<GrenadeScript>();
        grenadeScript.Initialize(throwVelocity, _damage, _radius, _playerAbilityManager);
        grenadeScript.SetPlayerOwner(_player);

        if (grenade.TryGetComponent(out PlayerOwnedObject playerOwnedObject))
        {
            playerOwnedObject.SetOwner(gameObject);
        }
    }
}
