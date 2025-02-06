using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RusherDemonEnemy : Enemy
{
    [Header("Rusher Settings")]
    [SerializeField] private float _dashTime = 2f;
    [SerializeField] private float _dashForce = 2f;
    [SerializeField] private Vector2 _distanceFromPlayerToIncreaseAggression = Vector2.zero;
    [SerializeField] private float _aggressionLevelNeededToDash = 3f;

    [SerializeField] private float _aggressionLevel = 0f;

    bool _isDashing = false;

    EnemyMeleeAttack _meleeAttack;
    ContactDamage _contactDamage;
    PlatformPathfindingMovement _movement;

    protected override void Awake()
    {
        base.Awake();
        _meleeAttack = GetComponent<EnemyMeleeAttack>();
        _contactDamage = GetComponent<ContactDamage>();
        _movement = GetComponent<PlatformPathfindingMovement>();
    }

    protected override void Start()
    {
        base.Start();

        _contactDamage.enabled = false;
        _meleeAttack.enabled = true;

        _movement.SetInitialSpeed(_movement.GetInitialSpeed() + (LevelManager.Instance.CurrentRound * 2f));
        _movement.SetTarget(_player.transform);
        _movement.StartMoving();
    }

    protected override void Update()
    {
        base.Update();

        if(_isDashing == true)
        {
            return;
        }

        float xDistance = _player.transform.position.x - _rigidBody.position.x;
        float yDistance = _player.transform.position.y - _rigidBody.position.y;

        if(Mathf.Abs(xDistance) <= _distanceFromPlayerToIncreaseAggression.x && Mathf.Abs(yDistance) <= _distanceFromPlayerToIncreaseAggression.y)
        {
            _aggressionLevel += Time.deltaTime;
        }
        else
        {
            _aggressionLevel -= Time.deltaTime * .1f;
        }
        _aggressionLevel = Mathf.Clamp(_aggressionLevel, 0, _aggressionLevelNeededToDash * 1.5f);

        if(_aggressionLevel >= _aggressionLevelNeededToDash)
        {
            if (Mathf.Abs(yDistance) < 1)
            {
                Vector2 direction = new Vector2(Mathf.Sign(xDistance), 0);
                StartCoroutine(Dash(direction));
                _aggressionLevel = 0;
            }
        }
    }

    IEnumerator Dash(Vector2 dashDirection)
    {
        _isDashing = true;

        _meleeAttack.enabled = false;

        _spriteRenderer.color = Color.red;
        _movement.SetSpeed(_movement.GetInitialSpeed() / 2);

        yield return new WaitForSeconds(.5f);

        _contactDamage.enabled = true;

        _movement.ResetSpeed();
        _movement.StopMoving();

        float t = 0;
        while(t < _dashTime)
        {
            _rigidBody.AddForce(dashDirection * (_dashForce * 100) * Time.fixedDeltaTime);

            Debug.Log("Trying to dash in " + dashDirection);

            t += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        _contactDamage.enabled = false;

        yield return new WaitForSeconds(.5f);

        _spriteRenderer.color = Color.white;

        _movement.StartMoving();

        _meleeAttack.enabled = true;

        _isDashing = false;

        yield break;
    }

    protected override void OnFalterStart()
    {
        base.OnFalterStart();
        _movement.StopMoving();
    }

    protected override void OnFalterStop()
    {
        base.OnFalterStop();
        _movement.StartMoving();
    }
}
