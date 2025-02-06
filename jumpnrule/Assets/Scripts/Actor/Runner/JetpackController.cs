using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JetpackController : MonoBehaviour
{
    [SerializeField] Rigidbody2D _rigidBody = null;
    [Header("Ground Settings")]
    [SerializeField] float _maxSpeed = 8;
    [SerializeField] float _groundAcceleration = 1f;
    [SerializeField] float _groundDeceleration = 1f;
    [SerializeField] float _airAcceleration = 1f;
    [SerializeField] float _airDeceleration = 1f;
    [Header("Jump Settings")]
    [SerializeField] float _jumpUpGravity = 10;
    [SerializeField] float _jumpAmount = 18;
    [SerializeField] float _minJumpVelocity = 6;
    [SerializeField] float _bufferedJumpTime = .1f;
    [SerializeField] float _coyoteJumpTime = .1f;
    [Header("Jetpack Settings")]
    [SerializeField] float _initialJetpackFuel = 30;
    [SerializeField] float _jetpackForce = 6;
    [SerializeField] float _jetpackAcceleration = 6;
    [SerializeField] LayerMask _solidMask = default; // Any layer the player can detect as the ground to reset his jump

    float _leftWidth;
    float _rightWidth;
    float _downHeight;
    float _epsilon;

    Vector3 _resetPosition;
    bool _resetPositionSet = false;

    bool _grounded;
    bool _isJumping; //Checks if the player is performing a jump, resets in the OnLand() method
    bool _canLand;
    float _xInput;

    float _bufferedJumpElapsedTime = 0;
    float _coyoteJumpElapsedTime = 0;

    float _currentJetpackFuel;

    float _initialGravity;

    float _velocityX;
    float _targetVelocityX;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();

        _leftWidth = 0.5f;
        _rightWidth = 0.5f;
        _downHeight = 0.5f;
        _epsilon = 0.05f;
    }

    void Start()
    {
        _initialGravity = _rigidBody.gravityScale;
        _currentJetpackFuel = _initialJetpackFuel;

        SetBufferedAndCoyoteJumpValues();
        InitLevel();
    }

    void SetBufferedAndCoyoteJumpValues()
    {
        _bufferedJumpElapsedTime = _bufferedJumpTime;
        _coyoteJumpElapsedTime = _coyoteJumpTime;
    }

    void Update()
    {
        _grounded = IsGrounded();
        CheckLand();
        ProcessJump();
        ProcessJetpack();
        ProcessHorizontalMovement();
    }

    #region Jump
    void ProcessJump()
    {
        //If the jump input is pressed
        if (Input.GetButtonDown("Jump"))
        {
            //If the player is grounded, or if the player fell from the platform a short time ago
            if (_grounded || CanCoyoteJump())
            {
                Jump();
                //Makes sure the player can't coyote jump after a jump
                _coyoteJumpElapsedTime = _coyoteJumpTime;
            }
            else
            {
                //If the player can't jump, resets the buffer jump timer to 0
                _bufferedJumpElapsedTime = 0;
            }
        }

        if (_grounded)
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
                    Jump(limitVelocity: true);
                }

                _bufferedJumpElapsedTime = _bufferedJumpTime;
            }

            //While the player is on the ground and not jumping, the coyote jump timer stays at 0
            if (!_isJumping)
            {
                _coyoteJumpElapsedTime = 0;
            }
        }
        else
        {
            //If the player is not on the ground and releases the jump key, limit his velocity so he falls quickier
            if (_rigidBody.velocity.y > 0 && _isJumping)
            {
                if (Input.GetButtonUp("Jump"))
                {
                    Jump(limitVelocity: true);
                }
            }

            //If the player is in the air, he can land
            _canLand = true;
        }

        //Changes gravity values depending on the current velocity
        if (_isJumping && _rigidBody.velocity.y > 0)
        {
            _rigidBody.gravityScale = _jumpUpGravity;
        }
        else
        {
            _rigidBody.gravityScale = _initialGravity;
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
    }

    void Jump(bool limitVelocity = false)
    {
        if (!limitVelocity)
        {
            _rigidBody.velocity = new Vector2(_rigidBody.velocity.x, _jumpAmount);
        }
        else
        {
            _rigidBody.velocity = new Vector2(_rigidBody.velocity.x, _minJumpVelocity);
        }

        _isJumping = true;
    }

    bool IsJumpBuffered()
    {
        return _bufferedJumpElapsedTime <= _bufferedJumpTime;
    }

    bool CanCoyoteJump()
    {
        return _coyoteJumpElapsedTime <= _coyoteJumpTime;
    }
    #endregion

    void ProcessJetpack()
    {
        if (Input.GetButton("Jump"))
        {
            if (!_grounded)
            {
                if (_currentJetpackFuel > 0)
                {
                    float newYVel = _rigidBody.velocity.y + (_jetpackAcceleration * Time.deltaTime);
                    newYVel = Mathf.Clamp(newYVel, -5, _jetpackForce);
                    _rigidBody.velocity = new Vector2(_rigidBody.velocity.x, newYVel);
                    _currentJetpackFuel -= Time.deltaTime;
                }

                Debug.Log("Current Jetpack Fuel: " + _currentJetpackFuel);
            }
        }
    }

    void ProcessHorizontalMovement()
    {
        _xInput = Input.GetAxisRaw("Horizontal");

        _targetVelocityX = _xInput * _maxSpeed;

        float t = 1;

        //Player is trying to move
        if (_xInput != 0)
        {
            //Ground acceleration
            if (_grounded)
            {
                t = _groundAcceleration;
            }
            //Air acceleration
            else
            {
                t = _airAcceleration;
            }
        }
        //Player is coming to a stop
        else
        {
            //Ground deceleration
            if (_grounded)
            {
                t = _groundDeceleration;
            }
            //Air deceleration
            else
            {
                t = _airDeceleration;
            }
        }

        _velocityX = Mathf.MoveTowards(_rigidBody.velocity.x, _targetVelocityX, t * Time.deltaTime);

        _rigidBody.velocity = new Vector2(_velocityX, _rigidBody.velocity.y);

        if (Mathf.Abs(_rigidBody.velocity.x) < 0.05f)
        {
            _velocityX = 0;
        }
    }

    private void FixedUpdate()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Player Collide: collision tag = " + collision.tag);

        if (collision.tag == "Finish")
        {
            Debug.Log("Collide: with Finish");
            LevelManager.Instance.EndLevel("Finish has been reached");
        }
    }

    private bool IsGrounded()
    {
        Vector2 myPosition = GetMyPosition();

        Vector2 leftRayOrigin = myPosition + Vector2.left * _leftWidth;
        Vector2 rightRayOrigin = myPosition + Vector2.right * _rightWidth;
        float rayLength = _downHeight + _epsilon;

        // Creates a raycast with rays going down from the left and right boundary of the player, and extend a bit over the bottom boundary the player
        // Returns true if and only if one of them touches "ground"
        bool doesPlayerTouchGround = Physics2D.Raycast(leftRayOrigin, Vector2.down, rayLength, _solidMask) || Physics2D.Raycast(rightRayOrigin, Vector2.down, rayLength, _solidMask);

        // Debugs a ray on the screen using the above rays
        Debug.DrawRay(leftRayOrigin, Vector2.down * rayLength, Color.blue);
        Debug.DrawRay(rightRayOrigin, Vector2.down * rayLength, Color.red);

        return doesPlayerTouchGround;
    }

    private void CheckLand()
    {
        if (_canLand && _grounded)
        {
            _canLand = false;
            OnLand();
        }
    }

    //Called on the frame where the player lands
    void OnLand()
    {
        _isJumping = false;
        _currentJetpackFuel = _initialJetpackFuel;
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
}
