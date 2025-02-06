using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] StudioEventEmitter _musicEmitter = null;

    private void OnEnable()
    {
        GameEvents.Instance.onRoundStarted += OnRoundStarted;
        GameEvents.Instance.onRoundCleared += OnRoundCleared;
        GameEvents.Instance.onEnemiesSpawned += OnEnemiesSpawned;
    }

    private void OnDisable()
    {
        GameEvents.Instance.onRoundStarted -= OnRoundStarted;
        GameEvents.Instance.onRoundCleared -= OnRoundCleared;
        GameEvents.Instance.onEnemiesSpawned -= OnEnemiesSpawned;
    }

    void OnRoundStarted(int roundIndex)
    {
        _musicEmitter.Stop();
    }

    void OnRoundCleared(int roundIndex)
    {
        _musicEmitter.SetParameter("MusicStatus", 1);
    }

    void OnEnemiesSpawned()
    {
        _musicEmitter.Play();
        _musicEmitter.SetParameter("MusicStatus", 0);
    }
}
