using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneWayPlatform : MonoBehaviour
{
    EdgeCollider2D _collider;

    GameObject _player;
    PlatformController _playerMovement;

    float _size;

    bool _isActive = true;
    bool _hasReleasedDown = false;

    private void Awake()
    {
        _collider = GetComponent<EdgeCollider2D>();
    }

    private void Start()
    {
        _player = GameObject.FindWithTag("Player");
        _playerMovement = _player.GetComponent<PlatformController>();

        _size = _collider.bounds.size.x + 2;

        if (_collider != null)
            _collider.enabled = false;
    }

    private void Update()
    {
        if (_player == null || _playerMovement == null || _collider == null)
            return;

        if (_isActive)
        {
            if (Vector2.Distance(_player.transform.position, transform.position) > _size)
            {
                _collider.enabled = true;
            }
            else
            {
                if (_playerMovement.GetVelocity().y <= 0)
                {
                    _collider.enabled = true;
                }
                else
                {
                    _collider.enabled = false;
                }
            }
        }


        if (Input.GetAxisRaw("Vertical") < 0)
        {
            if (_isActive == true)
            {
                _isActive = false;
                _collider.enabled = false;
            }
        }
        else
        {
            if (_isActive == false && _hasReleasedDown == false)
            {
                _hasReleasedDown = true;
                StopAllCoroutines();
                StartCoroutine(CRT_DisablePlatform());
            }
        }
    }

    IEnumerator CRT_DisablePlatform()
    {
        _isActive = false;

        yield return new WaitForSeconds(.2f);

        _isActive = true;
        _hasReleasedDown = false;

        yield break;
    }
}
