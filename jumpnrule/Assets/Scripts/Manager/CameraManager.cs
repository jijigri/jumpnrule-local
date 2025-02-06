using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private Transform _aimPosition = null;
    [SerializeField] private Transform _mousePosition = null;
    [SerializeField] private Transform _player = null;
    [SerializeField] private float _maxCameraDistance = 4;

    private Camera _mainCamera = null;
    CinemachineVirtualCamera _cam;

    private void Start()
    {
        Cursor.visible = false;

        if (_cam == null)
        {
            _cam = GameObject.FindObjectOfType<CinemachineVirtualCamera>();

            _cam.Follow = _aimPosition.transform;
        }

        _mainCamera = Camera.main;
    }

    private void Update()
    {
        Vector2 mousePos = _mainCamera.ScreenToWorldPoint(Input.mousePosition);

        if (_player != null)
        {
            _aimPosition.transform.position = mousePos;
            Vector2 playerToMouse = mousePos - (Vector2)_player.transform.position;
            playerToMouse = Vector2.ClampMagnitude(playerToMouse, _maxCameraDistance);
            _aimPosition.transform.position = (Vector2)_player.transform.position + playerToMouse;
        }

        _mousePosition.position = mousePos;
    }
}
