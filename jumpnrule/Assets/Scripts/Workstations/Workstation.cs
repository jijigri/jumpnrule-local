using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Workstation : MonoBehaviour
{
    [SerializeField] protected bool _canInteractWhenRoundPlaying = false;
    [SerializeField] protected GameObject _outline = null;
    [SerializeField] protected WorldUIPanel _worldUIPanel = null;

    protected bool _isPlayerOn = false;

    protected LevelManager levelManager;

    protected virtual void Start()
    {
        levelManager = LevelManager.Instance;

        GameEvents.Instance.onRoundStarted += OnRoundStarted;
        GameEvents.Instance.onRoundCleared += OnRoundCleared;

        _outline.SetActive(_isPlayerOn);
        _worldUIPanel.gameObject.SetActive(false);
    }

    protected virtual void OnDestroy()
    {
        GameEvents.Instance.onRoundStarted -= OnRoundStarted;
        GameEvents.Instance.onRoundCleared -= OnRoundCleared;
    }

    protected virtual void OnRoundStarted(int roundIndex) { }

    protected virtual void OnRoundCleared(int roundIndex) { }

    protected virtual void Update()
    {
        if (_isPlayerOn)
        {
            if (Input.GetButtonDown("Interact"))
            {
                OnPlayerInteract();
            }
        }
    }

    protected virtual void OnPlayerInteract()
    {

    }

    protected virtual void OnTriggerStay2D(Collider2D collision)
    {
        if (_isPlayerOn == false)
        {
            if (!levelManager.IsRoundPlaying || (levelManager.IsRoundPlaying && _canInteractWhenRoundPlaying))
            {
                _isPlayerOn = true;
                _outline.SetActive(_isPlayerOn);
                _worldUIPanel.gameObject.SetActive(false);
                _worldUIPanel.gameObject.SetActive(true);
            }
        }
    }

    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        _isPlayerOn = false;
        _outline.SetActive(_isPlayerOn);
        _worldUIPanel.ClosePanel();
    }
}
