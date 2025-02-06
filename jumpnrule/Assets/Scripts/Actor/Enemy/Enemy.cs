using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public enum EnemyType
{
    DEMON = 0,
    ENERGY_CREATURE = 1,
    HEALTH_CREATURE = 2,
    ARMOR_CREATURE = 3,
    SPEED_CREATURE = 4,
    OTHER = 5
}

public class Enemy : MonoBehaviour, ICanBeDamaged
{
    [HideInInspector] [SerializeField] protected float _maxHealth = 10;
    [HideInInspector] [SerializeField] protected float _weight = 10;
    [HideInInspector] [SerializeField] protected float _moneyDrop = 1;
    [HideInInspector] [SerializeField] public bool canRespawn = false;
    [HideInInspector] public EnemyType enemyType = EnemyType.DEMON;
    [HideInInspector] public int tier;
    [HideInInspector] [SerializeField] protected float _falterInvulnerabilityTime = 4f;
    [Range(1, 100)]
    [HideInInspector] [SerializeField] protected float _damageNeededToFalter = 40f;
    [HideInInspector] [SerializeField] protected float _falterLockTime = 1f;
    [HideInInspector] [SerializeField] protected float _hitStopTime = .2f;
    [HideInInspector] [SerializeField] protected float _timeBeforeResettingFalter = .5f;

    [HideInInspector] [SerializeField] protected string _damageSound = "";
    [HideInInspector] [SerializeField] protected string _deathSound = "";

    bool _isActive = true;

    float _currentHealth = 0;

    protected bool _isFaltered = false;
    protected bool _isHitStunned = false;

    protected float _falterValue = 0;
    protected float _timeLeftToFalter = 0;
    protected float _falterInvulnerabilityCooldown = 0;

    private EventInstance _successShotSound;
    private EventInstance _criticalShotSound;
    private EventInstance _generalDeathSound;

    protected Rigidbody2D _rigidBody;
    protected Animator _animator;
    protected SpriteRenderer _spriteRenderer;
    protected DropsManager _dropsManager;

    protected ObjectPooler _pooler;

    protected GameEvents _gameEvents;

    protected GameObject _player;

    protected virtual void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _animator = GetComponentInChildren<Animator>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _dropsManager = GetComponent<DropsManager>();

        _successShotSound = RuntimeManager.CreateInstance("event:/Enemies/SuccessShotSound");
        _criticalShotSound = RuntimeManager.CreateInstance("event:/Enemies/CriticalShotSound");
        _generalDeathSound = RuntimeManager.CreateInstance("event:/Enemies/EnemyDeathSound");

