using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WandererTargetEnemy : Enemy
{
    [SerializeField] private GameObject _spinningSaw = null;
    [SerializeField] private float _maxWanderDistanceFromPlayer = 20f;
    [SerializeField] private float _minWanderDistanceFromPlayer = 10f;
    [SerializeField] private float _wanderRadius = 10f;
    [SerializeField] private float _scareDistanceFromPlayer = 5f;
    [SerializeField] private float _wanderSpeed = 8f;
    [SerializeField] private float _fleeSpeed = 14f;

    enum WandererState
    {
        WANDER,
        FOLLOW,
        FLEE
    }

    WandererState _wandererState = WandererState.WANDER;

    FlyingPathfindingMovement _movement;

    protected override void Awake()
    {
        base.Awake();
        _movement = GetComponent<FlyingPathfindingMovement>();
    }

    protected override void Start()
    {
        base.Start();

        _minWanderDistanceFromPlayer = Mathf.Clamp(_minWanderDistanceFromPlayer, 0, _maxWanderDistanceFromPlayer);

        _movement.StartWandering(_wanderRadius);
        StartCoroutine(PeriodicallyCheckForPlayerDistance());
    }

    protected override void Update()
    {
        base.Update();
    }

    void OnTargetReached()
    {

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

    void CheckForPlayerDistance()
    {
        float distance = Vector2.Distance(transform.position, _player.transform.position);

        float randomDistanceBetweenMinAndMax = UnityEngine.Random.Range(_minWanderDistanceFromPlayer, _maxWanderDistanceFromPlayer);

        if(distance > _maxWanderDistanceFromPlayer)
        {
            if (_wandererState != WandererState.FOLLOW)
            {
                _wandererState = WandererState.FOLLOW;
                StartFollowingPlayer();
            }
        }
        else if (distance <= randomDistanceBetweenMinAndMax && distance > _scareDistanceFromPlayer)
        {
            if(_wandererState != WandererState.WANDER && _wandererState != WandererState.FLEE)
            {
                _wandererState = WandererState.WANDER;
                StartWandering();
            }
        }
        else if(distance <= _scareDistanceFromPlayer)
        {
            if(_wandererState != WandererState.FLEE)
            {
                _wandererState = WandererState.FLEE;
                StartFleeing();
            }
        }
    }

    void StartFollowingPlayer()
    {
        _movement.MoveTowardsTarget(_player.transform, OnTargetReached);
        _movement.SetSpeed(_wanderSpeed);

        _animator.Play("BubbleMonsterIdle");
    }

    void StartWandering()
    {
        _movement.StartWandering(_wanderRadius);
        _movement.SetSpeed(_wanderSpeed);
       _animator.Play("BubbleMonsterIdle");
    }

    void StartFleeing()
    {
        _movement.FleeFromTarget(_player.transform, OnTargetReached);
        _movement.SetSpeed(_fleeSpeed);
        _animator.Play("BubbleMonsterFleeing");

        Action callback = new Action(delegate { StartShooting(this); });
        EnemyAttacksCooldownManager.Instance.RequestAttack(EnemyAttackType.RANGED, callback);
    }

    void StartShooting(WandererTargetEnemy caller)
    {
        if(caller == null)
        {
            return;
        }

        //Debug.Log("Starting to Shoot after " + Time.time);
    }
}
