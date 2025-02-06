using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FMOD.Studio;
using FMODUnity;

public class RoundManager : MonoBehaviour
{
    public static RoundManager Instance { get; private set; }

    //[SerializeField] private AnimationCurve _demonSpawningCurve = null;
    //[SerializeField] private AnimationCurve _creatureSpawningCurve = null;
    [SerializeField] private float _numberOfDemonsMultiplier = 3f;
    [SerializeField] private float _numberOfCreaturesMultiplier = 1f;
    [SerializeField] private bool _disableEnemySpawning = false;
    public float respawnEventCooldown = 15f;
    public float CurrentRoundPerfectTime { get; set; } = 20f;

    int _numberOfDemonsToSpawn = 1;
    int _numberOfCreaturesOfEachTypeToSpawn = 1;

    List<WorldRoom> _unlockedRooms = new List<WorldRoom>();

    float _respawnEventTime;

    private Dictionary<EnemyType, int> _numberOfEnemiesSpawned = new Dictionary<EnemyType, int>();
    private Dictionary<EnemyType, int> _numberOfEnemiesRemaining = new Dictionary<EnemyType, int>();

    public List<Enemy> CurrentEnemiesInRoom { get; set; } = new List<Enemy>();

    EventInstance _waveStartEvent;
    EventInstance _waveEndEvent;

