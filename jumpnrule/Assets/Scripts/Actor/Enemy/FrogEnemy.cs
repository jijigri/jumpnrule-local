using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrogEnemy : Enemy
{
    [SerializeField] GameObject _spinningSaw = null;
    [SerializeField] float _wanderRadius = 5f;
    [SerializeField] float _fleeRadius = 15f;
    [SerializeField] float _wanderTimeBeforeMoving = 5f;
    [SerializeField] float _wanderSpeed = 80f;
    [SerializeField] float _fleeingTimeBeforeMoving = 5f;
    [SerializeField] float _fleeingSpeed = 100f;
    [SerializeField] float _distanceFromPlayerToStartFleeing = 5f;
    [SerializeField] float _timeBeforeShooting = 1f;
    [SerializeField] float _timeToStopFleeing = 5f;

    float _currentTimeBeforeMoving = 5f;
    float _currentWanderRadius = 5f;

    bool _tryingToFlee = false;
    bool _isWarmingUp = false;

    Vector2 _direction;

    PlatformPathfindingMovement _movement;
    EnemyAnimationManager _animationManager;

    enum WandererState
    {
        WANDER,
        FLEE
    }

    private WandererState _wandererState = WandererState.WANDER;

    protected override void Awake()
    {
        base.Awake();

        _movement = GetComponent<PlatformPathfindingMovement>();
        _animationManager = GetComponent<EnemyAnimationManager>();
    }

    protected override void Start()
    {
        base.Start();

        _currentTimeBeforeMoving = _wanderTimeBeforeMoving;
        _movement.SetSpeed(_wanderSpeed);
        _currentWanderRadius = _wanderRadius;

        StopAllCoroutines();
        StartCoroutine("CRT_Move");
        StartCoroutine(PeriodicallyCheckForPlayerDistance());
    }

    protected override void Update()
    {
        base.Update();
    }

    private IEnumerator CRT_Move()
    {
        while (true)
        {
            if (_movement.HasReachedEndOfPath)
            {

                CheckForPlayerDistance();
                if (_tryingToFlee)
                {
                    _wandererState = WandererState.FLEE;
                    StopCoroutine(StartFleeing());
                    StartCoroutine(StartFleeing());

                    yield return new WaitForSeconds(_timeBeforeShooting + .5f);

                    _tryingToFlee = false;
                }

                yield return new WaitForSeconds(_currentTimeBeforeMoving);

                Vector2 movePosition = (Vector2)_player.transform.position + (UnityEngine.Random.insideUnitCircle.normalized * _currentWanderRadius);
                _movement.SetTarget(movePosition);
                _movement.StartMoving();
            }
            yield return null;
        }
    }


    IEnumerator PeriodicallyCheckForPlayerDistance()
    {
        yield return new WaitForSeconds(UnityEngine.Random.Range(0, .25f));

        while (true)
        {
            CheckForPlayerDistance();
            yield return new WaitForSeconds(.1f);
        }
    }

    protected override void LateUpdate()
    {
        if (_isWarmingUp)
        {
            _animationManager.SetDirection((int)_direction.x);
        }
    }

    void CheckForPlayerDistance()
    {
        float distance = Vector2.Distance(transform.position, _player.transform.position);

        float randomDistanceBetweenMinAndMax = _distanceFromPlayerToStartFleeing;

        if (distance < _distanceFromPlayerToStartFleeing)
        {
            _tryingToFlee = true;
        }
    }

     void StartWandering()
     {
        _currentTimeBeforeMoving = _wanderTimeBeforeMoving;

        _movement.SetSpeed(_wanderSpeed);

        _currentWanderRadius = _wanderRadius;
     }

    IEnumerator StartFleeing()
    {
        _isWarmingUp = true;

        _movement.StopMoving();

        _direction = (_player.transform.position - transform.position).normalized;
        _animationManager.AttackWarmup();

        yield return new WaitForSeconds(_timeBeforeShooting);

        _animationManager.Attack();

        Action callback = new Action(delegate { StartShooting(this); });
        EnemyAttacksCooldownManager.Instance.RequestAttack(EnemyAttackType.RANGED, callback);

        yield return new WaitForSeconds(.5f);

        _movement.StartMoving();

        _currentTimeBeforeMoving = _fleeingTimeBeforeMoving;

        _movement.SetSpeed(_fleeingSpeed);

        _currentWanderRadius = _fleeRadius;

        Vector2 movePosition = (Vector2)_player.transform.position + (UnityEngine.Random.insideUnitCircle.normalized * _currentWanderRadius);
        _movement.SetTarget(movePosition);
        _movement.StartMoving();

        yield return new WaitForSeconds(_timeToStopFleeing);

        if (_wandererState != WandererState.WANDER)
        {
            _wandererState = WandererState.WANDER;
            StopCoroutine(StartFleeing());
            StartWandering();
        }

        yield break;
    }

    void StartShooting(FrogEnemy caller)
    {
        if (caller == null)
        {
            return;
        }

        /*
        SpinningSaw saw = Instantiate(_spinningSaw, transform.position, Quaternion.identity).GetComponent<SpinningSaw>();
        if(saw != null)
        {
            saw.Initialize(4, 1, 30, new Vector2(_direction.x * 5, 1));
        }
        */

        float posY = UnityEngine.Random.Range(-10, 10);
        Vector2 position = new Vector2(0, posY);
        //ObjectPooler.Instance.SpawnObject("DeadlyLaser", position, Quaternion.identity);

        _isWarmingUp = false;
    }
}
