using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour, IHasPlayerOwner
{
    enum ShootType
    {
        MANUAL,
        AUTOMATIC
    }

    [Header("Global options")]
    [SerializeField] protected Transform _shootPoint = null;
    [SerializeField] protected float _initialBulletLifetime = 10;
    [SerializeField] protected float _gunDamage = 10;
    [SerializeField] protected float _gunShootRate = 10;
    [SerializeField] private ShootType _initialShootType = ShootType.MANUAL;
    [SerializeField] private CooldownManager _primaryCooldown;
    [SerializeField] private CooldownManager _secondaryCooldown;
    [Space(1)]
    [Header("Visual effects options")]
    [SerializeField] protected string[] _primaryShootEffectTags = new string[1] { "Empty" };
    [SerializeField] protected string[] _secondaryShootEffectTags = new string[0];
    [Header("Camera shake options")]
    [SerializeField] protected float _primaryShakeStrength = 0.1f;
    [SerializeField] protected float _primaryShakeDuration = 0.1f;
    [SerializeField] protected float _secondaryShakeStrength = 0.1f;
    [SerializeField] protected float _secondaryShakeDuration = 0.1f;
    [Header("Sound options")]
    [SerializeField] private string primaryShotSound = default;
    [SerializeField] private string secondaryShotSound = default;

    [SerializeField] private EventInstance primaryShotEvent = default;
    [SerializeField] private EventInstance secondaryShotEvent = default;

    public float AimAngle { get; set; }

    protected const float _AngleOffset = 180;

    protected float _timeBeforeNextShot = 60;

    protected SpriteRenderer _spriteRenderer;
    protected Animation _recoilAnimation;

    protected Camera _camera;
    protected ObjectPooler _pooler;
    protected CameraEffects _cameraEffects;

    protected GameObject _player;

    public bool canHardFalter { get; set; } = false;
    public bool isActive { get; set; } = true;

    public virtual void Awake()
    {
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _recoilAnimation = _spriteRenderer.gameObject.GetComponent<Animation>();

        if (primaryShotSound != "")
        {
            primaryShotEvent = RuntimeManager.CreateInstance(primaryShotSound);
        }

        if (secondaryShotSound != "")
        {
            secondaryShotEvent = RuntimeManager.CreateInstance(secondaryShotSound);
        }

        _camera = Camera.main;
    }
    
    protected virtual void OnEnable()
    {
        RunnerStat stat = GetComponentInParent<RunnerStat>();
        if (stat != null)
        {
            SetPlayerOwner(stat.gameObject);
        }
    }

    public virtual void Start()
    {
        GameEvents.Instance.onEnemyFalter += OnEnemyFalter;

        _pooler = ObjectPooler.Instance;
        _cameraEffects = CameraEffects.Instance;
    }

    private void OnDestroy()
    {
        GameEvents.Instance.onEnemyFalter -= OnEnemyFalter;
    }

    void OnEnemyFalter(Gun gun)
    {
        if(gun == this)
        {
            canHardFalter = false;
        }
        else
        {
            canHardFalter = true;
        }
    }

    public virtual void Update()
    {
        if (_timeBeforeNextShot < 60.0f)
        {
            _timeBeforeNextShot += Time.deltaTime * _gunShootRate;
        }
    }

    public virtual void OnPrimaryPressed()
    {

    }

    public virtual void OnPrimaryReleased()
    {
        if(_primaryCooldown != null)
        {
            _primaryCooldown.OnKeyReleased();
        }
    }

    public virtual void OnPrimary()
    {
        if (_primaryCooldown != null)
        {
            if (_primaryCooldown.CheckCooldown())
            {
                Shoot();
            }
        }
    }

    public virtual void OnSecondaryPressed()
    {
        
    }

    public virtual void OnSecondaryReleased()
    {
        if (_secondaryCooldown != null)
        {
            _secondaryCooldown.OnKeyReleased();
        }
    }

    public virtual void OnSecondary()
    {
        if (_secondaryCooldown != null)
        {
            if (_secondaryCooldown.CheckCooldown())
            {
                ShootSecondary();
            }
        }
    }

    public virtual void OnSwitch(bool switchToSelf)
    {
        if (switchToSelf)
        {
            _spriteRenderer.enabled = true;
            isActive = true;
        }
        else
        {
            _spriteRenderer.enabled = false;
            isActive = false;
        }
    }

    public virtual void Rotate(float angle)
    {
        AimAngle = angle;
    }

    public virtual void Shoot()
    {
        if(isActive == false)
        {
            return;
        }

        PrimaryShootFeedback();
    }

    public virtual void PrimaryShootFeedback()
    {
        if (_primaryShootEffectTags.Length > 0)
        {
            string randomShootEffect = _primaryShootEffectTags[Random.Range(0, _primaryShootEffectTags.Length)];
            _pooler.SpawnObject(randomShootEffect, _shootPoint.position, Quaternion.Euler(0, 0, AimAngle + _AngleOffset));
        }

        if (_recoilAnimation != null)
        {
            _recoilAnimation.Play();
        }

        if (_cameraEffects != null)
        {
            _cameraEffects.Shake(_primaryShakeDuration, _primaryShakeStrength);      
        }

        primaryShotEvent.start();
    }

    public virtual void ShootSecondary()
    {
        if (isActive == false)
        {
            return;
        }

        SecondaryShootFeedback();
    }

    public virtual void SecondaryShootFeedback()
    {
        if (_secondaryShootEffectTags.Length > 0)
        {
            string randomShootEffect = _secondaryShootEffectTags[Random.Range(0, _secondaryShootEffectTags.Length)];
            _pooler.SpawnObject(randomShootEffect, _shootPoint.position, Quaternion.Euler(0, 0, AimAngle + _AngleOffset));
        }

        if (_recoilAnimation != null)
        {
            _recoilAnimation.Play();
        }

        if (_cameraEffects != null)
        {
            _cameraEffects.Shake(_secondaryShakeDuration, _secondaryShakeStrength);
        }

        secondaryShotEvent.start();
    }

    public void SetPlayerOwner(GameObject playerOwner)
    {
        _player = playerOwner;
    }

    public GameObject GetPlayerOwner()
    {
        return _player;
    }
}
