using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;
using FMODUnity;

public class Railgun : Gun
{
    [Header("Railgun Fields")]
    [SerializeField] private float _damageIncreasePerBomb = 5;
    [SerializeField] private float _hitRange = 12;
    [SerializeField] private LayerMask _hitMask = default;
    [SerializeField] private float _selfKnockbackStrength = 5;
    [SerializeField] private float _hitKnockbackStrength = 5;
    [SerializeField] private int _initialPiercing = 1;
    [SerializeField] private SpriteRenderer _hitSprite = null;
    [SerializeField] private Animator _hitAnimator = null;
    [SerializeField] private ParticleSystem _greenParticles = null;
    [SerializeField] private ParticleSystem _yellowParticles = null;
    [SerializeField] private ParticleSystem _orangeParticles = null;
    [SerializeField] private ParticleSystem _redParticles = null;
    [SerializeField] private ParticleSystem _dustParticles = null;
    [SerializeField] private float _particlesPerUnit = 1.5f;
    [Header("Flying Bombs Fields")]
    [SerializeField] private string _flyingBombTag = "";
    [SerializeField] private int _maxNumberOfBombs = 3;
    [SerializeField] private float _bombExplosionDamage = 10;
    [SerializeField] private float _bombExplosionRadius = 5;
    [SerializeField] private float _bombSpeed = 2;
    [SerializeField] private LayerMask _bombBlockMask = default;
    [Header("Cooldown Fields")]
    [SerializeField] private AmmoCooldown _cooldown;

    float _currentDamage = 0;
    int _currentPiercing = 1;

    List<FlyingBomb> _currentBombs = new List<FlyingBomb>();

    Material _material = null;
    PlatformController _playerController = null;
    EventInstance _upgradeEvent;

    public override void Awake()
    {
        base.Awake();
        _upgradeEvent = RuntimeManager.CreateInstance("event:/Weapons/Guns/Railgun/RailgunUpgrade");
        _material = _hitSprite.material;
    }

    public override void Start()
    {
        base.Start();

        _playerController = _player.GetComponent<PlatformController>();

        _currentDamage = _gunDamage;
    }

    public override void Shoot()
    {
        base.Shoot();

        _currentDamage = _gunDamage;

        Vector2 direction = _camera.ScreenToWorldPoint(Input.mousePosition) - _shootPoint.position;
        direction.Normalize();


        SetParticles(_hitRange);

        _greenParticles.Play();
        _dustParticles.Play();


        _hitSprite.transform.SetParent(transform);

        _hitSprite.transform.rotation = transform.rotation * Quaternion.Euler(0, 0, 90);
        _hitSprite.transform.position = (Vector2)_shootPoint.transform.position;
        _hitSprite.transform.localScale = new Vector3(1, transform.lossyScale.y * -1, 1);
        _hitSprite.size = new Vector2(1, _hitRange);


        _material.SetFloat("_FirstPoint", _hitRange);

        _material.SetFloat("_SecondPoint", _hitRange);

        _material.SetFloat("_ThirdPoint", _hitRange);


        _material.SetFloat("_TextureHeight", _hitRange);

        _hitAnimator.SetTrigger("shoot");
        _hitSprite.transform.SetParent(null);

        _currentPiercing = _initialPiercing;

        RaycastHit2D[] hits = Physics2D.RaycastAll(_shootPoint.position, direction, _hitRange, _hitMask);

        if(hits.Length > 0)
        {
            OnHit(hits, direction);
        }

        if(_playerController != null)
        {
            _playerController.OverrideVelocity((_playerController.GetVelocity() / 3) + (direction * -1 * _selfKnockbackStrength));
        }
    }

