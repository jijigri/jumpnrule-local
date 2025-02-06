using System.Collections;
using UnityEngine;

public class PlatformController : MonoBehaviour, ICanIncreaseSpeed
{
    protected enum PlayerState
    {
        GROUNDED,
        AIRBORNE,
        WALL_SLIDING
    }

    [Header("Ground Settings")]
    [SerializeField] protected float _defaultSpeed = 17f;
    [SerializeField] protected float _maxSpeed = 22f;
    [SerializeField] protected float _groundAcceleration = 1.8f;
    [SerializeField] protected float _groundFriction = 2.6f;
    [SerializeField] protected float _airAcceleration = 1.5f;
    [SerializeField] protected float _airFriction = 0f;

    [Space(10)]
    [Header("Jump Settings")]
    [SerializeField] protected float _jumpAmount = 30;

    [Space(10)]
    [SerializeField] protected float _jumpFallGravityModifier = 1.4f;
    [SerializeField] protected float _jumpApexTreshold = 1.5f;
    [Range(0, 1)]
    [SerializeField] protected float _jumpApexGravityMultiplier = .6f;

    [Space(10)]
    [Range(0, 1)]
    [SerializeField] protected float _bufferedJumpTime = .25f;
    [Range(0, 1)]
    [SerializeField] protected float _coyoteJumpTime = .1f;
    [SerializeField] protected float _maxFallVelocity = 25f;

    [Space(10)]
    [SerializeField] protected LayerMask _solidMask = default; // Any layer the player can detect as the ground to reset his jump

    [Space(10)]
    [Header("Wall Jump Settings")]
    [SerializeField] protected float _wallSlidingGravity = 2f;
    [SerializeField] protected float _maxWallSlidingFallSpeed = 5f;
    [SerializeField] protected Vector2 _wallJumpVelocity = new Vector2(16, 28);
    [SerializeField] protected float _airControlAfterWallJump = 1.5f;
    [SerializeField] protected float _airControlRegainSpeedAfterWallJump = 2f;
    [Range(0, 1)]
    [SerializeField] protected float _wallJumpCoyoteTime = .1f;

    [Space(10)]
    [Header("Debug")]
    [SerializeField] protected float _currentSpeed = 18;

    float _leftWidth;
    float _rightWidth;
    float _downHeight;
    float _epsilon;

    Vector3 _resetPosition;
    bool _resetPositionSet = false;

    //protected bool _grounded;
    protected bool _isJumping; //Checks if the player is performing a jump, resets in the OnLand() method
    protected PlayerState _playerState = PlayerState.AIRBORNE;
    bool _canLand;
    protected float _xInput;
    protected int _faceDirection = 1;

    float _bufferedJumpElapsedTime = 0;
    float _coyoteJumpElapsedTime = 0;
    float _wallJumpCoyoteElapsedTime = 0;

    float _initialAirAcceleration = 5f;

    protected float _initialGravity;

    protected int _lastWallDirection = -1;

    protected float _velocityX;
    protected float _lastVelocityX;

    protected Rigidbody2D _rigidBody = null;
    protected BoxCollider2D _boxCollider;
    SpriteRenderer _spriteRenderer;
    protected PlayerAnimationManager _animationManager;
    PlayerAudioManager _audioManager;

    public virtual void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _boxCollider = GetComponent<BoxCollider2D>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _animationManager = GetComponent<PlayerAnimationManager>();
        _audioManager = GetComponent<PlayerAudioManager>();

        _currentSpeed = _defaultSpeed;