    GameEvents events;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }

        _waveStartEvent = RuntimeManager.CreateInstance("event:/Misc/WaveStartingSound");
        _waveEndEvent = RuntimeManager.CreateInstance("event:/Misc/WaveEndingSound");
    }

    private void Start()
    {
        events = GameEvents.Instance;
    }

    private void Update()
    {
        if (LevelManager.Instance.IsRoundPlaying)
        {
            if (_respawnEventTime < respawnEventCooldown)
            {
                _respawnEventTime += Time.deltaTime;
                //Debug.Log(_timeLeftUntilRespawnEvent);

                events.RespawnEventTick(_respawnEventTime);
            }
            else
            {
                RespawnEvent();
                _respawnEventTime = 0;
            }
        }
    }

    public void StartRound()
    {
        if (_disableEnemySpawning)
        {
            return;
        }

        Debug.Log("STARTING ROUND");

        CurrentEnemiesInRoom.Clear();
        SetNumberOfEnemiesToSpawn();
        List<GameObject> enemiesToSpawn = GetEnemyObjectsToSpawn();
        SetEnemyCounts();
        SpawnEnemiesInEnemySpawner(enemiesToSpawn);

        _respawnEventTime = 0;

        _waveStartEvent.start();
    }

    void SetNumberOfEnemiesToSpawn()
    {
        int currentRound = LevelManager.Instance.CurrentRound;
        _numberOfDemonsToSpawn = Mathf.CeilToInt((currentRound * _numberOfDemonsMultiplier));
        _numberOfCreaturesOfEachTypeToSpawn = Mathf.CeilToInt((currentRound * _numberOfCreaturesMultiplier)) + 1;
    }

    List<GameObject> GetEnemyObjectsToSpawn()
    {
        List<GameObject> enemyObjectsToSpawn = new List<GameObject>();

        for(int i = 0; i < _numberOfDemonsToSpawn; i++)
        {
            enemyObjectsToSpawn.Add(EnemyPool.Instance.GetRandomEnemy(EnemyType.DEMON, 0));
        }

        for(int i = 0; i < _numberOfCreaturesOfEachTypeToSpawn; i++)
        {
            enemyObjectsToSpawn.Add(EnemyPool.Instance.GetRandomEnemy(EnemyType.ENERGY_CREATURE, 0));
            enemyObjectsToSpawn.Add(EnemyPool.Instance.GetRandomEnemy(EnemyType.HEALTH_CREATURE, 0));
            enemyObjectsToSpawn.Add(EnemyPool.Instance.GetRandomEnemy(EnemyType.ARMOR_CREATURE, 0));
        }

        return enemyObjectsToSpawn;
    }

    void SetEnemyCounts()
    {
        _numberOfEnemiesSpawned.Clear();
        _numberOfEnemiesRemaining.Clear();

        _numberOfEnemiesSpawned.Add(EnemyType.DEMON, _numberOfDemonsToSpawn);
        _numberOfEnemiesSpawned.Add(EnemyType.ENERGY_CREATURE, _numberOfCreaturesOfEachTypeToSpawn);
        _numberOfEnemiesSpawned.Add(EnemyType.HEALTH_CREATURE, _numberOfCreaturesOfEachTypeToSpawn);
        _numberOfEnemiesSpawned.Add(EnemyType.ARMOR_CREATURE, _numberOfCreaturesOfEachTypeToSpawn);
        _numberOfEnemiesSpawned.Add(EnemyType.SPEED_CREATURE, _numberOfCreaturesOfEachTypeToSpawn);

        _numberOfEnemiesRemaining = new Dictionary<EnemyType, int>(_numberOfEnemiesSpawned);
    }

    void SpawnEnemiesInEnemySpawner(List<GameObject> enemiesToSpawn)
    {
        WorldRoom currentRoom = _unlockedRooms[0];
        currentRoom.StartRound();
        currentRoom.EnemySpawner.StartEnemySpawn(enemiesToSpawn);

        Debug.Log("Number of enemies per room: " + enemiesToSpawn.Count);
    }

    public void RemoveEnemyFromRound(Enemy enemy)
    {
        if (_numberOfEnemiesRemaining.ContainsKey(enemy.enemyType))
        {
            _numberOfEnemiesRemaining[enemy.enemyType]--;
            CurrentEnemiesInRoom.Remove(enemy);
        }
        else
        {
            return;
        }

        Debug.Log("RoundManager: Number of enemies remaining: " + _numberOfEnemiesRemaining[EnemyType.DEMON]);

        if (_numberOfEnemiesRemaining[EnemyType.DEMON] <= 0)
        {
            Debug.Log("Round ended!");

            foreach(Enemy script in CurrentEnemiesInRoom)
            {
                script.SoftKill();
            }

            LevelManager.Instance.EndRound();

            _waveEndEvent.setParameterByName("FlawlessClear", LevelManager.Instance.IsRoundFlawless ? 1  : 0);
            _waveEndEvent.setParameterByName("PerfectTimeClear", LevelManager.Instance.IsRoundTimePerfect ? 1 : 0);
            _waveEndEvent.start();
        }
    }

    public void AddRoomToPool(WorldRoom room)
    {
        _unlockedRooms.Add(room);
    }

    void RespawnEvent()
    {
        Debug.Log("RESPAWN EVENT");
        Debug.Log(_numberOfEnemiesSpawned[EnemyType.ENERGY_CREATURE]);

        int numberOfEnergyCreaturesToRespawn = _numberOfEnemiesSpawned[EnemyType.ENERGY_CREATURE] - _numberOfEnemiesRemaining[EnemyType.ENERGY_CREATURE];
        int numberOfHealthCreaturesToRespawn = _numberOfEnemiesSpawned[EnemyType.HEALTH_CREATURE] - _numberOfEnemiesRemaining[EnemyType.HEALTH_CREATURE];
        int numberOfArmorCreaturesToRespawn = _numberOfEnemiesSpawned[EnemyType.ARMOR_CREATURE] - _numberOfEnemiesRemaining[EnemyType.ARMOR_CREATURE];
        int numberOfSpeedCreaturesToRespawn = _numberOfEnemiesSpawned[EnemyType.SPEED_CREATURE] - _numberOfEnemiesRemaining[EnemyType.SPEED_CREATURE];

        List<GameObject> enemiesToRespawn = new List<GameObject>();
        for(int i = 0; i < numberOfEnergyCreaturesToRespawn; i++)
        {
            enemiesToRespawn.Add(EnemyPool.Instance.GetRandomEnemy(EnemyType.ENERGY_CREATURE, 0));
        }
        for (int i = 0; i < numberOfHealthCreaturesToRespawn; i++)
        {
            enemiesToRespawn.Add(EnemyPool.Instance.GetRandomEnemy(EnemyType.HEALTH_CREATURE, 0));
        }
        for (int i = 0; i < numberOfArmorCreaturesToRespawn; i++)
        {
            enemiesToRespawn.Add(EnemyPool.Instance.GetRandomEnemy(EnemyType.ARMOR_CREATURE, 0));
        }
        for (int i = 0; i < numberOfSpeedCreaturesToRespawn; i++)
        {
            enemiesToRespawn.Add(EnemyPool.Instance.GetRandomEnemy(EnemyType.SPEED_CREATURE, 0));
        }

        _numberOfEnemiesRemaining[EnemyType.ENERGY_CREATURE] = _numberOfEnemiesSpawned[EnemyType.ENERGY_CREATURE];
        _numberOfEnemiesRemaining[EnemyType.HEALTH_CREATURE] = _numberOfEnemiesSpawned[EnemyType.HEALTH_CREATURE];
        _numberOfEnemiesRemaining[EnemyType.ARMOR_CREATURE] = _numberOfEnemiesSpawned[EnemyType.ARMOR_CREATURE];
        _numberOfEnemiesRemaining[EnemyType.SPEED_CREATURE] = _numberOfEnemiesSpawned[EnemyType.SPEED_CREATURE];

        WorldRoom currentRoom = _unlockedRooms[0];
        currentRoom.EnemySpawner.StartEnemyRespawn(enemiesToRespawn);

        events.RespawnEvent();
    }
}
