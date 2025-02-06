using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisoningGhost : ShooterEnemy, IHasTriggerChild, IHasTriggerExitChild, IHasTriggerStayChild
{
    [SerializeField] private float _wanderingSpeed = 50;
    [SerializeField] private float _fleeingSpeed = 300;
    [SerializeField] private float _fleeingShootRate = 300;
    [SerializeField] private LayerMask _solidMask = default;
    [SerializeField] private int _maxFramesBeforeDirectionChange = 200;

    bool _isWandering = true;
    bool _isBlocked = false;

    Vector2 _moveDirection;

    float _initialShootRate = 0;

    GameObject _fleeingPlayer = null;

    protected override void Start()
    {
        base.Start();

        _initialShootRate = timeToReload;

        StartShooting(Random.Range(1, 4));
        OnStartWandering();
    }

    protected override void Update()
    {
        base.Update();

        if (_isWandering)
        {
            Debug.DrawRay(transform.position, _moveDirection * .8f, Color.red);
            RaycastHit2D hit = Physics2D.Raycast(transform.position, _moveDirection, .8f, _solidMask);

            if (hit)
            {
                _moveDirection = hit.normal;
            }

            int randomChanceToChangeDirection = Random.Range(0, _maxFramesBeforeDirectionChange);
            if(randomChanceToChangeDirection == 0)
            {
                _moveDirection = Helper.GetRandomDirection();
            }
        }
        else
        {
            Debug.DrawRay(transform.position, _moveDirection * .8f, Color.red);
            RaycastHit2D hit = Physics2D.Raycast(transform.position, _moveDirection, .8f, _solidMask);

            if (hit)
            {
                if (!_isBlocked)
                {
                    _moveDirection = hit.normal;

                    _isBlocked = true;
                    StartCoroutine(CRT_OnBlock());
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (_isWandering)
        {
            _rigidBody.AddForce(_moveDirection * (_wanderingSpeed * 10) * Time.fixedDeltaTime);
        }
        else
        {
            if (_fleeingPlayer != null)
            {
                if (!_isBlocked)
                {
                    _moveDirection = (((Vector2)_fleeingPlayer.transform.position - (Vector2)transform.position).normalized) * -1;
                    _rigidBody.AddForce(_moveDirection * (_fleeingSpeed * 10) * Time.fixedDeltaTime);

                }
                else
                {
                    _rigidBody.AddForce(_moveDirection * (_fleeingSpeed * 10) * Time.fixedDeltaTime);
                }
            }
        }
    }

    void OnStartWandering()
    {
        _isWandering = true;
        timeToReload = _initialShootRate;

        _moveDirection = Helper.GetRandomDirection();

        _spriteRenderer.color = Color.yellow;
    }

    void OnStartFleeing()
    {
        _isWandering = false;
        timeToReload = _fleeingShootRate;

        _spriteRenderer.color = Color.red;
    }

    public void OnChildTriggerEnter(Collider2D collision)
    {
      
    }

    public void OnChildTriggerExit(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StopCoroutine("CRT_StopFleeing");
            StartCoroutine(CRT_StopFleeing());
        }
    }

    IEnumerator CRT_StopFleeing()
    {
        if (_isWandering == true) yield break;
        yield return new WaitForSeconds(1f);
        if (_isWandering == true) yield break;
        _isWandering = true;
        OnStartWandering();
        yield break;
    }

    IEnumerator CRT_OnBlock()
    {
        yield return new WaitForSeconds(1f);
        _isBlocked = false;
        yield break;
    }

    public void OnChildTriggerStay(Collider2D collision)
    {
        if (_isWandering)
        {
            if (collision.CompareTag("Player"))
            {
                StopCoroutine("CRT_StopFleeing");

                _fleeingPlayer = collision.gameObject;
                OnStartFleeing();
            }
        }
    }
}
