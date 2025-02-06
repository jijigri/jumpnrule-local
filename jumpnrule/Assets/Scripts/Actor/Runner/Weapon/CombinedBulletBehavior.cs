using UnityEngine;

public class CombinedBulletBehavior : MonoBehaviour
{
    [SerializeField] Rigidbody2D _rigidBody;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Bullet Collide: collision tag = " + collision.gameObject.tag);

        if (collision.gameObject.tag == "KillableEnemy")
        {
            if(collision.gameObject.TryGetComponent<ICanBeDamaged>(out ICanBeDamaged damageable))
            {
                Debug.Log("CombinedBullet Collide: " + "enemy will be killed");

                damageable.DealDamage(20, this.gameObject, _rigidBody != null ? _rigidBody.velocity : Vector2.zero);

                // TODO: Implement properly - for the fun of it, I will give you gold for now
                LevelManager.Instance.GetLevelState().GetRunner().GetComponent<RunnerStat>().IncreaseGold(1);

                Destroy(this.gameObject);
            }
        }
    }
}
