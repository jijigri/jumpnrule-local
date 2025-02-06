using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BruteChaserEnemy : Enemy
{
    [Header("Brute Settings")]
    [SerializeField] private float _attackDamage = 40f;
    [SerializeField] private float _distanceFromPlayerToAttack = 2f;

    PlatformPathfindingMovement _movement;

    protected override void Awake()
    {
        base.Awake();
        _movement = GetComponent<PlatformPathfindingMovement>();
    }

    protected override void Start()
    {
        base.Start();

        _movement.SetInitialSpeed(_movement.GetInitialSpeed() + (LevelManager.Instance.CurrentRound * 2f));
        _movement.SetTarget(_player.transform);
        _movement.StartMoving();
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void OnFalterStart()
    {
        base.OnFalterStart();
        _movement.StopMoving();
    }

    protected override void OnFalterStop()
    {
        base.OnFalterStop();
        _movement.StartMoving();
    }
}
