using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketBullet : Bullet
{
    public float explosionRadius = 2f;
    [SerializeField] private string _explosionTag;
    [SerializeField] private float _explosionDamage = 4f;

    public override void DestroyBullet()
    {
        GameObject explosionObject = ObjectPooler.Instance.SpawnObject(_explosionTag, transform.position, Quaternion.identity);

        if (explosionObject.TryGetComponent(out PlayerOwnedObject playerOwnedObject))
        {
            playerOwnedObject.SetOwner(_playerOwnedObject.Owner);
        }

        if (explosionObject.TryGetComponent(out IHasGunOwner gunOwner))
        {
            gunOwner.SetGunOwner(_gunOwner);
        }

        explosionObject.GetComponent<Explosion>().InitializeExplosion(_explosionDamage, explosionRadius);

        base.DestroyBullet();
    }
}
