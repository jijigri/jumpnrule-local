using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinningSaw : MonoBehaviour
{
    [SerializeField] private float _damage = 30f;
    [SerializeField] private LayerMask _solidMask = default;
    [SerializeField] private LayerMask _hitMask = default;
    [SerializeField] private Animator _smallSawAnimator = null;
    [SerializeField] private Animator _bigSawAnimator = null;
    [SerializeField] private Material _defaultMaterial = null;
    [SerializeField] private Material _outlineMaterial = null;
    [SerializeField] private CircleCollider2D _collider = null;

    Vector2 _moveDirection;
    Vector2 _wallDirection;

    Vector2 _initialVelocity;

    float _speed = 6f;
    float _radius = .5f;

    bool _canChangeDirection = true;

    bool _isActive = false;

    Rigidbody2D _rigidBody;
    Animator _animator;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        Destroy(gameObject, 20f);
    }

    public void Initialize(float speed, float radius, float damage, Vector2 throwVelocity, float lifetime = 5)
    {
        _speed = speed;
        _radius = radius;
        _damage = damage;

        if(radius <= .5f)
        {
            _animator = _smallSawAnimator;
            _smallSawAnimator.gameObject.SetActive(true);
            _bigSawAnimator.gameObject.SetActive(false);
        }
        else
        {
            _animator = _bigSawAnimator;
            _bigSawAnimator.gameObject.SetActive(true);
            _smallSawAnimator.gameObject.SetActive(false);
        }
        _collider.radius = radius - .1f;
        _animator.GetComponent<SpriteRenderer>().material = _defaultMaterial;

        if (_rigidBody == null)
        {
            _rigidBody = GetComponent<Rigidbody2D>();
        }

        _rigidBody.AddForce(throwVelocity, ForceMode2D.Impulse);

        _initialVelocity = throwVelocity;

        Destroy(gameObject, lifetime);
    }

    private void FixedUpdate()
    {
        if (_isActive)
        {
            _rigidBody.velocity = _moveDirection * ((_speed * 100) * Time.fixedDeltaTime);
        }
    }

    private void Update()
    {
        if (!_isActive)
        {
            return;
        }

        if (_canChangeDirection)
        {
            float epsilon = .025f;

            RaycastHit2D forwardRay = Physics2D.Raycast((Vector2)transform.position + (_wallDirection * -epsilon), _moveDirection, _radius / 4, _solidMask);

            if (forwardRay)
            {
                GetComponent<Rigidbody2D>().position = forwardRay.point + (_wallDirection * -epsilon);

                //WALL UP OR DOWN
                if (_wallDirection.x == 0)
                {
                    Vector2 oldMoveDirection = _moveDirection;
                    _moveDirection = new Vector2(0, -_wallDirection.y);
                    _wallDirection = new Vector2(oldMoveDirection.x, 0);
                    StartCoroutine(DirectionChangeCooldown());
                }
                //WALL LEFT OR RIGHT
                else
                {
                    Vector2 oldMoveDirection = _moveDirection;
                    _moveDirection = new Vector2(-_wallDirection.x, 0);
                    _wallDirection = new Vector2(0, oldMoveDirection.y);
                    StartCoroutine(DirectionChangeCooldown());
                }
            }

            RaycastHit2D ray = Physics2D.Raycast((Vector2)transform.position + (_wallDirection * -epsilon), _wallDirection, _radius / 2, _solidMask);
            if (ray)
            {
                GetComponent<Rigidbody2D>().position = ray.point;
            }
            else
            {
                //WALL UP OR DOWN
                if (_wallDirection.x == 0)
                {
                    Vector2 oldMoveDirection = _moveDirection;
                    _moveDirection = new Vector2(0, _wallDirection.y);
                    _wallDirection = new Vector2(-oldMoveDirection.x, 0);

                    StartCoroutine(DirectionChangeCooldown());
                }
                //WALL LEFT OR RIGHT
                else
                {
                    Vector2 oldMoveDirection = _moveDirection;
                    _moveDirection = new Vector2(_wallDirection.x, 0);
                    _wallDirection = new Vector2(0, -oldMoveDirection.y);
                    StartCoroutine(DirectionChangeCooldown());
                }
            }
        }
    }

    IEnumerator DirectionChangeCooldown()
    {
        _canChangeDirection = false;
        yield return new WaitForSeconds(.05f);
        _canChangeDirection = true;

        yield break;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (_solidMask.Contains(collision.gameObject.layer))
        {
            if (!_isActive)
            {
                Vector2 contactPoint = collision.contacts[0].point;
                Vector2 directionToWall = contactPoint - (Vector2)transform.position;
                transform.position = contactPoint;
                gameObject.layer = 19;
                _rigidBody.gravityScale = 0;

                if (Mathf.Abs(directionToWall.x) > Mathf.Abs(directionToWall.y))
                {
                    //WALL LEFT OR RIGHT

                    _moveDirection = new Vector2(0, Mathf.Sign(_initialVelocity.y));
                    _wallDirection = new Vector2(Mathf.Sign(directionToWall.x), 0);
                }
                else
                {
                    //WALL UP OR DOWN

                    _moveDirection = new Vector2(Mathf.Sign(_initialVelocity.x), 0);
                    _wallDirection = new Vector2(0, Mathf.Sign(directionToWall.y));
                }

                if (_animator != null)
                {
                    _animator.SetTrigger("spin");
                    _animator.GetComponent<SpriteRenderer>().material = _outlineMaterial;
                }

                _isActive = true;
            }
        }
        else
        {
            if (_isActive)
            {
                if (collision.gameObject.TryGetComponent(out ICanBeDamaged iCanBeDamaged))
                {
                    iCanBeDamaged.DealDamage(_damage, gameObject, _moveDirection * 4, DamageType.DIRECT);
                    Destroy(gameObject);
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay((Vector2)transform.position + (_wallDirection * -.05f), _moveDirection * _radius);
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, _wallDirection * _radius * 2);
    }
}
