using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldsCollide : Gun
{
    [Space(4)]
    [Header("Gun-specific options")]
    [SerializeField] private GameObject _bulletPrefab = null;
    [Tooltip("Number of shots per minute")] [SerializeField] private float _initialFirerate = 60f;
    [SerializeField] private int _maxNumberOfBulletsOut = 3;

    //protected float _timeBeforeNextShot = 60;

    Queue<BulletBehavior> _bulletsAlive = new Queue<BulletBehavior>();

    public override void Update()
    {
        base.Update();
    }

    public override void Shoot()
    {
        base.Shoot();

        SpawnBullet(BulletBehavior.BulletType.eBlue);
    }

    public override void ShootSecondary()
    {
        base.ShootSecondary();

        SpawnBullet(BulletBehavior.BulletType.eRed);
    }

    private void SpawnBullet(BulletBehavior.BulletType bulletType)
    {
        if(_bulletsAlive.Count >= _maxNumberOfBulletsOut)
        {
            BulletBehavior bulletToDestroy = _bulletsAlive.Dequeue();

            if (bulletToDestroy != null)
            {
                bulletToDestroy.DestroyBullet();
            }
        }

        GameObject bullet = Instantiate(_bulletPrefab, _shootPoint.position, Quaternion.Euler(0, 0, AimAngle + _AngleOffset));
        BulletBehavior bulletBehavior = bullet.GetComponent<BulletBehavior>();
        bulletBehavior.bulletType = bulletType;

        _bulletsAlive.Enqueue(bulletBehavior);
    }
}