    void OnHit(RaycastHit2D[] hits, Vector2 direction)
    {
        for(int i = 0; i < hits.Length; i++)
        {
            RaycastHit2D hit = hits[i];

            if (!hit.collider.CompareTag("Player"))
            {
                if (hit.collider.TryGetComponent(out ICanBeDamaged iDamageable))
                {
                    iDamageable.DealDamage(_currentDamage, gameObject, direction * _hitKnockbackStrength);
                    _currentPiercing--;
                }
                else if (hit.collider.gameObject.layer.Equals(LayerMask.NameToLayer("Solid")) || hit.collider.gameObject.layer.Equals(LayerMask.NameToLayer("Level Collidable")))
                {
                    _currentPiercing = 0;
                }
                else if (hit.collider.TryGetComponent(out FlyingBomb flyingBomb))
                {
                    if (flyingBomb.IsActive == false)
                        continue;

                    flyingBomb.Explode();
                    _currentPiercing++;

                    switch (_currentPiercing)
                    {
                        case 2:
                            SetParticles(_yellowParticles, _hitRange);
                            _yellowParticles.Play();
                            _material.SetFloat("_FirstPoint", hit.distance);

                            _upgradeEvent.setParameterByName("Pitch", 1);
                            _upgradeEvent.start();

                            _currentDamage = _gunDamage + _damageIncreasePerBomb;
                            break;
                        case 3:
                            SetParticles(_orangeParticles, _hitRange);
                            _orangeParticles.Play();
                            _material.SetFloat("_SecondPoint", hit.distance);

                            _upgradeEvent.setParameterByName("Pitch", 2);
                            _upgradeEvent.start();

                            _currentDamage = _gunDamage + (_damageIncreasePerBomb * 2);
                            break;

                        case 4:
                            SetParticles(_redParticles, _hitRange);
                            _redParticles.Play();
                            _material.SetFloat("_ThirdPoint", hit.distance);

                            _upgradeEvent.setParameterByName("Pitch", 3);
                            _upgradeEvent.start();

                            _currentDamage = _gunDamage + (_damageIncreasePerBomb * 3);
                            break;
                    }
                }
            }

            if (_currentPiercing <= 0)
            {
                _hitSprite.size = new Vector2(1, hit.distance);

                SetParticles(_greenParticles, hit.distance);
                SetParticles(_yellowParticles, hit.distance);
                SetParticles(_orangeParticles, hit.distance);
                SetParticles(_redParticles, hit.distance);
                SetParticles(hit.distance);

                break;
            }
        }
    }

    void SetParticles(ParticleSystem particles, float distance)
    {
        var shape = particles.shape;
        shape.scale = new Vector3(distance / 2, 1, 1);
        shape.position = new Vector3(distance / 2, 0, -1);

        ParticleSystem.EmissionModule burst = particles.emission;
        burst.SetBurst(0, new ParticleSystem.Burst(0, distance * _particlesPerUnit));
    }

    void SetParticles(float distance)
    {
        var dustShape = _dustParticles.shape;
        dustShape.scale = new Vector3(distance / 2, 1, 1);
        dustShape.position = new Vector3(distance / 2, 0, -1);

        ParticleSystem.EmissionModule dustBurst = _dustParticles.emission;
        dustBurst.SetBurst(0, new ParticleSystem.Burst(0, distance * _particlesPerUnit));
    }

    public override void ShootSecondary()
    {
        base.ShootSecondary();

        if (_currentBombs.Count >= _maxNumberOfBombs)
        {
            _currentBombs[0].Explode();
        }


        Vector2 aimVector = _camera.ScreenToWorldPoint(Input.mousePosition) - _player.transform.position;
        aimVector = Helper.ClampMagnitudeMinMax(aimVector, 2, 15);

        Vector2 spawnPosition = (Vector2)_player.transform.position + aimVector;

        RaycastHit2D blockTest = Physics2D.Raycast(_shootPoint.position, aimVector.normalized, aimVector.magnitude, _bombBlockMask);
        if (blockTest)
        {
            spawnPosition = blockTest.point + (blockTest.normal.normalized * .5f);
        }

        FlyingBomb flyingBomb = _pooler.SpawnObject(_flyingBombTag, _shootPoint.position, Quaternion.identity).GetComponent<FlyingBomb>();
        flyingBomb.InitializeFlyingBomb(_bombExplosionDamage, _bombExplosionRadius, spawnPosition, _bombSpeed, this);
        flyingBomb.SetPlayerOwner(_player);
        flyingBomb.SetGunOwner(this);

        _currentBombs.Add(flyingBomb);
    }

    public void RemoveFlyingBomb(FlyingBomb bomb)
    {
        if (_currentBombs.Contains(bomb))
        {
            _currentBombs.Remove(bomb);
        }
    }
}
