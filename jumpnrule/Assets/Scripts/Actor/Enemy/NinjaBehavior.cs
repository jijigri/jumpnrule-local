using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NinjaBehavior : ShooterEnemy
{
    [Header("Ninja Fields")]
    [SerializeField] private float _maxInvisibilityTime = 4;
    [SerializeField] private float _moveSpeed = 4;
    [SerializeField] private float _wallCheckDistance = 4;
    [SerializeField] private LayerMask _wallMask = default;

    bool _isInvisible = false;
    bool _isPreparingAttack = false;

    protected override void Start()
    {
        base.Start();

        StartShooting(1f);
    }

    void FixedUpdate()
    {
        if (_isInvisible)
        {
            Vector2 direction = _player.transform.position - transform.position;
            direction.Normalize();
            direction.y = 0;

            CheckForWalls(direction);

            _rigidBody.AddForce(direction * _moveSpeed);

            float distance = _player.transform.position.x - transform.position.x;
            distance = Mathf.Abs(distance);

            if(distance < 3)
            {
                StopInvisibility();
            }
        }
        else if (_isPreparingAttack)
        {
            if (transform.position.y >= _player.transform.position.y)
            {
                Debug.Log("WAHOUUUUH");

                _rigidBody.velocity = Vector2.zero;
                _rigidBody.bodyType = RigidbodyType2D.Kinematic;

                _spriteRenderer.color = Color.red;

                _isPreparingAttack = false;
            }
        }
    }

    protected override void OnMagazineEmpty()
    {
        base.OnMagazineEmpty();

        _spriteRenderer.color = Color.blue;
        _isInvisible = true;

        StartCoroutine(CRT_Invisibility());
    }

    IEnumerator CRT_Invisibility()
    {
        yield return new WaitForSeconds(_maxInvisibilityTime);
        StopInvisibility(true);

        yield break;
    }

    void StopInvisibility(bool timedOut = false)
    {
        StopAllCoroutines();

        _spriteRenderer.color = Color.white;
        _isInvisible = false;


        PrepareMeleeAttack();
    }

    void PrepareMeleeAttack()
    {
        Jump(500f);

        _isPreparingAttack = true;
    }

    void CheckForWalls(Vector2 direction)
    {
        bool isWalkingTowardsWall = Physics2D.Raycast(transform.position, direction, _wallCheckDistance, _wallMask);

        if (isWalkingTowardsWall)
        {
            Jump(80f);
        }
    }

    void Jump(float jumpHeight)
    {
        _rigidBody.AddForce(Vector2.up * jumpHeight, ForceMode2D.Impulse);
    }
}
