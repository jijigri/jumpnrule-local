using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPistolHolder : ShooterEnemy
{
    private enum EnemyState
    {
        SEEKING,
        MOVING,
        SHOOTING
    }

    [Space(10)]
    [Header("Pistol Holder Settings")]
    [SerializeField] private float _minDistanceFromPlayer = 8;
    [SerializeField] private float _maxDistanceFromPlayer = 12;
    [SerializeField] private float _seekingSpeed = 10;
    [SerializeField] private float _movingSpeed = 10;
    [SerializeField] private float _timeBeforeShooting = .8f;

    Vector2 _direction = Vector2.zero;

    float _currentSpeed = 10;
    float _distanceFromPlayer = 10;

    private EnemyState _enemyState = EnemyState.SEEKING;

    protected override void Start()
    {
        base.Start();

        _distanceFromPlayer = Random.Range(_minDistanceFromPlayer, _maxDistanceFromPlayer);
    }

    private void FixedUpdate()
    {
        if (_enemyState == EnemyState.SEEKING)
        {
            _direction = (Vector2)_player.transform.position - (Vector2)transform.position;
            _direction.Normalize();

            _currentSpeed = _seekingSpeed;

            if (Vector2.Distance(transform.position, _player.transform.position) < _maxDistanceFromPlayer)
            {
                SwitchToMovingState();
            }
        }

        if (_enemyState != EnemyState.SHOOTING)
        {
            _rigidBody.AddForce(_direction * (_movingSpeed * (!_isFaltered ? 1000 : 200)) * Time.fixedDeltaTime);
        }
    }

    IEnumerator SwitchToSeekingState()
    {
        yield return new WaitForSeconds(.5f);

        _distanceFromPlayer = Random.Range(_minDistanceFromPlayer, _maxDistanceFromPlayer);

        _enemyState = EnemyState.SEEKING;

        yield break;
    }

    void SwitchToMovingState()
    {
        _enemyState = EnemyState.MOVING;
        _direction = Helper.GetRandomDirection();

        _currentSpeed = _movingSpeed;

        StartCoroutine(SwitchToShootingState());
    }

    IEnumerator SwitchToShootingState()
    {
        float timeBeforeStopping = Random.Range(0f, 1f);
        yield return new WaitForSeconds(timeBeforeStopping);

        _enemyState = EnemyState.SHOOTING;

        _direction = Vector2.zero;

        StartShooting(_timeBeforeShooting);
    }

    protected override void OnMagazineEmpty()
    {
        base.OnMagazineEmpty();
        StartCoroutine(SwitchToSeekingState());
    }

    protected override void CancelShooting()
    {
        base.CancelShooting();
        _enemyState = EnemyState.SEEKING;
        Debug.Log("CHANGING STATE");
    }
}
