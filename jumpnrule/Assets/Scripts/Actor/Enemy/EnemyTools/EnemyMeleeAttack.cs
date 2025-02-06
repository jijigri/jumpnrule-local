using FMOD.Studio;
using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMeleeAttack : MonoBehaviour, IHasEventReceiverChild
{
    [SerializeField] private float _attackDamage = 40f;
    [SerializeField] private float _distanceFromPlayerToAttack = 2f;
    [SerializeField] private float _speedWhileAttacking = 2f;
    [SerializeField] private float _attackChargeTime = 1;
    [SerializeField] private float _attackSuccessEndLag = .5f;
    [SerializeField] private float _attackFailEndLag = .5f;
    [SerializeField] private float _attackRange = .5f;
    [SerializeField] private float _attackHitboxRadius = .5f;
    [SerializeField] private LayerMask _hitMask = default;
    [SerializeField] private string _hitSound = "";

    bool _isAttacking = false;
    bool _isRequestingAttack = false;

    EventInstance _hitEvent;

    GameObject _player;

    ICanSetSpeed iCanChangeSpeed;
    EnemyAnimationManager _animationManager;

    private void Awake()
    {
        iCanChangeSpeed = GetComponent<ICanSetSpeed>();
        _animationManager = GetComponent<EnemyAnimationManager>();

        _hitEvent = RuntimeManager.CreateInstance(_hitSound);
    }

    private void Start()
    {
        _player = GameObject.FindWithTag("Player");
    }

    void Update()
    {
        if (!_isRequestingAttack)
        {
            if (!_isAttacking)
            {
                if (Vector2.Distance(transform.position, _player.transform.position) < _distanceFromPlayerToAttack)
                {
                    Action callback = new Action(delegate { StartAttacking(this); });
                    EnemyAttacksCooldownManager.Instance.RequestAttack(EnemyAttackType.MELEE, callback);

                    _isRequestingAttack = true;
                }
            }
        }
    }

    void StartAttacking(EnemyMeleeAttack caller)
    {
        if (caller != null)
        {
            _isRequestingAttack = false;

            if (!_isAttacking)
            {
                StopCoroutine("CRT_AttackSequence");
                StartCoroutine("CRT_AttackSequence");
            }
        }
        else
        {
            Debug.Log("FIXED THE BUG");
        }
    }

    IEnumerator CRT_AttackSequence()
    {
        Debug.Log("Trying to attack");

        _isAttacking = true;

        if (_animationManager != null)
        {
            _animationManager.Attack();
        }

        if (iCanChangeSpeed != null)
        {
            iCanChangeSpeed.SetSpeed(_speedWhileAttacking);
        }

        yield return new WaitForSeconds(_attackChargeTime);

        bool attackSuccess = AttackAction();

        if (attackSuccess)
        {
            yield return new WaitForSeconds(_attackSuccessEndLag);
        }
        else
        {
            yield return new WaitForSeconds(_attackFailEndLag);
        }

        if (iCanChangeSpeed != null)
        {
            iCanChangeSpeed.ResetSpeed();
        }

        _isAttacking = false;

        yield break;
    }

    bool AttackAction()
    {
        bool success = false;

        Vector2 hitDirection = new Vector2(Mathf.Sign(_player.transform.position.x - transform.position.x), 0);
        Vector2 hitPosition = (Vector2)transform.position + (hitDirection * _attackRange);

        Collider2D[] hit = Physics2D.OverlapCircleAll(hitPosition, _attackHitboxRadius, _hitMask);

        if (hit != null)
        {
            if (hit.Length > 0)
            {
                for (int i = 0; i < hit.Length; i++)
                {
                    Collider2D col = hit[i];

                    if (col.TryGetComponent(out ICanBeDamaged damage))
                    {
                        damage.DealDamage(_attackDamage, gameObject, hitDirection, DamageType.DIRECT);
                        success = true;
                    }
                }

                if(_hitSound != "")
                {
                    _hitEvent.start();
                }
            }
        }

        return success;
    }

    public void CancelAttack()
    {
        StopCoroutine("CRT_AttackSequence");
    }

    public void ReceiveEvent(int id)
    {
       
    }
}
