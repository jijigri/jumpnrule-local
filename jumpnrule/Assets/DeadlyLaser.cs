using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadlyLaser : MonoBehaviour
{
    [SerializeField] private LayerMask _hitMask;
    [SerializeField] private float _prepareTime = 2f;
    [SerializeField] private float _lifeTime = 2f;
    [SerializeField] private AnimationClip _prepareAnimation = null;
    [SerializeField] private ParticleSystem _particles = null;

    Vector2 size = Vector2.zero;

    bool _isActive = false;

    SpriteRenderer _spriteRenderer;
    Animator _spriteAnimator;

    private void Awake()
    {
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _spriteAnimator = _spriteRenderer.gameObject.GetComponent<Animator>();
    }

    private void OnEnable()
    {
        _isActive = false;
        _spriteRenderer.gameObject.SetActive(true);

        Initialize();
    }

    void Initialize()
    {
        size = new Vector2(150, 1);
        _spriteRenderer.size = size;

        _particles.Stop();
        var shape = _particles.shape;
        shape.scale = (size / 2) - (Vector2.one * .1f);

        StartCoroutine(CRT_PrepareLaser());
    }

    IEnumerator CRT_PrepareLaser()
    {
        float time = _prepareAnimation.length;
        float speedModifier = time / _prepareTime;
        _spriteAnimator.SetFloat("SpeedModifier", speedModifier);

        yield return new WaitForSeconds(_prepareTime);

        ActivateLaser();

        yield return new WaitForSeconds(.1f);

        Shoot();

        yield return new WaitForSeconds(_lifeTime);

        DisableLaser();
    }

    void ActivateLaser()
    {
        _spriteAnimator.SetTrigger("Shoot");

        _particles.Play();
    }

    void Shoot()
    {
        _isActive = true;
    }

    void Update()
    {
        if (!_isActive)
        {
            return;
        }

        Collider2D hit = Physics2D.OverlapBox(transform.position, size, 0, _hitMask);

        if (hit)
        {
            if(hit.TryGetComponent(out ICanBeDamaged canBeDamaged))
            {
                canBeDamaged.DealDamage(20, gameObject, Vector2.zero, DamageType.AOE);

                DisableLaser();
            }
        }
    }

    void DisableLaser()
    {
        _isActive = false;

        _spriteRenderer.gameObject.SetActive(false);
        _particles.Stop();
        Invoke("DisableAction", 1f);
    }

    void DisableAction()
    {
        gameObject.SetActive(false);
    }
}
