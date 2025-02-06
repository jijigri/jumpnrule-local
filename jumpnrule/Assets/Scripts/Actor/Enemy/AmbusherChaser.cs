using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbusherChaser : Enemy
{
    [SerializeField] float _predictDistanceFromPlayer = 4f;
    [Range(0, 1)]
    [SerializeField] float _playerSpeedInfluence = .5f;

    PlatformPathfindingMovement _movement;
    PlatformController _playerController;

    protected override void Awake()
    {
        base.Awake();
        _movement = GetComponent<PlatformPathfindingMovement>();
    }

    protected override void Start()
    {
        base.Start();

        _movement.SetInitialSpeed(_movement.GetInitialSpeed() + (LevelManager.Instance.CurrentRound * 2f));
        _playerController = _player.GetComponent<PlatformController>();

        StartCoroutine(PathUpdate());
    }

    IEnumerator PathUpdate()
    {
        while (true)
        {
            _movement.SetTarget(GetAmbushPosition());
            _movement.StartMoving();

            yield return new WaitForSeconds(.25f);
        }
    }

    Vector2 GetAmbushPosition()
    { 
        Vector2 playerPosition = _player.transform.position;
        Vector2 playerVelocity = Vector2.zero;
        if (_playerController != null)
        {
            playerVelocity = _playerController.GetVelocity();
        }

        playerVelocity = playerVelocity.normalized + (playerVelocity.normalized * (playerVelocity.magnitude * _playerSpeedInfluence));

        Vector2 ambushPosition = playerPosition + (playerVelocity * _predictDistanceFromPlayer);

        return ambushPosition;
    }

    private void OnDrawGizmos()
    {
        if (_player != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(GetAmbushPosition(), .5f);
        }
    }
}
