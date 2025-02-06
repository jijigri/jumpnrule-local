using UnityEngine;

public class BulletBehavior : MonoBehaviour
{
    public enum BulletType
    {
        eBlue = 0,
        eRed,
    }
    public static BulletType GetOtherBulletType(BulletType bulletType)
    {
        switch(bulletType)
        {
            case BulletType.eBlue:
                return BulletType.eRed;
                //break;
            case BulletType.eRed:
                return BulletType.eBlue;
                //break;
            default:
                Debug.LogWarning("Unknown bullet type");
                return BulletType.eBlue;
                //break;
        }
    }

    [SerializeField] Rigidbody2D _rigidBody;
    public BulletType bulletType;
    private bool bulletTypeSet = false;
    [SerializeField] GameObject _combinedBulletPrefab;

    private void Start()
    {
        _rigidBody.velocity = transform.right * 20.0f;
    }

    private void Update()
    {
        if (!bulletTypeSet)
        {
            Color color;

            switch (bulletType)
            {
                case BulletType.eBlue:
                    color = Color.blue;
                    break;
                case BulletType.eRed:
                    color = Color.red;
                    break;
                default:
                    Debug.LogWarning("Unknown bullet color");
                    color = Color.black;
                    break;
            }

            var renderer = GetComponentInChildren<Renderer>();
            renderer.material.SetColor("_Color", color);

            bulletTypeSet = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Bullet Collide: collision tag = " + collision.gameObject.tag);

        if (this.bulletType == BulletType.eBlue)
        {
            if (collision.gameObject.TryGetComponent<BulletBehavior>(out var otherBulletBehavior) && otherBulletBehavior.bulletType == BulletType.eRed)
            {
                Debug.Log("Bullet Collide: " + "they combine into one bullet");

                GameObject combinedBullet = Instantiate(_combinedBulletPrefab, this.gameObject.transform.position, Quaternion.identity); // TODO: Create that with BulletSpawner
                combinedBullet.GetComponent<Rigidbody2D>().velocity = this._rigidBody.velocity;

                Destroy(collision.gameObject);
                Destroy(this.gameObject);
            }
        }
    }
    public void DestroyBullet()
    {
        Destroy(gameObject);
    }
}
