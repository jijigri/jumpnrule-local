using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionAvoidance : MonoBehaviour
{
    [SerializeField] private LayerMask _collisionMask = default;
    [SerializeField] private float _checkRadius = 1;
    [SerializeField] private float _pushForce = 10;

    Rigidbody2D _rigidBody;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, _checkRadius, _collisionMask);
        if (colliders.Length > 0)
        {
            for (int i = 0; i < colliders.Length; i++)
            {
                Collider2D current = colliders[i];
                if (current.gameObject != gameObject)
                {
                    Vector2 direction = transform.position - current.transform.position;
                    direction.Normalize();

                    if (direction.x == 0)
                    {
                        direction.x = Random.Range(-1, 1);
                        if (direction.x < 0)
                        {
                            direction.x = -1;
                        }
                        else
                        {
                            direction.x = 1;
                        }
                    }

                    _rigidBody.AddForce(new Vector2(direction.x, 0) * _pushForce);
                }
            }
        }
    }
}
