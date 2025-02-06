using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Seeker))]
public class FlyingPathfindingMovement : MonoBehaviour, ICanSetSpeed
{
    [SerializeField] private float _pathSearchTickTime = .25f;
    [SerializeField] float _speed = 200f;
    [SerializeField] float _nextWaypointDistance = 3f;

    Transform _target;

    Path _path;
    int _currentWaypoint = 0;
    bool _reachedEndOfPath = false;

    bool _isWandering = false;
    float _lastWanderRadius = 1f;

    float _initialSpeed;

    Seeker _seeker;
    Rigidbody2D _rigidBody;

    Action _positionReachedCallback;

    private void Awake()
    {
        _seeker = GetComponent<Seeker>();
        _rigidBody = GetComponent<Rigidbody2D>();
        _initialSpeed = _speed;
    }

    public void SetSpeed(float speed)
    {
        _speed = speed;
    }

    public void ResetSpeed()
    {
        _speed = _initialSpeed;
    }

    private void FixedUpdate()
    {
        if(_path == null)
        {
            return;
        }

        if(_currentWaypoint >= _path.vectorPath.Count)
        {
            //Reached the end of path!
            _reachedEndOfPath = true;

            _positionReachedCallback?.Invoke();

            return;
        }
        else
        {
            _reachedEndOfPath = false;
        }

        Vector2 direction = (Vector2)_path.vectorPath[_currentWaypoint] - _rigidBody.position;
        direction.Normalize();
        Vector2 velocity = direction * (_speed * 100) * Time.fixedDeltaTime;

        _rigidBody.AddForce(velocity);

        float distance = Vector2.Distance(_rigidBody.position, _path.vectorPath[_currentWaypoint]);
        if(distance < _nextWaypointDistance)
        {
            _currentWaypoint++;
        }
    }

    public void MoveTowardsTarget(Transform target, Action positionReachedCallback)
    {
        _isWandering = false;

        if (!_seeker.IsDone())
        {
            _seeker.CancelCurrentPathRequest();
        }

        _target = target;
        _positionReachedCallback = positionReachedCallback;

        StopAllCoroutines();
        StartCoroutine(UpdatePathToTarget(target.position));     
    }

    public void FleeFromTarget(Transform target, Action positionReachedCallback)
    {
        _isWandering = false;

        if (!_seeker.IsDone())
        {
            _seeker.CancelCurrentPathRequest();
        }

        _target = target;
        _positionReachedCallback = positionReachedCallback;

        StopAllCoroutines();
        StartCoroutine(UpdatePathToOppositeToTarget(target));
    }

    public void MoveTowardsPosition(Vector2 position, Action positionReachedCallback)
    {
        if (!_seeker.IsDone())
        {
            _seeker.CancelCurrentPathRequest();
        }

        StopAllCoroutines();

        _target = null;
        _positionReachedCallback = positionReachedCallback;

        _seeker.StartPath(_rigidBody.position, position, OnPathFoundCallback);
    }

    public void StartWandering(float wanderRadius)
    {
        Vector2 point = UnityEngine.Random.insideUnitCircle * wanderRadius;
        Vector2 position = (Vector2)_rigidBody.transform.position + point;

        _isWandering = true;
        _lastWanderRadius = wanderRadius;

        MoveTowardsPosition(position, OnPositionReached);
    }

    public void StartMovingInDirection(Vector2 direction, Action positionReachedCallback)
    {
        _isWandering = false;

        if (!_seeker.IsDone())
        {
            _seeker.CancelCurrentPathRequest();
        }

        _positionReachedCallback = positionReachedCallback;

        StopAllCoroutines();
        StartCoroutine(UpdatePathToTarget(_rigidBody.position + direction, true));
    }

    IEnumerator UpdatePathToTarget(Vector2 targetPosition, bool relativeToPosition = false)
    {
        while (true)
        {
            Vector2 endPosition = relativeToPosition ? _rigidBody.position + targetPosition : targetPosition;
            _seeker.StartPath(_rigidBody.position, endPosition, OnPathFoundCallback);
            yield return new WaitForSeconds(_pathSearchTickTime);
        }
    }

    IEnumerator UpdatePathToOppositeToTarget(Transform targetPosition)
    {
        while (true)
        {
            Vector2 endDirection = ((Vector2)targetPosition.position - _rigidBody.position).normalized * -5;
            _seeker.StartPath(_rigidBody.position, _rigidBody.position + endDirection, OnPathFoundCallback);
            yield return new WaitForSeconds(_pathSearchTickTime);
        }
    }

    void OnPositionReached()
    {
        if (_isWandering)
        {
            StartWandering(_lastWanderRadius);
        }
    }

    void OnPathFoundCallback(Path path)
    {
        if (!path.error)
        {
            _path = path;
            _currentWaypoint = 0;
        }
    }
}
