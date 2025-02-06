using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationManager : MonoBehaviour
{
    protected Animator _animator;
    SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    public virtual void SetDirection(int xVelocity)
    {
        if (xVelocity < 0)
        {
            _spriteRenderer.flipX = true;
        }
        else
        {
            _spriteRenderer.flipX = false;
        }
    }

    public virtual void AttackWarmup()
    {
        _animator.SetTrigger("attackWarmup");
    }

    public virtual void Attack()
    {
        _animator.SetTrigger("attack");
    }

    public virtual void SetGrounded(bool grounded)
    {
        _animator.SetBool("isGrounded", grounded);
    }
}
