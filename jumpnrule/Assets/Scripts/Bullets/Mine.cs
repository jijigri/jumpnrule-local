using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD;
using FMOD.Studio;

public class Mine : MonoBehaviour, IHasPlayerOwner, IHasGunOwner
{
    [SerializeField] private Rigidbody2D _rigidBody = null;
    [SerializeField] private BoxCollider2D _boxCollider = null;
    [SerializeField] private SpriteRenderer _sprite = null;
    [SerializeField] private Animator _animator = null;
    [SerializeField] private LayerMask _solidMask;
    [SerializeField] private LayerMask _hitMask;
    [SerializeField] private float _timeToArm = .5f;
    [SerializeField] private float _lifeTime = 8f;
    [SerializeField] private string _readySound = "";

    private float _damage = 10;
    private Vector2 _launchVelocity = Vector2.up;

    bool _isOnGround = false;
    bool _canExplode = false;

    GameObject _playerOwner;
    Gun _gunOwner;

    float _timeLeftToBeArmed = 0f;

    EventInstance _readyEvent;

    RocketLauncher _launcher;

    public void SetPlayerOwner(GameObject playerOwner)
    {
        _playerOwner = playerOwner;
    }

    public GameObject GetPlayerOwner()
    {
        return _playerOwner;
    }
    public void SetGunOwner(Gun gunOwner)
    {
        _gunOwner = gunOwner;
    }

    public Gun GetGunOwner()
    {
        return _gunOwner;
    }

    private void Awake()
    {
        _readyEvent = RuntimeManager.CreateInstance(_readySound);
    }

    public void InitializeMine(Vector2 throwVelocity, float damage, Vector2 launchVelocity, RocketLauncher launcher = null)
    {
        _isOnGround = false;
        _rigidBody.bodyType = RigidbodyType2D.Dynamic;
        _sprite.transform.rotation = Quaternion.identity;

        _timeLeftToBeArmed = _timeToArm;

        _rigidBody.AddForce(throwVelocity, ForceMode2D.Impulse);
        _damage = damage;
        _launchVelocity = launchVelocity;

        if(launcher != null)
        {
            _launcher = launcher;
        }
        
        if(_animator != null)
        {
            _animator.SetTrigger("reset");
        }

        Invoke("ExplodeMine", _lifeTime);
    }

    private void Update()
    {
        if (_timeLeftToBeArmed > 0)
        {
            _timeLeftToBeArmed -= Time.deltaTime;
            _canExplode = false;
        }
        else
        {
            if(_canExplode == false)
            {
                OnReady();
            }
            _canExplode = true;
        }
    }

    void FixedUpdate()
    {
        if(_isOnGround == false)
        {
            _sprite.transform.Rotate(new Vector3(0, 0, (500 * Time.fixedDeltaTime)));

            float rayLength = .25f;
            Vector2 raycastPosition = new Vector2(transform.position.x, transform.position.y - (_boxCollider.size.y / 2) + rayLength);
            bool isGrounded = Physics2D.Raycast(raycastPosition, Vector2.down, (rayLength + 0.15f), _solidMask);

            if (isGrounded)
            {
                _rigidBody.bodyType = RigidbodyType2D.Static;
                _rigidBody.velocity = Vector2.zero;
                _isOnGround = true;

                _sprite.transform.rotation = Quaternion.Euler(0, 0, 0);

                _timeLeftToBeArmed = 0;

                //_boxCollider.enabled = false;
            }
        }

        if (_canExplode)
        {
            DetectCollisions();

            if (_animator != null)
            {
                _animator.SetTrigger("ready");
            }
        }
    }

    private void DetectCollisions()
    {
        Collider2D hit = Physics2D.OverlapBox(transform.position, _boxCollider.size, 0, _hitMask);

        if (hit)
        {
            OnCollision(hit);
        }
    }

    private void OnCollision(Collider2D hit)
    {
        if (!hit.gameObject.CompareTag("Player"))
        {
            ICanBeDamaged damage = hit.gameObject.GetComponent<ICanBeDamaged>();

            if (damage != null)
            {
                damage.DealDamage(_damage, gameObject, _launchVelocity);

                ExplodeMine();
            }
            else if(hit.gameObject != this.gameObject)
            {
                ExplodeMine();
            }
        }
        else
        {
            PlatformController controller = hit.gameObject.GetComponent<PlatformController>();

            if (controller != null)
            {
                Vector2 oldVelocity = controller.GetVelocity();
                oldVelocity.y = Mathf.Clamp(oldVelocity.y, 0, Mathf.Infinity);
                controller.OverrideVelocity((oldVelocity / 3) + _launchVelocity);
                ExplodeMine();
            }
        }
    }

    void OnReady()
    {
        RuntimeManager.AttachInstanceToGameObject(_readyEvent, transform);
        _readyEvent.start();
    }

    public void ExplodeMine()
    {
        CancelInvoke();

        _timeLeftToBeArmed = _timeToArm;

        ObjectPooler.Instance.SpawnObject("MineExplosion", transform.position, Quaternion.identity);

        _launcher.RemoveMine(this);

        gameObject.SetActive(false);
    }
}
