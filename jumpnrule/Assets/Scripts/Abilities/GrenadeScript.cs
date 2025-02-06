using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeScript : MonoBehaviour, IHasPlayerOwner
{
    [SerializeField] private string _explosionPoolerId = "Explosion";
    [SerializeField] private bool _destroyOnExplode = false;

    Rigidbody2D _rigidBody;
    float _damage;
    float _radius;

    GameObject _playerOwner;

    bool _active = true;

    private void OnEnable()
    {
        _active = true;
    }

    public GameObject GetPlayerOwner()
    {
        throw new System.NotImplementedException();
    }

    public void Initialize(Vector2 throwVelocity, float damage, float radius, PlayerAbilityManager playerAbilityManager = null)
    {
        if(_rigidBody == null)
        {
            _rigidBody = GetComponent<Rigidbody2D>();
        }

        _rigidBody.AddForce(throwVelocity, ForceMode2D.Impulse);
        _damage = damage;
        _radius = radius;

        Invoke("Explode", 30);
    }

    public void SetPlayerOwner(GameObject playerOwner)
    {
        _playerOwner = playerOwner;
    }

    void Explode()
    {
        CancelInvoke();
        _active = false;

        GameObject explosionObject = ObjectPooler.Instance.SpawnObject("Explosion", transform.position, Quaternion.identity);

        if (explosionObject.TryGetComponent(out PlayerOwnedObject playerOwnedObject))
        {
            playerOwnedObject.SetOwner(_playerOwner);
        }

        explosionObject.GetComponent<Explosion>().InitializeExplosion(_damage, _radius);

        if (_destroyOnExplode)
        {
            Destroy(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (_active)
        {
            Explode();
        }
    }

    private void Update()
    {
        transform.Rotate(Vector3.forward, Time.deltaTime * 300);
    }
}
