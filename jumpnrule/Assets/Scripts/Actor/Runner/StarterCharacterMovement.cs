using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarterCharacterMovement : PlatformController
{
    [Header("Character Specific Settings")]
    [SerializeField] private float _rollSpeed = 5f;
    [SerializeField] private float _rollInvulnerabilityTime = .5f;
    [SerializeField] private float _rollCooldown = 2f;
    [SerializeField] private Vector2 _airRollVelocity = Vector2.zero;
    //[SerializeField] private float _rollFriction = 200;

    bool _isRolling = false;
    bool _isDiving = false;
    bool _canDive = true;

    float _timeBeforeNextRoll = 0;

    PlayerHealthManager _healthManager;

    public override void Awake()
    {
        base.Awake();
        _healthManager = GetComponent<PlayerHealthManager>();
    }

    public override void Update()
    {
        base.Update();

        if (Input.GetButtonDown("Dodge"))
        {
            if(_playerState == PlayerState.AIRBORNE)
            {
                Dive();
            }
            else if (_playerState == PlayerState.GROUNDED)
            {
                Roll();
            }
        }

        if(_timeBeforeNextRoll > 0)
        {
            _timeBeforeNextRoll -= Time.deltaTime;
        }
    }

    void Dive()
    {
        if (_canDive)
        {
            float inputDirection = _xInput != 0 ? _faceDirection : Mathf.Sign(_lastVelocityX);
            _rigidBody.velocity = new Vector2(_rigidBody.velocity.x + (_airRollVelocity.x * inputDirection), _airRollVelocity.y);
            _isJumping = false;
            _isDiving = true;

            _animationManager.Dive((int)inputDirection);

            _canDive = false;
        }
    }

    void Roll()
    {
        if (_timeBeforeNextRoll <= 0)
        {
            GameEvents.Instance.PlayerRoll(this);

            float inputDirection = _xInput != 0 ? _faceDirection : Mathf.Sign(_lastVelocityX);
            _rigidBody.velocity = new Vector2(_rollSpeed * inputDirection, 0);
            _timeBeforeNextRoll = _rollCooldown;

            _healthManager.SetInvulnerability(_rollInvulnerabilityTime);
            _animationManager.Roll(inputDirection, _rollInvulnerabilityTime);
        }
    }

    public override void Jump(bool canCoyoteJump = false)
    {
        _isRolling = false;
        _isDiving = false;
        base.Jump();
    }

    
    public override void OnLand()
    {
        base.OnLand();

        if (_isDiving)
        {
            Roll();
            _isDiving = false;
        }

        _canDive = true;
    }

    public override void ProcessWallSliding()
    {
        if (!_isRolling)
        {
            base.ProcessWallSliding();
        }
    }

    public override void ProcessHorizontalMovement()
    {
        if (!_isRolling)
        {
            base.ProcessHorizontalMovement();
        }
    }

    public float GetMaxCooldown()
    {
        return _rollCooldown;
    }

    public float GetCurrentCooldown()
    {
        return _timeBeforeNextRoll;
    }
}
