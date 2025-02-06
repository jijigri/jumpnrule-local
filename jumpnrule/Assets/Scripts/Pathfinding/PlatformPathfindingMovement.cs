using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformPathfindingMovement : MonoBehaviour, ICanSetSpeed, ICanRetreat
{
    enum MoveState
    {
        MOVING,
        JUMPING,
        FALLING
    }

    [SerializeField] float _speed = 5;
    [SerializeField] float _pathUpdateTickTime = .25f;
    [SerializeField] float jumpAngle = 60f;
    [SerializeField] LayerMask _solidMask = default;

    private Transform _target;
    private Vector2 _targetPosition;

    bool _isTargetDynamic = true;

    Vector2[] _path;
    int _targetIndex;

    float _initialSpeed = 0;

    public bool HasReachedEndOfPath { get; private set; } = true;
    public bool CanMove { get; set; } = true;

    MoveState _moveState = MoveState.MOVING;
    Vector2[] _jumpPath = new Vector2[0];

    public bool IsGrounded { get; private set; } = false;

    Rigidbody2D _rigidBody;
    BoxCollider2D _collider;
    EnemyAnimationManager _animationManager;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _collider = GetComponent<BoxCollider2D>();
        _animationManager = GetComponent<EnemyAnimationManager>();
        _initialSpeed = _speed;
    }

    public void SetTarget(Transform target)
    {
        if (CanMove)
        {
            _target = target;
            _isTargetDynamic = true;
        }
    }

    public void SetTarget(Vector2 position)
    {
        if (CanMove)
        {
            _targetPosition = position;
            _isTargetDynamic = false;
        }
    }

    [ContextMenu("Start Moving")]
    public void StartMoving()
    {
        if (CanMove)
        {
            _moveState = MoveState.MOVING;

            StopCoroutine("CRT_StartMoving");
            StartCoroutine("CRT_StartMoving");
        }
    }

    [ContextMenu("Stop Moving")]
    public void StopMoving()
    {
        if (CanMove)
        {
            StopCoroutine("CRT_FollowPath");
            StopCoroutine("CRT_StartMoving");
        }
    }

    public void SetSpeed(float speed)
    {
        _speed = speed;
    }

    public void ResetSpeed()
    {
        _speed = _initialSpeed;
    }

    public void SetInitialSpeed(float speed)
    {
        _initialSpeed = speed;
    }

    public float GetSpeed()
    {
        return _speed;
    }

    public float GetInitialSpeed()
    {
        return _initialSpeed;
    }

    IEnumerator CRT_StartMoving()
    {
        while (true)
        {
            if (_moveState == MoveState.MOVING)
            {
                StartNewPath();
            }

            if (!_isTargetDynamic)
            {
                yield break;
            }

            yield return new WaitForSeconds(_pathUpdateTickTime);
        }
    }

    void StartNewPath()
    {
        Debug.Log("STARTING NEW PATH YES");

        if (_isTargetDynamic)
        {
            PlatformPathRequestManager.RequestPath(transform.position, _target.position, OnPathFoundCallback);
        }
        else
        {
            PlatformPathRequestManager.RequestPath(transform.position, _targetPosition, OnPathFoundCallback);
        }
    }

    public void OnPathFoundCallback(WayPoint[] newPath, bool pathSuccessful)
    {
        if (this != null)
        {
            if (pathSuccessful)
            {
                _targetIndex = 0;
                _path = new Vector2[newPath.Length];
                for(int i = 0; i < newPath.Length; i++)
                {
                    _path[i] = newPath[i].position;
                }
                StopCoroutine("CRT_FollowPath");
                StartCoroutine("CRT_FollowPath");
            }
        }
    }

    IEnumerator CRT_FollowPath()
    {
        if (_path.Length < 1)
        {
            Debug.Log("Stopping because path is too short");
            yield break;
        }

        HasReachedEndOfPath = false;
        Vector2 currentWaypoint = _path[0];

        while (true)
        {
            if (Vector2.Distance((Vector2)transform.position, currentWaypoint) < .25f && _moveState == MoveState.MOVING)
            {
                _targetIndex++;
                if (_targetIndex >= _path.Length)
                {
                    _targetIndex = 0;
                    _path = new Vector2[0];

                    HasReachedEndOfPath = true;

                    //StartNewPath();

                    //_isJumping = false;

                    if (!CanMove)
                    {
                        StopRetreating();
                    }

                    Debug.Log("STOPPING COROUTINE BECAUSE HAS REACHED END OF PATH");
                    yield break;
                }

                currentWaypoint = _path[_targetIndex];

                //StartNewPath();

                //_isJumping = false;
            }

            SetMoveState(currentWaypoint);

            //CheckGrounded();
            SetGravity();
            SetMovement(currentWaypoint);
            

            if (_animationManager != null)
            {
                if (_moveState == MoveState.MOVING)
                {
                    _animationManager.SetGrounded(true);
                }
                else
                {
                    _animationManager.SetGrounded(false);
                }
            }

            gameObject.name = "FOLLOWER: " + IsGrounded + " " + _moveState;

            Debug.Log("FOLLOWPATH COROUTINE RUNNING");

            yield return new WaitForFixedUpdate();
        }
    }

    void SetMoveState(Vector2 currentWaypoint)
    {
        float yDistanceFromNextWaypoint = currentWaypoint.y - transform.position.y;
        if (yDistanceFromNextWaypoint > .9f)
        {
            if (_moveState != MoveState.JUMPING)
            {
                //CHARACTER NEEDS TO JUMP
                _moveState = MoveState.JUMPING;

                CreateJumpParabola(transform.position, currentWaypoint);
            }
        }
        else if (yDistanceFromNextWaypoint < -.9f)
        {
            if (_moveState != MoveState.FALLING && _moveState != MoveState.JUMPING)
            {
                _moveState = MoveState.FALLING;
            }
        }
        else
        {
            if (_moveState == MoveState.FALLING)
            {
                _moveState = MoveState.MOVING;
            }
        }

        if (_rigidBody.velocity.magnitude < .5f && _moveState == MoveState.JUMPING)
        {
            _moveState = MoveState.FALLING;
        }
    }

    void CheckGrounded()
    {
        if (_moveState == MoveState.JUMPING || _moveState == MoveState.FALLING)
            return;

        Debug.DrawRay(_rigidBody.position,  Vector2.down * ((_collider.size.y / 2) + .1f));
        bool isGroundedLeft = Physics2D.Raycast(_rigidBody.position - new Vector2(_collider.size.x / 2, 0), Vector2.down, (_collider.size.y / 2) + .1f);
        bool isGroundedRight = Physics2D.Raycast(_rigidBody.position + new Vector2(_collider.size.x / 2, 0), Vector2.down, (_collider.size.y / 2) + .1f);

        if (isGroundedLeft || isGroundedRight)
        {
            IsGrounded = true;
        }
        else
        {
            IsGrounded = false;
        }

        Debug.Log("IsGrounded: " + IsGrounded);
    }

    void SetGravity()
    {
        if (_moveState == MoveState.JUMPING)
        {
            _rigidBody.gravityScale = 0f;
        }
        else
        {
            /*
            if (!IsGrounded || _moveState == MoveState.FALLING)
            {
                _rigidBody.gravityScale = 30f;
            }
            else
            {
                _rigidBody.gravityScale = 0f;
            }
            */

            if (_moveState == MoveState.FALLING)
            {
                _rigidBody.gravityScale = 30f;
            }
            else
            {
                _rigidBody.gravityScale = 0f;
            }
        }
    }

    void SetMovement(Vector2 currentWaypoint)
    {
        if (_moveState != MoveState.JUMPING)
        {
            Move(currentWaypoint);
        }
        else
        {
            MoveAlongJumpParabola();
        }
    }

    void Move(Vector2 targetPos)
    {
        Vector2 direction = targetPos - (Vector2)transform.position;
        //direction.y = 0;
        direction.Normalize();

        if (!HasReachedEndOfPath)
        {
            /*
            if(Mathf.Abs(direction.x) < .5f)
            {
                Debug.Log("Assigning random direction for some reason");
                direction.x = 1;
            }
            */

            _rigidBody.AddForce(direction * (_speed * 100) * Time.fixedDeltaTime);
        }

        if (_animationManager != null)
        {
            _animationManager.SetDirection(Mathf.RoundToInt(Mathf.Sign(direction.x)));
        }
    }

    int currentJumpWaypoint = 0;

    void CreateJumpParabola(Vector2 start, Vector2 end)
    {
        currentJumpWaypoint = 0;

        float yDistance = end.y - start.y;
        float jumpHeight = yDistance / 2;
        int jumpPrecision = 10;
        _jumpPath = new Vector2[jumpPrecision + 1];

        for(int t = 0; t <= jumpPrecision; t++)
        {
            _jumpPath[t] = Helper.Parabola(start, end, jumpHeight, (float)t / (float)jumpPrecision);

            /*
            GameObject debug = new GameObject("Debug");
            debug.AddComponent<SpriteRenderer>().sprite = debugSprite;
            debug.transform.position = _jumpPath[t];
            */
        }
    }

    void MoveAlongJumpParabola()
    {
        if (currentJumpWaypoint < _jumpPath.Length)
        {
            Vector2 direction = _jumpPath[currentJumpWaypoint] - (Vector2)transform.position;
            direction.Normalize();
            _rigidBody.AddForce(direction * (_speed * 100) * Time.fixedDeltaTime);

            if(Vector2.Distance(transform.position, _jumpPath[currentJumpWaypoint]) < .75f)
            {
                currentJumpWaypoint++;
            }
        }
        else
        {
            _moveState = MoveState.MOVING;
        }
    }

    Transform _initialTarget;
    Vector2 _initialTargetPosition;
    bool _isInitialTargetDynamic;
    public void StartRetreating(Vector2 retreatPosition)
    {
        _isInitialTargetDynamic = _isTargetDynamic;

        if (_isTargetDynamic)
        {
            _initialTarget = _target;
        }
        else
        {
            _initialTargetPosition = _targetPosition;
        }

        Debug.Log("CHANGING POSITION AND T ATATA");

        SetTarget(retreatPosition);
        StartMoving();

        CanMove = false;
    }

    public void StopRetreating()
    {
        CanMove = true;

        if (_isInitialTargetDynamic)
        {
            SetTarget(_initialTarget);
        }
        else
        {
            SetTarget(_initialTargetPosition);
        }

        StartMoving();
    }

    private void OnDrawGizmos()
    {
        if (_path != null)
        {
            for (int i = _targetIndex; i < _path.Length; i++)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(_path[i], .25f);

                if (i == _targetIndex)
                {
                    Gizmos.DrawLine(transform.position, _path[i]);
                }
                else
                {
                    Gizmos.DrawLine(_path[i - 1], _path[i]);
                }
            }
        }

        /*
        Vector3 initialVelocity = new Vector3(5, -5, 0); // Initial velocity of the jump
        int numberOfWaypoints = 10; // Number of waypoints in the jump arc

        Vector2[] waypoints; // Array to store the waypoints
        float timeOfFlight; // Total time of the jump

        // Calculate the time of flight of the jump
        timeOfFlight = 2 * initialVelocity.y / 10;

        // Calculate the position of each waypoint in the jump arc
        waypoints = new Vector2[numberOfWaypoints];
        float timeInterval = timeOfFlight / numberOfWaypoints;
        for (int i = 0; i < numberOfWaypoints; i++)
        {
            float t = i * timeInterval;
            float x = initialVelocity.x * t;
            float y = initialVelocity.y * t - (0.5f * Physics.gravity.y * Mathf.Pow(t, 2));
            float z = initialVelocity.z * t;
            waypoints[i] = new Vector2(transform.position.x + x, transform.position.y + y);
        }

        foreach(Vector2 waypoint in waypoints)
        {
            Gizmos.DrawSphere(waypoint, .25f);
        }
        */
    }

    public bool GetTargetType()
    {
        return _isTargetDynamic;
    }

    public Transform GetTarget()
    {
        return _target;
    }

    public Vector2 GetTargetPosition()
    {
        return _targetPosition;
    }
}
