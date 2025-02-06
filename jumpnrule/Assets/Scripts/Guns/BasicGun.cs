using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicGun : Gun
{
    [Space(4)]
    [Header("Gun-specific options")]
    [SerializeField] private GameObject _bullet = null;
    [SerializeField] private float _initialBulletSpeed = 10;
    [SerializeField] private float _initialBulletDamage = 10;
    [SerializeField] private float _initialInaccuracy = 10;
    [Tooltip("Number of shots per minute")] [SerializeField] private float _initialFirerate = 60f;

    [Header("Multishots")]
    [SerializeField] private int _initialNumberOfBullets = 1;
    [SerializeField] private int _initialSpreadInDegrees = 45;

    public override void Shoot()
    {
        base.Shoot();

        if (_initialNumberOfBullets <= 1)
        {
            SpawnBullet(0);
        }
        else
        {
            float angleOffset = 0;
            float spreadBetweenBullets = _initialSpreadInDegrees / _initialNumberOfBullets;
            angleOffset -= (spreadBetweenBullets * (_initialNumberOfBullets - 1)) / 2;

            for (int i = 0; i < _initialNumberOfBullets; i++)
            {
                SpawnBullet(angleOffset);
                angleOffset += spreadBetweenBullets;
            }
        }
    }

    void SpawnBullet(float spreadOffset)
    {
        Debug.Log("Pew pew!");

        float accuracy = Random.Range(-_initialInaccuracy / 2, _initialInaccuracy / 2);

        GameObject bulletObj = Instantiate(_bullet, _shootPoint.transform.position, Quaternion.Euler(0, 0, (AimAngle + _AngleOffset) + accuracy + spreadOffset));

        if(bulletObj.TryGetComponent(out Bullet bullet))
        {
            bullet.InitializeBullet(_initialBulletSpeed, _gunDamage, _initialBulletLifetime);
            bullet.SetPlayerOwner(_player);
            bullet.SetGunOwner(this);
        }

        if(bulletObj.TryGetComponent(out PlayerOwnedObject playerOwnedObject))
        {
            playerOwnedObject.SetOwner(_player);
        }
    }
}
