using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudioManager : MonoBehaviour
{
    [SerializeField] private string _playerJumpSound = "";
    [SerializeField] private string _playerLandSound = "";
    [SerializeField] private string _playerWallSlideSound = "";
    [SerializeField] private string _playerDamageSound = "";
    [SerializeField] private string _playerDeathSound = "";

    StudioEventEmitter _playerStepEvent;
    EventInstance _playerJumpEvent;
    EventInstance _playerLandEvent;
    EventInstance _playerWallSlideEvent;
    EventInstance _playerDamageEvent;
    EventInstance _playerDeathEvent;

    private void Awake()
    {
        _playerStepEvent = GetComponentInChildren<StudioEventEmitter>();
        _playerJumpEvent = RuntimeManager.CreateInstance(_playerJumpSound);
        _playerLandEvent = RuntimeManager.CreateInstance(_playerLandSound);
        _playerWallSlideEvent = RuntimeManager.CreateInstance(_playerWallSlideSound);
        _playerDamageEvent = RuntimeManager.CreateInstance(_playerDamageSound);
        _playerDeathEvent = RuntimeManager.CreateInstance(_playerDeathSound);
    }

    public void PlayJump()
    {
        RuntimeManager.AttachInstanceToGameObject(_playerJumpEvent, transform);
        _playerJumpEvent.start();
    }

    public void PlayLand()
    {
        RuntimeManager.AttachInstanceToGameObject(_playerLandEvent, transform);
        _playerLandEvent.start();
    }

    public void PlayWallSlide()
    {
        RuntimeManager.AttachInstanceToGameObject(_playerWallSlideEvent, transform);
        PLAYBACK_STATE state;
        _playerWallSlideEvent.getPlaybackState(out state);
        if(state != PLAYBACK_STATE.PLAYING)
        {
            _playerWallSlideEvent.start();
        }
    }

    public void StopWallSlide()
    {
        PLAYBACK_STATE state;
        _playerWallSlideEvent.getPlaybackState(out state);
        if (state == PLAYBACK_STATE.PLAYING)
        {
            _playerWallSlideEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        }
    }

    public void PlayDeath()
    {
        StopCoroutine(CRT_StartMuffleSound());
        StopMuffleSound();
        RuntimeManager.AttachInstanceToGameObject(_playerDeathEvent, transform);
        _playerDeathEvent.start();
    }

    public void PlayDamage()
    {
        RuntimeManager.AttachInstanceToGameObject(_playerDamageEvent, transform);
        _playerDamageEvent.start();
    }

    public void StartMuffleSound()
    {
        StartCoroutine(CRT_StartMuffleSound());
    }

    IEnumerator CRT_StartMuffleSound()
    {
        RuntimeManager.StudioSystem.setParameterByName("Muffle", 1);
        yield return new WaitForSeconds(.5f);
        RuntimeManager.StudioSystem.setParameterByName("Muffle", 0.25f);
    }

    public void StopMuffleSound()
    {
        RuntimeManager.StudioSystem.setParameterByName("Muffle", 0f);
    }
}
