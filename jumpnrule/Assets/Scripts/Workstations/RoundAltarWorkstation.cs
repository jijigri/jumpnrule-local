using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RoundAltarWorkstation : Workstation
{
    [SerializeField] private TextMeshProUGUI _roundText = null;
    [SerializeField] private Animator _animator = null;
    [SerializeField] private ParticleSystem _startParticles = null;
    [SerializeField] private SpriteRenderer _fillRenderer = null;
    [SerializeField] private Sprite[] _sprites = new Sprite[0];

    float _respawnTime = 0;

    protected override void Start()
    {
        base.Start();

        GameEvents.Instance.OnRespawnEventTick += OnRespawnEventTick;

        _respawnTime = RoundManager.Instance.respawnEventCooldown;

        _roundText.text = "0";
    }

    protected override void OnDestroy()
    {
        GameEvents.Instance.OnRespawnEventTick -= OnRespawnEventTick;
    }

    void OnRespawnEventTick(float value)
    {
        float valueNormalized = value / _respawnTime;
        int spriteIndex = Mathf.FloorToInt(valueNormalized * _sprites.Length);

        if (_sprites.Length > spriteIndex)
        {
            _fillRenderer.sprite = _sprites[spriteIndex];
        }
    }

    protected override void Update()
    {
        base.Update();

        _animator.SetBool("isFire", !levelManager.IsRoundPlaying);
    }

    protected override void OnRoundCleared(int roundIndex)
    {
        base.OnRoundCleared(roundIndex);

        _fillRenderer.enabled = false;

        StartCoroutine(CRT_SpawnMoney(roundIndex));
    }

    protected override void OnRoundStarted(int roundIndex)
    {
        base.OnRoundStarted(roundIndex);

        _fillRenderer.enabled = true;
    }

    IEnumerator CRT_SpawnMoney(int roundIndex)
    {
        int moneyAmount = roundIndex + 5;

        if (LevelManager.Instance.IsRoundFlawless) moneyAmount += 4;
        if (LevelManager.Instance.IsRoundTimePerfect) moneyAmount += 4;

        for (int i = 0; i < moneyAmount; i++)
        {
            ObjectPooler.Instance.SpawnObject("MoneyPickup", transform.position, Quaternion.identity);
            yield return new WaitForSeconds(.1f);
        }

        yield break;
    }

    protected override void OnPlayerInteract()
    {
        base.OnPlayerInteract();

        levelManager.StartRound();
        _roundText.text = levelManager.CurrentRound.ToString();

        _startParticles.Play();

        CameraEffects.Instance.ZoomIn(.25f, .15f);
        CameraEffects.Instance.Shockwave(transform.position, 60f, 0.08f, -0.12f, 1.2f, .5f);
    }
}
