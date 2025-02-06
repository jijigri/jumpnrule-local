using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthManager : MonoBehaviour, ICanBeDamaged, ICanHeal, ICanGetArmor
{
    [SerializeField] private float _maxHealth = 120f;
    [SerializeField] private float _maxOverflowHealth = 120f;
    [SerializeField] private float _maxArmor = 250f;
    [SerializeField] private float _armorLossOverTimeRate = 8f;
    [SerializeField] private float _hitInvulnerabilityTime = .25f;
    [SerializeField] private bool _invincibleMode = false;

    [SerializeField] private float _currentHealth;
    private float _currentArmor;

    private bool _isCritical = false;

    PlayerAudioManager _audioManager;

    private void Awake()
    {
        _audioManager = GetComponent<PlayerAudioManager>();
    }

    void Start()
    {
        GameEvents.Instance.onRoundCleared += OnRoundCleared;

        _currentHealth = _maxHealth;
        _currentArmor = 0;

        HudManager.Instance.SetPlayerHealthManager(this);
    }

    void OnDestroy()
    {
        GameEvents.Instance.onRoundCleared -= OnRoundCleared;
    }

    void OnRoundCleared(int roundIndex)
    {
        Heal(_maxHealth * 100, LevelManager.Instance.gameObject);
        _currentArmor = 0;
    }

    public void DealDamage(float damageAmount, GameObject source, Vector2 hitVelocity, DamageType damageType = DamageType.DIRECT)
    {
        float excessDamage = 0;
        if(_currentArmor > 0)
        {
            _currentArmor -= damageAmount;

            if(_currentArmor < 0)
            {
                excessDamage = -_currentArmor;
                _currentArmor = 0;
            }
        }
        else
        {
            excessDamage = damageAmount;
        }

        _currentHealth -= excessDamage;

        if(_currentHealth <= 0 && _currentHealth > -3)
        {
            _currentHealth = 1f;
        }

        if (_currentHealth <= 0)
        {
            if (_invincibleMode == false)
            {
                Die();
                return;
            }
        }

        if (_audioManager != null)
        {
            _audioManager.PlayDamage();
        }

        float effectStrength = .25f;
        if (damageAmount < _maxHealth / 10) effectStrength = .1f;
        if (damageAmount > _maxHealth / 2) effectStrength = 1f;
        BackgroundEffectsManager.Instance.DamageEffect(.05f, .1f, effectStrength);

        if((_currentHealth / _maxHealth) * 100 <= 20)
        {
            BackgroundEffectsManager.Instance.SetCriticalEffect();

            if (!_isCritical)
            {
                if (_audioManager != null)
                {
                    _audioManager.StartMuffleSound();
                }
            }

            _isCritical = true;
        }
        else
        {
            if (_isCritical)
            {
                _isCritical = false;
            }
        }

        SetInvulnerability(_hitInvulnerabilityTime);

        GameEvents.Instance.PlayerDamaged(damageAmount, this);
    }

    public void Heal(float healAmount, GameObject source, bool giveOverflow = false)
    {
        if (!giveOverflow)
        {
            if (_currentHealth + healAmount < _maxHealth)
            {
                _currentHealth += healAmount;
            }
            else
            {
                if(_currentHealth < _maxHealth)
                {
                    _currentHealth = _maxHealth;
                }
            }
        }
        else
        {
            _currentHealth += healAmount;

            if (_currentHealth > _maxHealth + _maxOverflowHealth)
            {
                _currentHealth = _maxHealth + _maxOverflowHealth;
            }
        }

        BackgroundEffectsManager.Instance.HealEffect(.05f, .1f, .16f);

        if ((_currentHealth / _maxHealth) * 100 > 20)
        {
            BackgroundEffectsManager.Instance.StopCriticalEffect();

            if (_isCritical)
            {
                if (_audioManager != null)
                {
                    _audioManager.StopMuffleSound();
                }
            }

            _isCritical = false;
        }

        GameEvents.Instance.PlayerHealed(healAmount, this);
    }

    public void GiveArmor(float armorAmount, GameObject source)
    {
        _currentArmor += armorAmount;

        if (_currentArmor > _maxArmor)
        {
            _currentArmor = _maxArmor;
        }

        GameEvents.Instance.PlayerGetArmor(armorAmount, this);
    }

    Coroutine InvulnerabilityCoroutine;
    public void SetInvulnerability(float invulnerabilityTime)
    {
        if(InvulnerabilityCoroutine != null)
        {
            StopCoroutine(InvulnerabilityCoroutine);
        }
        InvulnerabilityCoroutine = StartCoroutine(CRT_Invulnerability(invulnerabilityTime));
    }

    IEnumerator CRT_Invulnerability(float invulnerabilityTime = .25f)
    {
        LayerMask defaultLayer = gameObject.layer;
        LayerMask invulLayer = LayerMask.NameToLayer("Invulnerability");

        if(defaultLayer == invulLayer)
        {
            defaultLayer = LayerMask.NameToLayer("Player");
            Debug.Log("PlayerHealthManager: Layer was still set on Invulnerability when trying to store the Player layer, putting it to Player by default");
        }

        gameObject.layer = invulLayer;

        yield return new WaitForSeconds(invulnerabilityTime);

        gameObject.layer = defaultLayer;

        yield break;
    }

    void Die()
    {
        _audioManager.PlayDeath();
        ObjectPooler.Instance.SpawnObject("DeathParticles", transform.position, Quaternion.identity);
        gameObject.SetActive(false);
        BackgroundEffectsManager.Instance.DeathEffect();
        CameraEffects.Instance.Shockwave(transform.position, 35f, 0.10f, -0.16f, 2f);

        LevelManager.Instance.EndLevel("Player Death");
    }

    public float GetHealth()
    {
        return _currentHealth;
    }

    public float GetMaxHealth()
    {
        return _maxHealth;
    }

    public float GetMaxOverflowHealth()
    {
        return _maxOverflowHealth;
    }

    public float GetArmor()
    {
        return _currentArmor;
    }

    public float GetMaxArmor()
    {
        return _maxArmor;
    }
}