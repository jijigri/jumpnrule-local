using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Debug fields that won't stay")]
    [SerializeField] private GameObject _enemyPrefab;

    List<Vector2> _freeSpawnPoints = new List<Vector2>();

    EventInstance _enemySpawningSound;

    ObjectPooler _pooler;

    private void Awake()
    {
        _enemySpawningSound = RuntimeManager.CreateInstance("event:/Misc/EnemySpawningSound");
    }

    private void Start()
    {
        NavMeshGeneration grid = GameObject.FindObjectOfType<NavMeshGeneration>();
        if(grid != null)
        {
            Debug.LogWarning("Finding the only grid in the world will cause issues with multiple room, need to make sure in the future that every room gets their grid");
            
            foreach(NavPoint point in grid.NavPoints)
            {
                if(point.navpointType != NavPointType.NONE && point.navpointType != NavPointType.BUSY)
                {
                    _freeSpawnPoints.Add(point.tileCoordinates);
                }
            }
        }
        else
        {
            Debug.LogError("Couldn't find a reference to the grid");
        }

        _pooler = ObjectPooler.Instance;
    }

    public void StartEnemySpawn(List<GameObject> enemiesToSpawn)
    {
        Debug.Log("Starting spawning: " + enemiesToSpawn.Count);
        StartCoroutine(Crt_StartEnemySpawn(enemiesToSpawn, false));
    }

    public void StartEnemyRespawn(List<GameObject> enemiesToSpawn)
    {
        Debug.Log("Starting spawning: " + enemiesToSpawn.Count);
        StartCoroutine(Crt_StartEnemySpawn(enemiesToSpawn, true));
    }

    IEnumerator Crt_StartEnemySpawn(List<GameObject> enemiesToSpawn, bool isRespawning = false)
    {
        Vector2[] spawnPositions = GetSpawnPositions(enemiesToSpawn.Count); //Select random positions to spawn enemies in

        SpawnEnemyIndicators(spawnPositions);

        yield return new WaitForSeconds(2f);

        StartCoroutine(SpawnEnemies(spawnPositions, enemiesToSpawn));

        if (isRespawning == false)
        {
            _enemySpawningSound.start();
            GameEvents.Instance.EnemiesSpawned();
        }       

        yield break;
    }

    Vector2[] GetSpawnPositions(int numberOfEnemiesToSpawn)
    {
        Vector2[] spawnPositions = new Vector2[numberOfEnemiesToSpawn];
        List<int> usedIndexes = new List<int>();

        for(int i = 0; i < spawnPositions.Length; i++)
        {
            bool foundPosition = false;
            for (int k = 0; k < 5000; k++)
            {
                int randomSpawnPoint = Random.Range(0, _freeSpawnPoints.Count);
                if (!usedIndexes.Contains(randomSpawnPoint))
                {
                    spawnPositions[i] = _freeSpawnPoints[randomSpawnPoint];
                    usedIndexes.Add(randomSpawnPoint);
                    foundPosition = true;
                }

                if(foundPosition == true)
                {
                    break;
                }
                else if(k >= 4999)
                {
                    Debug.LogError("Couldn't find a spawn point that wasn't already used after 5000 tries");
                }
            }
        }

        return spawnPositions;
    }

    void SpawnEnemyIndicators(Vector2[] spawnPositions)
    {
        foreach (Vector2 position in spawnPositions)
        {
            GameObject indicator = _pooler.SpawnObject("EnemySpawningIndicator", position, Quaternion.identity);
        }
    }

    IEnumerator SpawnEnemies(Vector2[] spawnPositions, List<GameObject> enemiesToSpawn)
    {
        Debug.Log("SPAWN POSITIONS: " + spawnPositions.Length);
        Debug.Log("NUMBER OF ENEMIES: " + enemiesToSpawn.Count);

        for(int i = 0; i < spawnPositions.Length; i++)
        {
            Vector2 position = spawnPositions[i];
            GameObject currentEnemyToSpawn;
            if(enemiesToSpawn[i] != null)
            {
                currentEnemyToSpawn = enemiesToSpawn[i];
            }
            else
            {
                currentEnemyToSpawn = EnemyPool.Instance.GetRandomEnemy(EnemyType.ENERGY_CREATURE, 0);
                Debug.LogWarning("EnemySpawner: Couldn't find an enemy to spawn in the list returned by EnemyPool");
            }

            GameObject enemy = Instantiate(currentEnemyToSpawn, position, Quaternion.identity);
            RoundManager.Instance.CurrentEnemiesInRoom.Add(enemy.GetComponent<Enemy>());
        }

        yield break;
    }
}