using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour, IHasTriggerChild
{
    [SerializeField] protected float _radius = 3f;
    [SerializeField] protected float _magnetSpeed = 10f;
    [SerializeField] protected bool _isStatic = false;
    [SerializeField] CircleCollider2D _circleCollider;
    [SerializeField] Rigidbody2D _rigidBody;
    [SerializeField] private string _soundPath = "";
    [Tooltip("0 = energy, 1 = health, 2 = armor")][SerializeField] private int _playerGlintIndex = 0;

    EventInstance _soundEvent;

    protected GameObject _player;
    protected PlayerVFXManager _playerVfxManager;
    protected bool _isAttracted = false;

    protected virtual void Awake()
    {
        if (_soundPath != "")
        {
            _soundEvent = RuntimeManager.CreateInstance(_soundPath);
        }
    }

    protected virtual void Start()
    {
        if (_circleCollider != null)
        {
            _circleCollider.radius = _radius;
        }

        _rigidBody.bodyType = RigidbodyType2D.Dynamic;

        if(_player == null)
        {
            _player = GameObject.FindWithTag("Player");

            _playerVfxManager = _player.GetComponent<PlayerVFXManager>();
        }
    }

    protected virtual void OnEnable()
    {
        _isAttracted = false;

        if (!_isStatic)
        {
            _rigidBody.bodyType = RigidbodyType2D.Dynamic;
            _rigidBody.AddForce(Helper.GetRandomDirection() * 12, ForceMode2D.Impulse);
        }
        else
        {
            _rigidBody.bodyType = RigidbodyType2D.Static;
        }
    }

    protected virtual void Update()
    {
        if (_isAttracted)
        {
            if (_player != null)
            {
                _rigidBody.bodyType = RigidbodyType2D.Static;

                transform.position = Vector3.MoveTowards(transform.position, _player.transform.position, _magnetSpeed * Time.deltaTime);

                if (Vector2.Distance(transform.position, _player.transform.position) < .4f)
                {
                    OnPlayerTrigger();

                    if(_playerVfxManager != null)
                    {
                        _playerVfxManager.SetGlintEffect(_playerGlintIndex);
                    }

                    GameObject effect = ObjectPooler.Instance.SpawnObject("PickupObtainedEffect", transform.position, Quaternion.identity);
                    if(_soundPath != "")
                    {
                        _soundEvent.start();
                    }

                    gameObject.SetActive(false);
                }
            }
        }
    }

    protected virtual void OnPlayerTrigger()
    {

    }

    public void OnChildTriggerEnter(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (_player == null)
            {
                _player = collision.gameObject;
            }
            
            if(_playerVfxManager == null)
            {
                if(_player != null)
                {
                    _playerVfxManager = _player.GetComponent<PlayerVFXManager>();
                }
            }

            _isAttracted = true;
        }
    }
}
