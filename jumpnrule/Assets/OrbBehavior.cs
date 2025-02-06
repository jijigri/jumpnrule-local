using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbBehavior : MonoBehaviour, ICanBeDamaged
{
    [SerializeField] private float _maxDamage = 60f;
    [SerializeField] private float _damageMultiplierIncreaseOnHit = .1f;
    [SerializeField] private Color _playerCaptureColor = Color.blue;
    [SerializeField] private Color _enemyCaptureColor = Color.red;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private SpriteRenderer _outlineRenderer;
    [SerializeField] private ParticleSystem _particles;
    [SerializeField] private ParticleSystem _deathParticles;

    EventInstance _playerHitEvent;
    EventInstance _playerCaptureEvent;
    EventInstance _enemyHitEvent;
    EventInstance _enemyCaptureEvent;

    private float _currentDamageValue = 0;
    private float _currentDamageMultiplier = 1;

    private Animator _animator;

    private Material _material;
    private Material _outlineMaterial;

    Color _particlesStartColor;

    float _rotationSpeed;

    bool _isActive = true;

    CircleCollider2D _collider;

    void Awake()
    {
        _collider = GetComponent<CircleCollider2D>();
        _animator = GetComponent<Animator>();

        _playerHitEvent = RuntimeManager.CreateInstance("event:/Misc/Orb/OrbPlayerHit");
        _playerCaptureEvent = RuntimeManager.CreateInstance("event:/Misc/Orb/OrbPlayerCapture");

        _enemyHitEvent = RuntimeManager.CreateInstance("event:/Misc/Orb/OrbPlayerHit");
        _enemyCaptureEvent = RuntimeManager.CreateInstance("event:/Misc/Orb/OrbPlayerCapture");

        _material = _spriteRenderer.material;
        _outlineMaterial = _outlineRenderer.material;
        _rotationSpeed = 30;
    }

    void Start()
    {
        _particlesStartColor = _particles.main.startColor.color;
    }

    void Update()
    {
        if (_isActive)
        {
            _spriteRenderer.transform.Rotate(Vector3.forward, _rotationSpeed * Time.deltaTime);
        }
    }

    public virtual void DealDamage(float damageAmount, GameObject source, Vector2 hitVelocity, DamageType damageType = DamageType.DIRECT)
    {
        if(_isActive == false)
        {
            return;
        }

        GameObject attacker = Helper.GetKiller(source);

        if (attacker.CompareTag("Player"))
        {
            _currentDamageValue += damageAmount * _currentDamageMultiplier;
            _playerHitEvent.start();
        }
        else
        {
            _currentDamageValue -= damageAmount * _currentDamageMultiplier;
            _enemyHitEvent.start();
        }

        if(_currentDamageValue >= _maxDamage)
        {
            OnPlayerCapture();
        }
        else if (_currentDamageValue < -_maxDamage)
        {
            OnEnemyCapture();
        }

        if (_currentDamageValue >= 0)
        {
            float t = _currentDamageValue / _maxDamage;
            _material.SetColor("_Color", _playerCaptureColor);
            _material.SetFloat("_t", t);

            _outlineMaterial.SetColor("_Color", _playerCaptureColor);
            _outlineMaterial.SetFloat("_t", t);

            var main = _particles.main;
            main.startColor = new ParticleSystem.MinMaxGradient(Color.Lerp(_particlesStartColor, _playerCaptureColor, t));

            if (t > .5f)
            {
                var emission = _particles.emission;
                emission.rateOverTime = t * 20;

                _particles.startSpeed = t * 2;
            }
            else
            {
                var emission = _particles.emission;
                emission.rateOverTime = 0;
            }

            _playerHitEvent.setParameterByName("OrbPitch", t);
        }
        else
        {
            float t = Mathf.Abs(_currentDamageValue) / _maxDamage;
            _material.SetColor("_Color", _enemyCaptureColor);
            _material.SetFloat("_t", t);

            _outlineMaterial.SetColor("_Color", _enemyCaptureColor);
            _outlineMaterial.SetFloat("_t", t);

            var main = _particles.main;
            main.startColor = new ParticleSystem.MinMaxGradient(Color.Lerp(_particlesStartColor, _enemyCaptureColor, t));

            if (t > .5f)
            {
                var emission = _particles.emission;
                emission.rateOverTime = t * 20;

                _particles.startSpeed = t * 2.5f;
            }
            else
            {
                var emission = _particles.emission;
                emission.rateOverTime = 0;
            }
        }

        _animator.SetTrigger("damage");

        float fillPercent = (Mathf.Abs(_currentDamageValue) / _maxDamage) * 100;
        if(fillPercent < 25)
        {
            _animator.SetInteger("level", 0);
            _rotationSpeed = 30;
        }
        else if(fillPercent >= 25 && fillPercent < 50)
        {
            _animator.SetInteger("level", 1);
            _rotationSpeed = 60;
        }
        else if(fillPercent >= 50 && fillPercent < 75)
        {
            _animator.SetInteger("level", 2);
            _rotationSpeed = 90;
        }
        else if(fillPercent >= 75)
        {
            _animator.SetInteger("level", 3);
            _rotationSpeed = 120;
        }

        _currentDamageMultiplier += _damageMultiplierIncreaseOnHit;
    }

    protected virtual void OnPlayerCapture()
    {
        _deathParticles.startColor = _playerCaptureColor;

        _playerCaptureEvent.start();

        DestroyOrb();
    }

    protected virtual void OnEnemyCapture()
    {
        _deathParticles.startColor = _enemyCaptureColor;

        _enemyCaptureEvent.start();

        DestroyOrb();
    }

    void DestroyOrb()
    {
        _animator.enabled = true;
        _animator.SetTrigger("Explode");

        _outlineRenderer.enabled = false;

        _particles.Stop();
        _deathParticles.Play();

        _collider.enabled = false;

        CameraEffects cameraEffects = CameraEffects.Instance;
        cameraEffects.Shake(.15f, 5);
        cameraEffects.ZoomIn(.4f, .1f);
        cameraEffects.Shockwave(transform.position, 3, 0.03f, -0.16f, 4f, 1);

        _isActive = false;

        Destroy(gameObject, 1f);
    }
}
