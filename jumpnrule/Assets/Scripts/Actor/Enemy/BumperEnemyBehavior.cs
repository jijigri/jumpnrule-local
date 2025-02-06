using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BumperEnemyBehavior : ShooterEnemy
{
    [SerializeField] private float _speed = 4f;
    [SerializeField] private float _collisionDamage = 10f;
    [SerializeField] private AnimationCurve _timeBeforeShootingCurve = default;
    [SerializeField] private float _ShotsBeforeReachingShortestShootTime = 8;
    [SerializeField] private LayerMask _solidMask = default;

    private float _timeBeforeNextShot;

    private Vector2 _moveDirection = Vector2.zero;

    private bool _canMove = true;

    private string _shootSound = "event:/Enemies/Bumper/BumperShoot";
    private EventInstance _shootEvent;
    private EventInstance _preShootEvent;

    int _shotIndex = 0;

    protected override void Awake()
    {
        base.Awake();
        _shootEvent = RuntimeManager.CreateInstance(_shootSound);
        _preShootEvent = RuntimeManager.CreateInstance("event:/Enemies/Bumper/BumperPreShoot");
    }

    protected override void Start()
    {
        base.Start();

        _moveDirection = Helper.GetRandomDirection();

        StartCoroutine(ShootBehavior());
    }

    private IEnumerator ShootBehavior()
    {
        float curveTime = (_shotIndex / _ShotsBeforeReachingShortestShootTime) * 1;
        _timeBeforeNextShot = _timeBeforeShootingCurve.Evaluate(curveTime);
        _timeBeforeNextShot += Random.Range(0, 2f);

        while (true)
        {
            yield return new WaitForSeconds(_timeBeforeNextShot);

            _shotIndex++;

            _moveDirection = Helper.GetRandomDirection();

            _canMove = false;

            RuntimeManager.AttachInstanceToGameObject(_preShootEvent, transform);
            _preShootEvent.start();

            yield return new WaitForSeconds(.75f);

            StartShooting(0f);

            RuntimeManager.AttachInstanceToGameObject(_shootEvent, transform);
            _shootEvent.start();

            yield return new WaitForSeconds(.4f);

            curveTime = (_shotIndex / _ShotsBeforeReachingShortestShootTime) * 1;
            _timeBeforeNextShot = _timeBeforeShootingCurve.Evaluate(curveTime);
            _timeBeforeNextShot += Random.Range(0, .5f);

            _timeBeforeNextShot = Mathf.Clamp(_timeBeforeNextShot, .1f, 100f);

            _canMove = true;
        }
    }

    void FixedUpdate()
    {
        if (_canMove)
        {
            _rigidBody.AddForce(_moveDirection * (_speed * 100) * Time.fixedDeltaTime);

            bool isCollidingWithWall = Physics2D.Raycast(transform.position, _moveDirection, 1f, _solidMask);

            if (isCollidingWithWall)
            {
                _moveDirection = Helper.GetRandomDirection();
            }
        }
    }

    protected override void OnFalterStart()
    {
        StopCoroutine(ShootBehavior());
    }

    protected override void OnFalterStop()
    {
        StartCoroutine(ShootBehavior());
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Collider2D collider = collision.collider;

        if (collider.CompareTag("Player"))
        {
            PlatformController controller = collider.GetComponent<PlatformController>();

            if (_isFaltered)
            {
                OnPlayerStomp(controller);
                return;
            }

            if (controller.GetVelocity().y < 0)
            {
                if (collider.transform.position.y > transform.position.y)
                {
                    if (controller != null)
                    {
                        //PLAYER ON TOP, KILL ENEMY
                        OnPlayerStomp(controller);
                        return;
                    }
                }
            }

            ICanBeDamaged iDamageable = collider.GetComponent<ICanBeDamaged>();
            if(iDamageable != null)
            {
                iDamageable.DealDamage(_collisionDamage, gameObject, Vector2.zero);
            }

            Die(gameObject);
        }
    }

    void OnPlayerStomp(PlatformController controller)
    {
        controller.OverrideVelocity(Vector2.up * 25);
        Die(gameObject);
    }
}
