using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingBomb : MonoBehaviour, IHasPlayerOwner, IHasGunOwner, IHasTriggerChild
{
    [SerializeField] private LayerMask _mineLayer;
    private GameObject _playerOwner;
    private Gun _gunOwner;
    private Railgun _railgun;

    float _explosionDamage;
    float _explosionRadius;

    Vector2 _initialPosition;
    Vector2 _targetPosition;
    float _moveSpeed;

    private Animator _animator;

    public bool IsActive { get; set; } = false;

    void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
    }

    public void InitializeFlyingBomb(float explosionDamage, float explosionRadius, Vector2 targetPosition, float moveSpeed, Railgun railgun)
    {
        _explosionDamage = explosionDamage;
        _explosionRadius = explosionRadius;

        _initialPosition = transform.position;
        _targetPosition = targetPosition;
        _moveSpeed = moveSpeed;

        _railgun = railgun;

        IsActive = false;
        StartCoroutine(ReachPosition());

        Collider2D otherMine = Physics2D.OverlapCircle(targetPosition, .4f, _mineLayer);
        if (otherMine)
        {
            if (otherMine.transform != transform)
            {
                if (otherMine.gameObject.TryGetComponent(out FlyingBomb flyingBomb))
                {
                    flyingBomb.Explode();
                }
            }
        }

        Invoke("Explode", 14f);
    }

    IEnumerator ReachPosition()
    {
        float t = 0;
        while(t < 1)
        {
            t += Time.deltaTime * _moveSpeed;

            transform.position = Vector2.Lerp(_initialPosition, _targetPosition, t);

            yield return null;
        }

        if (_animator != null)
        {
            _animator.SetTrigger("activate");
        }

        transform.position = _targetPosition;
        IsActive = true;

        yield break;
    }

    public void Explode()
    {
        CancelInvoke();

        GameObject explosionObject = ObjectPooler.Instance.SpawnObject("Explosion", transform.position, Quaternion.identity);

        if (explosionObject.TryGetComponent(out PlayerOwnedObject playerOwnedObject))
        {
            playerOwnedObject.SetOwner(_playerOwner);
        }

        if (explosionObject.TryGetComponent(out IHasGunOwner gunOwner))
        {
            gunOwner.SetGunOwner(_gunOwner);
        }

        explosionObject.GetComponent<Explosion>().InitializeExplosion(_explosionDamage, _explosionRadius);

        _railgun.RemoveFlyingBomb(this);

        StopAllCoroutines();

        gameObject.SetActive(false);
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

    public void OnChildTriggerEnter(Collider2D collision)
    {
        if (!IsActive)
            return;

        if(collision.gameObject.TryGetComponent(out PlatformController platformController))
        {
            if (platformController.GetVelocity().y <= 0)
            {
                Vector2 oldPlayerVelocity = platformController.GetVelocity();
                if(oldPlayerVelocity.y < 0)
                {
                    oldPlayerVelocity.y = 0;
                }
                Vector2 overrideVelocity = oldPlayerVelocity / 4 + (Vector2.up);
                platformController.OverrideVelocity(overrideVelocity);
                Explode();
            }
        }
    }
}
