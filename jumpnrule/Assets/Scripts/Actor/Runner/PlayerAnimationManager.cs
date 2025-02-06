using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationManager : MonoBehaviour, IHasEventReceiverChild
{
    [SerializeField] private LineRenderer _hand = null;
    [SerializeField] private Transform _armAnchor = null;
    [SerializeField] private SpriteRenderer _head = null;
    [SerializeField] private Transform _graphics = null;
    [SerializeField] private Animator _animator = null;

    int _lookDirection;
    bool _isLookDirectionLocked = false;

    bool _isRolling = false;

    float _animationSpeedMultiplier = 1f;

    List<Transform> _weaponSlots = new List<Transform>();

    Vector2 _velocity;
    public float _maxVelocityX { get; set; }

    ObjectPooler _pooler;

    private void Awake()
    {
        //_animator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        _weaponSlots = GetComponent<PlayerWeaponManager>().GetWeaponSlots();
        _pooler = ObjectPooler.Instance;
    }

    private void Update()
    {
        if(_hand != null)
        {
            _hand.SetPosition(0, _hand.transform.position);
            _hand.SetPosition(1, _armAnchor.transform.position);
        }

        if (_animator.GetCurrentAnimatorStateInfo(0).IsName("StarterCharacterDive"))
        {
            _isLookDirectionLocked = true;
        }
    }

    public void Rotate(float angle)
    {
       if (angle < -90 || angle > 90)
       {
            if (!_isLookDirectionLocked && !_isRolling)
            {
                _lookDirection = 1;
            }

        }
       else
       {
            if (!_isLookDirectionLocked && !_isRolling)
            {
                _lookDirection = -1;
            }
       }

        SetLookDirection(angle);
    }

    public void SetLookDirection(float angle)
    {
        if(_lookDirection == 1)
        {
            if(_graphics.localScale.x == -1)
            {
                Flip(1, angle);
            }

            _graphics.localScale = new Vector3(1, 1, 1);


            _head.flipX = false;
            _head.flipY = false;

            if (_velocity.x < 0)
            {
                _animator.SetFloat("TimeMultiplier", -1 * _animationSpeedMultiplier);
            }
            else
            {
                _animator.SetFloat("TimeMultiplier", 1 * _animationSpeedMultiplier);
            }


            foreach (Transform slot in _weaponSlots)
            {
                slot.transform.localScale = new Vector3(1, 1, 1);
            }
        }
        else
        {
            if(_graphics.localScale.x == 1)
            {
                Flip(-1, angle);
            }

            _graphics.localScale = new Vector3(-1, 1, 1);

            _head.flipX = true;
            _head.flipY = true;


            if (_velocity.x > 0)
            {
                _animator.SetFloat("TimeMultiplier", -1 * _animationSpeedMultiplier);
            }
            else
            {
                _animator.SetFloat("TimeMultiplier", 1 * _animationSpeedMultiplier);
            }

            foreach (Transform slot in _weaponSlots)
            {
                slot.transform.localScale = new Vector3(1, -1, 1);
            }
        }

        _head.transform.rotation = Quaternion.Euler(0, 0, angle + 180);
    }

    void Flip(int direction, float angle)
    {
        
    }

    public void SetVelocityX(float velocityX)
    {
        _animationSpeedMultiplier = Mathf.Abs(velocityX) / _maxVelocityX;

        _animationSpeedMultiplier = Mathf.Clamp(_animationSpeedMultiplier, .25f, 2f);

        _animator.SetFloat("VelocityX", Mathf.Abs(velocityX));
        _velocity.x = velocityX;
    }

    public void SetVelocityY(float velocityY)
    {
        _animator.SetFloat("VelocityY", velocityY);
        _velocity.y = velocityY;

        if(Mathf.Abs(_velocity.y) > .5f)
        {
            if (_isRolling)
            {
                StopCoroutine("RollCooldown");
                OnRollEnd();
            }
        }
    }

    public void OnJump()
    {
        if (_isRolling)
        {
            StopCoroutine("RollCooldown");
            OnRollEnd();
        }

        if (_pooler == null)
        {
            _pooler = ObjectPooler.Instance;
        }

        if(Mathf.Abs(_velocity.x) < 5f)
        {
            _pooler.SpawnObject("UpJumpEffect", (Vector2)transform.position + (Vector2.up * .5f), Quaternion.identity);
        }
        else
        {
            GameObject jumpEffect = _pooler.SpawnObject("ForwardJumpEffect", (Vector2)transform.position + (Vector2.up * .5f), Quaternion.identity);

            if(_velocity.x > 0)
            {
                jumpEffect.transform.localScale = new Vector3(-1, 1, 1);
            }
            else
            {
                jumpEffect.transform.localScale = new Vector3(1, 1, 1);
            }
        }
    }

    public void SetGroundedState(bool isGrounded)
    {
        _animator.SetBool("IsGrounded", isGrounded);
    }

    public void SetWallSlide(bool isWallSliding, int wallDirection)
    {
        _animator.SetBool("IsWallSliding", isWallSliding);

        if (isWallSliding)
        {
            _isLookDirectionLocked = true;

            _lookDirection = wallDirection;
        }
        else
        {
            _isLookDirectionLocked = false;

            //_lookDirection = wallDirection;
        }
    }

    public void Roll(float inputDirection, float rollTime)
    {
        _animator.SetTrigger("Roll");

        _isRolling = true;
        _lookDirection = (int)Mathf.Sign(inputDirection);

        _armAnchor.gameObject.SetActive(false);

        StopCoroutine("RollCooldown");
        StartCoroutine(RollCooldown(rollTime));
    }

    IEnumerator RollCooldown(float rollTime)
    {
        yield return new WaitForSeconds(rollTime);

        OnRollEnd();

        yield break;
    }

    void OnRollEnd()
    {
        _isRolling = false;

        _armAnchor.gameObject.SetActive(true);
    }

    public void Dive(int inputDirection)
    {
        _animator.SetTrigger("Dive");

        _lookDirection = (int)Mathf.Sign(inputDirection);
    }

    public void ReceiveEvent(int id)
    {
        if(id == 0)
        {
            if(_pooler == null)
            {
                _pooler = ObjectPooler.Instance;
            }

            GameObject part = _pooler.SpawnObject("RunParticles", transform.position, Quaternion.identity);

            if(_velocity.x < 0)
            {
                part.transform.localScale = new Vector3(-1, 1, 1);
            }
            else
            {
                part.transform.localScale = new Vector3(1, 1, 1);
            }
        }
    }
}