        _leftWidth = _boxCollider.bounds.size.x / 2;
        _rightWidth = _boxCollider.bounds.size.x / 2;
        _downHeight = _boxCollider.bounds.size.y / 2;
        _epsilon = 0.05f;
    }

    public virtual void Start()
    {
        _animationManager._maxVelocityX = _currentSpeed;
        _initialGravity = _rigidBody.gravityScale;
        _initialAirAcceleration = _airAcceleration;
        SetBufferedAndCoyoteJumpValues();
        InitLevel();
    }

    void SetBufferedAndCoyoteJumpValues()
    {
        _bufferedJumpElapsedTime = _bufferedJumpTime;
        _coyoteJumpElapsedTime = _coyoteJumpTime;
    }

    public void GiveSpeed(float speedAmount, GameObject source)
    {
        _currentSpeed += speedAmount;

        if(_currentSpeed > _maxSpeed)
        {
            _currentSpeed = _maxSpeed;
        }
    }

    public void RemoveSpeed(float speedAmount, GameObject source)
    {
        _currentSpeed -= speedAmount;

        if(_currentSpeed < _defaultSpeed)
        {
            _currentSpeed = _defaultSpeed;
        }
    }

    public virtual void Update()
    {
        _velocityX = _rigidBody.velocity.x;

        _faceDirection = GetFaceDirection();

        _playerState = GetPlayerState();

        if (_playerState == PlayerState.WALL_SLIDING)
        {
            ProcessWallSliding();
        }
        else
        {
            if (_audioManager != null)
            {
                _audioManager.StopWallSlide();
            }
        }

        CheckLand();

        GetInput();
        ProcessHorizontalMovement();
        //ApplyFriction();
        //_rigidBody.velocity = new Vector2(_velocityX, _rigidBody.velocity.y);

        ProcessJump();

        /*
        if(Input.GetAxisRaw("Vertical") < 0 && _rigidBody.velocity.y < 0)
        {
            _rigidBody.gravityScale = _initialGravity * 3f;
        }
        */

        if(_rigidBody.velocity.y < -_maxFallVelocity)
        {
            _rigidBody.velocity = new Vector2(_rigidBody.velocity.x, -_maxFallVelocity);
        }

        if(Mathf.Abs(_rigidBody.velocity.x) > .1f)
        {
            _lastVelocityX = _rigidBody.velocity.x;
        }

        if(_currentSpeed > _defaultSpeed)
        {
            _currentSpeed -= Time.deltaTime * 0.5f;

            if(_currentSpeed < _defaultSpeed)
            {
                _currentSpeed = _defaultSpeed;
            }
        }
    }

    public virtual void FixedUpdate()
    {
        
    }

    public virtual void ProcessHorizontalMovement()
    {
        float targetSpeed = _xInput * _currentSpeed;
        //targetSpeed = Mathf.Lerp(RB.velocity.x, targetSpeed, lerpAmount);

        float accelRate;

        //Gets an acceleration value based on if we are accelerating (includes turning) 
        //or trying to decelerate (stop). As well as applying a multiplier if we're air borne.
        if (_playerState == PlayerState.GROUNDED)
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? _groundAcceleration : _groundFriction;
        else
        {
            if(Mathf.Abs(targetSpeed) > 0.01f)
            {
                accelRate = _airAcceleration;
            }
            else
            {
                accelRate = Mathf.Abs(_rigidBody.velocity.x) > _currentSpeed / 2 ? _airFriction : _groundFriction / 2;
            }
        }

        //Calculate difference between current velocity and desired velocity
        float speedDif = targetSpeed - _rigidBody.velocity.x;
        //Calculate force along x-axis to apply to thr player

        float movement = speedDif * accelRate;

        //Convert this to a vector and apply to rigidbody
        _rigidBody.AddForce(movement * Vector2.right, ForceMode2D.Force);
    }


    int GetFaceDirection()
    {
        if(Input.GetAxisRaw("Horizontal") > 0.1f)
        {
            return 1;
        }
        else if (Input.GetAxisRaw("Horizontal") < -0.1f)
        {
            return -1;
        }
        else
        {
            return _faceDirection;
        }
    }

    #region Jump
    void ProcessJump()
    {
        //Changes gravity values depending on the current velocity
        if(Mathf.Abs(_rigidBody.velocity.y) < _jumpApexTreshold)
        {
            _rigidBody.gravityScale = _initialGravity * _jumpApexGravityMultiplier;
        }
        else if (_rigidBody.velocity.y > 0)
        {
            _rigidBody.gravityScale = _initialGravity;
        }
        else if (_rigidBody.velocity.y <= 0)
        {
            _rigidBody.gravityScale = _initialGravity * _jumpFallGravityModifier;
        }

        //If the jump input is pressed
        if (Input.GetButtonDown("Jump"))
        {
            //If the player is grounded, or if the player fell from the platform a short time ago
            if ((_playerState == PlayerState.GROUNDED || _playerState == PlayerState.WALL_SLIDING) || (CanCoyoteJump() || CanWallCoyoteJump()))
            {
                Jump(CanWallCoyoteJump());
                //Makes sure the player can't coyote jump after a jump
                _coyoteJumpElapsedTime = _coyoteJumpTime;
                _wallJumpCoyoteElapsedTime = _wallJumpCoyoteTime;
            }
            else
            {
                //If the player can't jump, resets the buffer jump timer to 0
                _bufferedJumpElapsedTime = 0;
            }
        }

        if (_playerState == PlayerState.GROUNDED || _playerState == PlayerState.WALL_SLIDING)
        {
            //Check if a jump was buffered
            if (IsJumpBuffered())
            {
                //If the player is still holding the jump key, do a normal jump
                if (Input.GetButton("Jump"))
                {
                    Jump();
                }
                //If the player is not holding the jump key, do the shortest jump possible
                else
                {
                    Jump();
                    CutJump();
                }

                _bufferedJumpElapsedTime = _bufferedJumpTime;
            }

            //While the player is on the ground and not jumping, the coyote jump timer stays at 0
            if (!_isJumping)
            {
                _coyoteJumpElapsedTime = 0;
            }
            if(_playerState == PlayerState.WALL_SLIDING)
            {
                _wallJumpCoyoteElapsedTime = 0;
            }
            else if(_playerState == PlayerState.GROUNDED)
            {
                _wallJumpCoyoteElapsedTime = _wallJumpCoyoteTime;
            }
        }
        else
        {
            //If the player is not on the ground and releases the jump key, limit his velocity so he falls quickier
            if (_isJumping)
            {
                if (Input.GetButtonUp("Jump"))
                {
                    CutJump();
                }
            }

            //If the player is in the air, he can land
            _canLand = true;
        }

        //Increases timer for buffer jumps and coyote jumps     
        if (_bufferedJumpElapsedTime <= _bufferedJumpTime)
        {
            _bufferedJumpElapsedTime += Time.deltaTime;
        }

        if (_coyoteJumpElapsedTime <= _coyoteJumpTime)
        {
            _coyoteJumpElapsedTime += Time.deltaTime;
        }

        if(_wallJumpCoyoteElapsedTime <= _wallJumpCoyoteTime)
        {
            _wallJumpCoyoteElapsedTime += Time.deltaTime;
        }
    }

    void CutJump()
    {
        Debug.Log("Cut jump");
        _rigidBody.velocity = new Vector2(_rigidBody.velocity.x, _rigidBody.velocity.y / 2f);
    }

    public virtual void Jump(bool canWallCoyote = false)
    {
        Debug.Log("Jump");
        Vector2 jumpVelocity = Vector2.zero;

        if (_playerState == PlayerState.WALL_SLIDING || CanWallCoyoteJump())
        {
            float direction = _playerState == PlayerState.WALL_SLIDING ? (Mathf.Sign(_lastVelocityX) * -1) : (Mathf.Sign(_lastVelocityX) * 1);
            jumpVelocity = new Vector2(direction * _wallJumpVelocity.x, _wallJumpVelocity.y);
            StartCoroutine(CRT_RegainAirControl());
        }
        else
        {
            if (canWallCoyote == false)
            {
                jumpVelocity = new Vector2(_rigidBody.velocity.x, _jumpAmount);

                if (_animationManager != null)
                {
                    _animationManager.OnJump();
                }
            }
        }

        _rigidBody.velocity = jumpVelocity;

        _isJumping = true;


        if(_audioManager != null)
        {
            _audioManager.PlayJump();
        }
    }

    IEnumerator CRT_RegainAirControl()
    {
        float t = 0;
        while(t < 1)
        {
            _airAcceleration = Mathf.Lerp(_airControlAfterWallJump, _initialAirAcceleration, t);
            t += Time.deltaTime * _airControlRegainSpeedAfterWallJump;

            yield return null;
        }

        yield break;
    }

    public virtual void ProcessWallSliding()
    {
        float yVel = _rigidBody.velocity.y;

        if(yVel >= 0)
        {
            _rigidBody.gravityScale = _initialGravity;
        }
        else
        {
            _rigidBody.gravityScale = _wallSlidingGravity;
        }

        yVel = Mathf.Clamp(yVel, -_maxWallSlidingFallSpeed, 500f);

        _rigidBody.velocity = new Vector2(_rigidBody.velocity.x, yVel);

        if (_audioManager != null)
        {
            _audioManager.PlayWallSlide();
        }
    }

    bool IsJumpBuffered()
    {
        return _bufferedJumpElapsedTime <= _bufferedJumpTime;
    }

    bool CanCoyoteJump()
    {
        return _coyoteJumpElapsedTime <= _coyoteJumpTime;
    }

    bool CanWallCoyoteJump()
    {
        return _wallJumpCoyoteElapsedTime <= _wallJumpCoyoteTime;
    }
    #endregion

    protected virtual void GetInput()
    {
        _xInput = Input.GetAxisRaw("Horizontal");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Finish")
        {
            Debug.Log("Collide: with Finish");
            LevelManager.Instance.EndLevel("Finish has been reached");
        }
    }

    private PlayerState GetPlayerState()
    {
        Vector2 myPosition = GetMyPosition();

        Vector2 leftRayOrigin = myPosition + Vector2.left * _leftWidth;
        Vector2 rightRayOrigin = myPosition + Vector2.right * _rightWidth;
        float rayLength = _downHeight + _epsilon;

        // Creates a raycast with rays going down from the left and right boundary of the player, and extend a bit over the bottom boundary the player
        // Returns true if and only if one of them touches "ground"
        bool doesPlayerTouchGround = Physics2D.Raycast(leftRayOrigin, Vector2.down, rayLength, _solidMask) || Physics2D.Raycast(rightRayOrigin, Vector2.down, rayLength, _solidMask);

        if(_rigidBody.velocity.y > .5f)
        {
            doesPlayerTouchGround = false;
        }

        // Debugs a ray on the screen using the above rays
        Debug.DrawRay(leftRayOrigin, Vector2.down * rayLength, Color.blue);
        Debug.DrawRay(rightRayOrigin, Vector2.down * rayLength, Color.red);

        if (doesPlayerTouchGround)
        {
            return PlayerState.GROUNDED;
        }
        else
        {
            bool isWallSliding = IsWallSliding();

            if (isWallSliding && Input.GetAxisRaw("Vertical") >= 0)
            {
                _lastWallDirection = (int)Mathf.Sign(_lastVelocityX) * -1;
                return PlayerState.WALL_SLIDING;
            }
            else
            {
                return PlayerState.AIRBORNE;
            }
        }
    }

    bool IsWallSliding()
    {
        bool isWallSliding = false;

        float length = ((_leftWidth + _epsilon) * Mathf.Sign(_lastVelocityX));

        int numberOfRays = 5;
        numberOfRays = Mathf.Clamp(numberOfRays, 1, 20);

        Vector2 rayOrigin = (Vector2)transform.position - new Vector2(0, _downHeight);
        float raySpacing = (_downHeight * 2) / (numberOfRays - 1);

        for(int i = 0; i < numberOfRays; i++)
        {
            Debug.DrawRay(rayOrigin, new Vector2(length, 0), Color.red);
            bool isTouchingWall = Physics2D.Raycast(rayOrigin, new Vector2(1, 0), length, _solidMask);
            if (isTouchingWall)
            {
                isWallSliding = true;
            }

            rayOrigin.y += raySpacing;
        }

        return isWallSliding;
    }

    private void CheckLand()
    {
        if (_canLand && _playerState == PlayerState.GROUNDED)
        {
            _canLand = false;
            OnLand();
        }
    }

    //Called on the frame where the player lands
    public virtual void OnLand()
    {
        _isJumping = false;
        StopCoroutine(CRT_RegainAirControl());
        _airAcceleration = _initialAirAcceleration;

        if (_audioManager != null)
        {
            _audioManager.PlayLand();
        }
    }

    private Vector2 GetMyPosition()
    {
        return transform.position;
    }

    // Should be called once, when the level is loaded
    private void InitLevel()
    {
        _resetPosition = GetMyPosition();
        _resetPositionSet = true;
    }

    // (Re)starts the run
    public void StartLevel()
    {
        if (_resetPositionSet)
        {
            transform.position = _resetPosition;
        }
        _rigidBody.velocity = Vector2.zero;
    }

    private void LateUpdate()
    {
        if(_animationManager != null)
        {
            _animationManager.SetVelocityX(_rigidBody.velocity.x);
            _animationManager.SetVelocityY(_rigidBody.velocity.y);
            _animationManager.SetGroundedState(_playerState == PlayerState.GROUNDED);
            _animationManager.SetWallSlide(_playerState == PlayerState.WALL_SLIDING, _lastWallDirection);
        }
    }

    public void OverrideVelocity(Vector2 velocity)
    {
        _rigidBody.velocity = velocity;
        _isJumping = false;
        StopCoroutine(CRT_RegainAirControl());
        _airAcceleration = _initialAirAcceleration;
    }

    public Vector2 GetVelocity()
    {
        return _rigidBody.velocity;
    }
}