        _spriteRenderer.sharedMaterial = new Material(_spriteRenderer.sharedMaterial);
        _spriteRenderer.sharedMaterial.SetFloat("_StepValue", 0);
    }

    protected virtual void Start()
    {
        _pooler = ObjectPooler.Instance;
        _player = GameObject.FindWithTag("Player");
        _gameEvents = GameEvents.Instance;

        _currentHealth = _maxHealth;
    }

    protected virtual void Update()
    {
        if (_timeLeftToFalter > 0)
        {
            _timeLeftToFalter -= Time.deltaTime;
        }
        else
        {
            //If the time left to falter enemy is 0 (Enemy hasn't been attacked in {timeBeforeResettingFalter}), resets the falter value
            _falterValue = 0;
        }

        if (_falterInvulnerabilityCooldown > 0)
        {
            _falterInvulnerabilityCooldown -= Time.deltaTime;
        }
    }

    protected virtual void LateUpdate()
    {
        if (_isHitStunned)
        {
            _rigidBody.velocity = Vector2.zero;
        }
    }

    public virtual void DealDamage(float damage, GameObject source, Vector2 sourceVelocity, DamageType damageType = DamageType.DIRECT)
    {
        _currentHealth -= damage;

        if (_isHitStunned == false)
        {
            Vector2 impulseVelocity = (sourceVelocity * .25f * _rigidBody.drag) / _weight;
            if (_rigidBody != null)
            {
                _rigidBody.AddForce(impulseVelocity, ForceMode2D.Impulse);
            }
        }

        Gun gunSource = GetGunSource(source);
        CalculateFalter(damage, gunSource);

        if (_animator != null)
        {
            _animator.SetTrigger("hit");
        }

        CreateDamageIndicator(damage, sourceVelocity);
        DamageEffect();

        //If less than 5% max health, kill enemy
        if ((_currentHealth / _maxHealth) * 100 <= 5)
        {
            Die(source);

            if (!_deathSound.Equals(""))
            {
                RuntimeManager.PlayOneShotAttached(_deathSound, gameObject);
            }
        }
        else
        {
            if (!_damageSound.Equals(""))
            {
                RuntimeManager.PlayOneShotAttached(_damageSound, gameObject);
            }
        }

        GameObject killer = GetKiller(source);
        _gameEvents.EnemyDamaged(this, killer);
    }

    protected virtual void CreateDamageIndicator(float damage, Vector2 sourceVelocity)
    {
        if (_pooler != null)
        {
            GameObject floatingText = _pooler.SpawnObject("FloatingText", transform.position, Quaternion.identity);
            floatingText.GetComponent<FloatingText>().InitializeText(TextType.ENEMY_DAMAGE, damage, transform.position, sourceVelocity);

            _pooler.SpawnObject("EnemyBloodParticles", transform.position, Quaternion.identity);
        }
    }

    protected virtual void DamageEffect()
    {
        CameraEffects.Instance.Shake(.1f, 2f, true, 4f);
        PlayDamageSound();
        SetDamageDecay();
    }

    protected virtual void PlayDamageSound()
    {
        if (false)
        {
            RuntimeManager.AttachInstanceToGameObject(_successShotSound, transform);
            _successShotSound.start();
        }
        else
        {
            //REMEMBER TO REPLACE CRIT SOUND WITH AN ACTUAL SOUND
            RuntimeManager.AttachInstanceToGameObject(_criticalShotSound, transform);
            _criticalShotSound.start();
        }
    }

    protected virtual void SetDamageDecay()
    {

        float value = _currentHealth / _maxHealth;
        value = (value - 1) * -1;
        _spriteRenderer.sharedMaterial.SetFloat("_StepValue", value);
    }

    protected virtual Gun GetGunSource(GameObject damageSource)
    {
        if (damageSource.TryGetComponent(out IHasGunOwner iHasGunOwner))
        {
            return iHasGunOwner.GetGunOwner();
        }

        if (damageSource.TryGetComponent(out Gun gun))
        {
            return gun;
        }

        return null;
    }

    protected virtual void CalculateFalter(float damage, Gun gunSource)
    {
        bool isHardFalter = gunSource != null ? gunSource.canHardFalter : false;

        if (_falterInvulnerabilityCooldown <= 0 || isHardFalter)
        {
            _timeLeftToFalter = _timeBeforeResettingFalter;
            if (_timeLeftToFalter > _timeBeforeResettingFalter)
            {
                _timeLeftToFalter = _timeBeforeResettingFalter;
            }

            _falterValue += damage;

            if (_falterValue >= _damageNeededToFalter || isHardFalter)
            {
                Falter(gunSource);
            }
        }
    }

    protected virtual void Falter(Gun gunSource)
    {
        StartCoroutine(FalterTimer());
        StartCoroutine(StunTimer());
        _falterInvulnerabilityCooldown = _falterInvulnerabilityTime;
        _falterValue = 0;
        _timeLeftToFalter = 0;

        GameEvents.Instance.EnemyFalter(gunSource);
    }

    protected virtual IEnumerator FalterTimer()
    {
        _spriteRenderer.color = Color.red;

        OnFalterStart();
        _isFaltered = true;

        yield return new WaitForSeconds(_falterLockTime);

        _spriteRenderer.color = Color.white;

        OnFalterStop();
        _isFaltered = false;

        yield break;
    }

    protected virtual void OnFalterStart()
    {

    }

    protected virtual void OnFalterStop()
    {

    }

    protected virtual IEnumerator StunTimer()
    {
        _isHitStunned = true;

        Vector2 impulseVelocity = _rigidBody.velocity;

        if (_animator != null)
        {
            _animator.speed = 0;
        }

        yield return new WaitForSeconds(_hitStopTime);

        _isHitStunned = false;
        _rigidBody.AddForce(impulseVelocity, ForceMode2D.Impulse);

        if (_animator != null)
        {
            _animator.speed = 1;
        }

        yield break;
    }

    protected virtual void Die(GameObject source)
    {
        if (!_isActive)
        {
            return;
        }

        if (RoundManager.Instance != null)
        {
            RoundManager.Instance.RemoveEnemyFromRound(this);
        }

        OnDeath(source);

        DeathEffect();

        StopAllCoroutines();

        Destroy(gameObject);

        _isActive = false;
    }

    protected virtual void DeathEffect()
    {
        CameraEffects cameraEffects = CameraEffects.Instance;
        cameraEffects.Shockwave(transform.position, 4, .14f, -.2f, 1f, 1.5f);
        cameraEffects.Shake(.275f, 6f, false, 10);

        if (_animator != null)
        {
            _animator.transform.SetParent(null);
            _animator.SetTrigger("death");
            Destroy(_animator.gameObject, 1f);
        }

        _pooler.SpawnObject("EnemyDeathEffect", transform.position, Quaternion.Euler(0, 0, Random.Range(0, 360)));

        RuntimeManager.AttachInstanceToGameObject(_generalDeathSound, transform);
        _generalDeathSound.start();
    }

    protected virtual void OnDeath(GameObject deathSource)
    {
        GameObject killer = GetKiller(deathSource);

        if (killer.CompareTag("Player"))
        {
            if (_dropsManager == null)
            {
                for (int i = 0; i < _moneyDrop; i++)
                {
                    _pooler.SpawnObject("MoneyPickup", transform.position, Quaternion.Euler(0, 0, Random.Range(0, 360)));
                }
            }
            else
            {
                _dropsManager.Drop();
            }
        }

        _gameEvents.EnemyKilled(this, killer);
    }

    GameObject GetKiller(GameObject source)
    {
        GameObject killer = source;

        PlayerOwnedObject playerOwnedObject = source.GetComponent<PlayerOwnedObject>();
        if (playerOwnedObject != null)
        {
            return playerOwnedObject.Owner;
        }
        else
        {
            IHasPlayerOwner playerOwner = source.GetComponent<IHasPlayerOwner>();
            if (playerOwner != null)
            {
                return playerOwner.GetPlayerOwner();
            }
            return killer;
        }
    }

    public void SoftKill()
    {
        if (!_isActive)
        {
            return;
        }

        OnDeath(gameObject);

        DeathEffect();

        StopAllCoroutines();

        Destroy(gameObject);

        _isActive = false;
    }
}
