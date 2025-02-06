using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour, IHasPlayerOwner, IHasGunOwner
{
    public float _hitboxRadius;
    [SerializeField] private LayerMask _hitMask = default;
    [SerializeField] private ParticleSystem _particles = null;
    [SerializeField] private ParticleSystem _flash = null;

    GameObject _playerOwner;
    Gun _gunOwner;

    float _damage = 1;

    public void InitializeExplosion(float damage, float radius, LayerMask hitMask = default)
    {
        var particlesShape = _particles.shape;
        particlesShape.radius = radius;

        _flash.startSize = radius * 2.5f;

        _hitboxRadius = radius;

        if(hitMask != default)
        {
            _hitMask = hitMask;
        }

        _damage = damage;

        Explode();
    }

    public void SetPlayerOwner(GameObject playerOwner)
    {
        _playerOwner = playerOwner;
    }

    public GameObject GetPlayerOwner()
    {
        return _playerOwner;
    }
    public void SetGunOwner(Gun gunOwner)
    {
        _gunOwner = gunOwner;
    }

    public Gun GetGunOwner()
    {
        return _gunOwner;
    }

    private void Explode()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, _hitboxRadius, _hitMask);

        if(hits.Length > 0)
        {
            foreach(Collider2D hit in hits)
            {
                Vector2 hitDirection = hit.transform.position - transform.position;
                if (!hit.gameObject.CompareTag("Player"))
                {
                    ICanBeDamaged damage = hit.gameObject.GetComponent<ICanBeDamaged>();

                    if (damage != null)
                    {
                        damage.DealDamage(_damage, gameObject, hitDirection);
                    }
                }
                else
                {
                    Rigidbody2D rigidBody = hit.gameObject.GetComponent<Rigidbody2D>();

                    if (rigidBody != null)
                    {
                        rigidBody.velocity = (rigidBody.velocity / 3) + (hitDirection.normalized * 25);
                    }
                }
            }
        }

        CameraEffects.Instance.Shake(_hitboxRadius / 18, _hitboxRadius / 2, false, 5);
    }
}
