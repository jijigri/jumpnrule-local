using UnityEngine;

public class ChaserBehavior : Enemy
{
    public double speed;
    public double damage;

    Vector2 _velocity;

    private void FixedUpdate()
    {
        GameObject targetRunner = LevelManager.Instance.GetLevelState().GetRunner();
        Vector2 targetPosition = targetRunner.transform.position;
        Vector2 myPosition = transform.position;
        Vector2 targetVelocity = targetPosition - myPosition;
        targetVelocity.Normalize();
        targetVelocity *= (float) speed;

        //_rigidBody.velocity = targetVelocity; // TODO implement properly
        _rigidBody.AddForce(targetVelocity);

        _velocity = targetVelocity;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!_isFaltered)
        {
            GameObject collider = collision.collider.gameObject;

            Debug.Log("Chaser Collide: collision tag = " + collider.gameObject.tag);

            if (collider.CompareTag("Player"))
            {
                if (collider.gameObject.TryGetComponent(out ICanBeDamaged damageable))
                {
                    Debug.Log("Chaser Collide: " + damage + " damage will be dealt to the Runner");
                    damageable.DealDamage((float)damage, gameObject, _velocity);
                    Die(gameObject);
                }
            }
        }
    }
}
