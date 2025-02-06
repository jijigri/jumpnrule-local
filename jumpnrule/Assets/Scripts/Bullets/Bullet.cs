using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour, IHasPlayerOwner, IHasGunOwner
{
    [SerializeField] private bool isFriendly = true;

    private float _speed;
    private float _damage;
    private float _lifetime = 200f;

    private bool _hasBeenInitialized = false;

    Vector2 _velocity;

    protected GameObject _playerOwner;
    protected Gun _gunOwner;

    private Rigidbody2D _rigidBody;
    private ParticleSystem _particles;
    protected PlayerOwnedObject _playerOwnedObject;

    TrailRenderer _trailRenderer;

    private ObjectPooler _pooler;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _particles = GetComponentInChildren<ParticleSystem>();
        _playerOwnedObject = GetComponent<PlayerOwnedObject>();
        _trailRenderer = GetComponentInChildren<TrailRenderer>();
    }

    private void Start()
    {
        if (!_hasBeenInitialized)
        {
            Debug.Log("Bullet has not been initialized");
            StopAllCoroutines();
            StopCoroutine(CRT_DestroyBulletTimer());
        }

        _pooler = ObjectPooler.Instance;
    }

    public void InitializeBullet(float speed, float damage, float lifetime = 10)
    {
        _speed = speed;
        _damage = damage;
        _lifetime = lifetime;

        _hasBeenInitialized = true;

        StopAllCoroutines();
        StartCoroutine(CRT_DestroyBulletTimer());
    }

    private void Update()
    {
        _velocity = (Vector2)transform.right * _speed;
    }

    private void FixedUpdate()
    {
        _rigidBody.position = _rigidBody.position + (_velocity * Time.fixedDeltaTime);
    }

    public virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (isFriendly)
        {
            //IF ENEMY HIT
            if (!collision.CompareTag("Player"))
            {
                ICanBeDamaged damageable = collision.GetComponent<ICanBeDamaged>();

                if (damageable != null)
                {
                    damageable.DealDamage(_damage, this.gameObject, _velocity);
                    _pooler.SpawnObject("NormalHitEffect", transform.position, Quaternion.Euler(0, 0, Random.Range(0, 360f)));
                }

                _pooler.SpawnObject("BulletHitEffect", transform.position, Quaternion.identity);
                DestroyBullet();
            }
        }
        else
        {
            //IF PLAYER HIT
            if (!collision.CompareTag("KillableEnemy"))
            {
                ICanBeDamaged damageable = collision.GetComponent<ICanBeDamaged>();

                if (damageable != null)
                {
                    damageable.DealDamage(_damage, this.gameObject, _velocity);
                }

                _pooler.SpawnObject("BulletHitEffect", transform.position, Quaternion.identity);
                DestroyBullet();
            }
        }
    }

    IEnumerator CRT_DestroyBulletTimer()
    {
        yield return new WaitForSeconds(_lifetime);

        DestroyBullet();

        yield break;
    }

    public virtual void DestroyBullet()
    {
        if (_particles != null)
        {
            _particles.transform.parent = null;
            _particles.Stop();
            Destroy(_particles.gameObject, 2f);
        }

        if(_trailRenderer != null)
        {
            _trailRenderer.transform.parent = null;
            _trailRenderer.emitting = false;
            Destroy(_trailRenderer.gameObject, 1f);
        }

        _pooler.SpawnObject("BulletDestroyEffect", transform.position, Quaternion.identity);

        Destroy(gameObject);
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
}
