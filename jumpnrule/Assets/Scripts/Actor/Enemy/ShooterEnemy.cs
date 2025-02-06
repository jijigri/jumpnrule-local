using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterEnemy : Enemy
{
    public GameObject bulletToShoot = null;
    public Transform shootPoint = null;

    public bool _startShootingOnStart = true;

    [HideInInspector] public int numberOfBulletsPerShot = 1;
    [HideInInspector] public int numberOfBulletsBeforeReloading = 10;
    [HideInInspector] public float timeBetweenShots = .4f;
    [HideInInspector] public bool stopsAfterEmptyingMagazine = false;
    [HideInInspector] public float timeToReload = 2f;

    [HideInInspector] public bool aimsAtPlayers = true;

    [HideInInspector] public float shotsInaccuracy = 0;

    [HideInInspector] public float shootDirection = 0;
    [HideInInspector] public float bulletSpreadInDegrees = 45;

    [HideInInspector] public float _bulletSpeed = 5f;
    [HideInInspector] public float _bulletDamage = 5f;
    [HideInInspector] public float _bulletLifetime = 3f;

    private bool _isShooting = false;

    protected override void Start()
    {
        base.Start();

        if (_startShootingOnStart)
        {
            StartShooting(.5f);
        }
    }

    public virtual void StartShooting(float delay)
    {
       StartCoroutine(Shoot(delay));
    }

    public virtual IEnumerator Shoot(float delay)
    {
        if (_isShooting)
        {
            yield break;
        }

        if (_isFaltered)
        {
            CancelShooting();
            yield break;
        }

        _isShooting = true;

        yield return new WaitForSeconds(delay);

        do
        {
            for (int shot = 0; shot < numberOfBulletsBeforeReloading; shot++)
            {
                if (_isFaltered)
                {
                    CancelShooting();
                    yield break;
                }

                if (numberOfBulletsPerShot <= 1)
                {
                    SpawnBullet(0);
                }
                else
                {
                    float angleOffset = 0;
                    float spreadBetweenBullets = bulletSpreadInDegrees / numberOfBulletsPerShot;
                    angleOffset -= (spreadBetweenBullets * (numberOfBulletsPerShot - 1)) / 2;

                    for (int i = 0; i < numberOfBulletsPerShot; i++)
                    {
                        SpawnBullet(angleOffset);
                        angleOffset += spreadBetweenBullets;
                    }
                }

                OnShoot();
                yield return new WaitForSeconds(timeBetweenShots);
            }

            OnMagazineEmpty();
            yield return new WaitForSeconds(timeToReload);
        }
        while (stopsAfterEmptyingMagazine == false);

        yield break;
    }

    void SpawnBullet(float spreadOffset)
    {
        float accuracy = Random.Range(-shotsInaccuracy / 2, shotsInaccuracy / 2);

        float shootAngle = 0;

        if (aimsAtPlayers)
        {
            shootAngle = GetDirectionToPlayer();
        }
        else
        {
            shootAngle = shootDirection;
        }

        GameObject bulletObj = Instantiate(bulletToShoot, shootPoint.transform.position, Quaternion.Euler(0, 0, shootAngle + accuracy + spreadOffset));

        if (bulletObj.TryGetComponent(out Bullet bullet))
        {
            bullet.InitializeBullet(_bulletSpeed, _bulletDamage, _bulletLifetime);
            bullet.SetPlayerOwner(gameObject);
        }
    }

    protected virtual void OnShoot()
    {

    }

    protected virtual void OnMagazineEmpty()
    {
        if (stopsAfterEmptyingMagazine)
        {
            _isShooting = false;
        }
    }

    protected virtual void CancelShooting()
    {
        _isShooting = false;
    }

    float GetDirectionToPlayer()
    {
        //TODO: GET A RANDOM PLAYER
        Vector2 playerPos = _player.transform.position;
        return Helper.AngleBetweenTwoPoints(playerPos, transform.position);
    }
}
