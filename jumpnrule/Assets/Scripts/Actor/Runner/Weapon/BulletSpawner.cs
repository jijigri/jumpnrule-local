using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    [SerializeField] private GameObject bulletHolder;
    [SerializeField] private GameObject bulletPrefab;

    private void Awake()
    {
    }

    public void Spawn(Vector2 velocity, BulletBehavior.BulletType bulletType)
    {
        GameObject bullet = Instantiate(bulletPrefab, this.gameObject.transform.position, Quaternion.identity, bulletHolder.transform);
        bullet.GetComponent<Rigidbody2D>().velocity = velocity;
        bullet.GetComponent<BulletBehavior>().bulletType = bulletType;
    }
}
