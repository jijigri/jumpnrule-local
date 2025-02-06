using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SniperEnemyBehavior : Enemy
{
    [SerializeField] private float _aimTime = 2f;
    [SerializeField] private float _hitRange = 20f;
    [SerializeField] private float _hitDamage = 20f;
    [SerializeField] private float _minInitialDelay = 4f;
    [SerializeField] private float _maxInitialDelay = 6f;
    [SerializeField] private float _shotsDelay = 4f;
    [SerializeField] private LineRenderer _lineRenderer = null;
    [SerializeField] private float _lineLenghtMultiplier = .2f;
    [SerializeField] private SpriteRenderer _hitSprite = null;
    [SerializeField] private Animator _hitAnimator = null;
    [SerializeField] private Transform _shootPoint = null;
    [SerializeField] private LayerMask _hitMask = default;

    Coroutine _shootCrt;

    protected override void Start()
    {
        base.Start();

        _lineRenderer.enabled = false;

        float delay = Random.Range(_minInitialDelay, _maxInitialDelay);
        Invoke("StartAiming", delay);
    }

    void StartAiming()
    {
        _shootCrt = StartCoroutine(CRT_Shoot());
    }

    IEnumerator CRT_Shoot()
    {
        _lineRenderer.enabled = true;

        float t = 0;
        _aimTime = Mathf.Clamp(_aimTime, .1f, float.PositiveInfinity);
        while(t < _aimTime)
        {
            _lineRenderer.SetPosition(0, _shootPoint.position);
            _lineRenderer.SetPosition(1, _player.transform.position);

            float length = ((t / _aimTime) - 1) / -1;
            _lineRenderer.startWidth = _lineRenderer.endWidth = length * _lineLenghtMultiplier;

            t += Time.deltaTime;

            yield return null;
        }

        _lineRenderer.enabled = false;

        Vector2 direction = _player.transform.position - transform.position;
        direction.Normalize();

        float angle = Helper.AngleBetweenTwoPoints(_player.transform.position, transform.position);

        yield return new WaitForSeconds(.2f);

        if (_isFaltered == false)
        {
            Shoot(direction, angle);
        }
        yield break;
    }

    void Shoot(Vector2 direction, float angle)
    {
        _hitSprite.transform.SetParent(_shootPoint);

        _shootPoint.rotation = Quaternion.Euler(0, 0, angle - 90f);
        _hitSprite.transform.position = (Vector2)_shootPoint.transform.position;
        _hitSprite.size = new Vector2(2, _hitRange);

        _hitAnimator.SetTrigger("shoot");
        _hitSprite.transform.SetParent(null);

        RaycastHit2D hit = Physics2D.Raycast(_shootPoint.position, direction, _hitRange, _hitMask);

        if (hit)
        {
            OnHit(hit, direction);
        }

        Invoke("StartAiming", 1.5f);
    }

    void OnHit(RaycastHit2D hit, Vector2 direction)
    {
        if (!hit.collider.CompareTag("KillableEnemy"))
        {
            if (hit.collider.TryGetComponent(out ICanBeDamaged iDamageable))
            {
                iDamageable.DealDamage(_hitDamage, gameObject, direction);
            }
        }
        _hitSprite.size = new Vector2(2, hit.distance);
    }

    protected override void OnFalterStart()
    {
        base.OnFalterStart();
        CancelInvoke("StartAiming");

        if (_shootCrt != null)
        {
            StopCoroutine(_shootCrt);
        }

        _lineRenderer.enabled = false;
    }

    protected override void OnFalterStop()
    {
        base.OnFalterStop();
        Invoke("StartAiming", 1f);
    }
}
